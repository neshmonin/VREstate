package com.condox.ecommerce.client.tree.presenter;

import java.util.Date;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.model.LoginModel;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.api.RequestType;
import com.condox.ecommerce.client.tree.api.ServerAPI;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.Cookies;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter implements I_Presenter/* , I_RequestCallback */{

	public static interface I_Display {
		void setPresenter(LoginPresenter presenter);

		HasClickHandlers getLoginButton();

		String getLogin();

		// void setData(LoginModel data);

		void setLogin(String login);

		//
		String getPassword();

		//
		// UserRole getUserRole();

		Widget asWidget();

		void beforeClose();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;
	// private ServerAPI api = new ServerAPI();

	// private String role;
	// private String uid;
	// private String pwd;
	// private UserInfo userInfo = new UserInfo();
	private LoginModel loginModel = new LoginModel();
	private final PopupPanel logining = new PopupPanel();

	private void bind() {
		display.getLoginButton().addClickHandler(new ClickHandler() {

			@Override
			public void onClick(ClickEvent event) {
				processLogin();
			}
		});
	}

	@Override
	public void setView(Composite view) {
		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

	@Override
	public void go(HasWidgets container) {
		String login = Cookies.getCookie("login");
		if (login != null) {
			display.setLogin(login);
			loginModel.setUid(login);
			// display.setData(loginModel);
		}
		bind();
		container.clear();
		container.add(display.asWidget());
	}

	// Events
	public void processLogin() {
		logining.clear();
		logining.setModal(true);
		logining.setGlassEnabled(true);
		logining.add(new Label("Logining, please wait..."));
		logining.center();

		loginModel.setUid(display.getLogin().trim());
		loginModel.setPwd(display.getPassword().trim());

		String uid = loginModel.getUid();
		String pwd = loginModel.getPwd();
		if ("web".equals(uid) && "web".equals(pwd))
			uid = "";
		Cookies.setCookie("login", display.getLogin().trim(), new Date(
				new Date().getTime() + 1000 * 60 * 60 * 24));

		tree.setData(Field.LoginModel, new Data(loginModel));

		ServerAPI.login(loginModel, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				if (response.getStatusCode() == 200) {
					String json = response.getText();
					if (json != null && !json.isEmpty()) {
						JSONObject obj = JSONParser.parseStrict(json)
								.isObject();
						if (obj != null) {
							loginOK(obj);
							return;
						}
					}
				}
				loginError();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				loginError();
			}
		});
	}

	private void loginOK(JSONObject obj) {
		loginModel.fromJSONObject(obj);
		tree.setData(Field.LoginModel, new Data(loginModel));

		UserInfo userInfo = new UserInfo();
		userInfo.fromJSONObject(obj);
		tree.setData(Field.UserInfo, new Data(userInfo));
		User.id = String.valueOf(userInfo.getId());

		User.SID = loginModel.getSid();
		User.role = loginModel.getRole();
		// User.id = String.valueOf(userInfo.getId());
		// User.SID = obj.get("sid").isString().stringValue();
		logining.hide();

		JSONObject data = new JSONObject();
		data.put("sid", new JSONString(User.SID));
		new ServerAPI().execute(RequestType.KeepSessionAlive, data, null);

		if (UserRole.Visitor.equals(loginModel.getRole()))
			tree.next(Actions.Guest);
		else
			tree.next(Actions.Agent);
	}

	private void loginError() {
		if (!UserRole.SuperAdmin.equals(loginModel.getRole())) {
			loginModel.setRole(UserRole.SuperAdmin);
			processLogin();
		} else {
			logining.hide();
			Window.alert("Error while logining.");
		}
	}

	public void onForgotPassword() {
		String login = display.getLogin().trim();
		String password = display.getPassword().trim();
		UserRole role = UserRole.Agent; // ?
		//
		// UserInfo userInfo =
		// UserInfo.fromJSON(tree.getJSONData(Field.UserInfo));
		UserInfo userInfo = new UserInfo();
		userInfo.setLogin(login);
		userInfo.setPassword(password);
		userInfo.setRole(role);
		// tree.setJSONData(Field.UserInfo, userInfo.toJSONObject());

		tree.setData(Field.UserInfo, new Data(userInfo));
		//
		tree.next(Actions.ForgotPassword);
	}

	public void onClose() {
		tree.close();
	}

}
