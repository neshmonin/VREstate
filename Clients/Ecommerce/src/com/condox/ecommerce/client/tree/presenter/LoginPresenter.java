package com.condox.ecommerce.client.tree.presenter;

import java.util.Date;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.api.I_RequestCallback;
import com.condox.ecommerce.client.tree.api.RequestType;
import com.condox.ecommerce.client.tree.api.ServerAPI;
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
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter implements I_Presenter, I_RequestCallback {

	public static interface I_Display {
		void setPresenter(LoginPresenter presenter);

		String getLogin();

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
	private ServerAPI api = new ServerAPI();

	private String role;
	private String uid;
	private String pwd;

	private RequestCallback loginHandler = new RequestCallback() {

		@Override
		public void onResponseReceived(Request request, Response response) {
			if (response.getStatusCode() == 200) {
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				if (obj.get("userId") != null)
					if (obj.get("UserId").isNumber() != null) {
						int UserId = (int) obj.get("UserId").isNumber().doubleValue();
						// TODO Save User.id to else place
						User.id = String.valueOf(UserId);
					}
				if (obj.get("sid") != null)
					if (obj.get("sid").isString() != null)
						// TODO Save User.SID to else place
						User.SID = obj.get("sid").isString().stringValue();
				finishLogin();
				
//				User.id = String.valueOf((int) result.get("userId").isNumber()
//						.doubleValue());
//				User.SID = result.get("sid").isString().stringValue();
//				JSONObject data = new JSONObject();
//				data.put("sid", new JSONString(User.SID));
//				api.execute(RequestType.KeepSessionAlive, data, this);
//				finishLogin();
			} else {
				if ("agent".equals(role)) {
					role = "superadmin";
					startLogin();
				} else
					Window.alert("Error while logining");
			}
		}

		@Override
		public void onError(Request request, Throwable exception) {
			if ("agent".equals(role)) {
				role = "superadmin";
				startLogin();
			} else
				Window.alert("Error while logining");

		}
	};

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
		if (login != null)
			display.setLogin(login);
		container.clear();
		container.add(display.asWidget());
	}

	// Events
	public void onLogin() {
		uid = display.getLogin().trim();
		pwd = display.getPassword().trim();
		role = "agent";
		if (uid.isEmpty() && pwd.isEmpty()) {
			role = "visitor";
			uid = "web";
			pwd = "web";
		}
		startLogin();
	}

	private void startLogin() {
		UserInfo info = new UserInfo();
		info.setLogin(uid);
		info.setPassword(pwd);
		if ("visitor".equals(role))
			info.setRole(UserRole.Visitor);
		if ("agent".equals(role))
			info.setRole(UserRole.SellingAgent);
		if ("superadmin".equals(role))
			info.setRole(UserRole.SuperAdmin);
		User.role = info.getRole();
		tree.setData(Field.UserInfo, new Data(info));
		// ---------------------
		JSONObject data = new JSONObject();
		data.put("role", new JSONString(role));
		data.put("uid", new JSONString(uid));
		data.put("pwd", new JSONString(pwd));

		api.execute(RequestType.Login, data, this);

//		ServerAPI.login(role, uid, pwd, loginHandler);
	}

	private void finishLogin() {
		onLoginSucceed();
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

	public void onLoginSucceed() {
		display.beforeClose();
		Data data = tree.getData(Field.UserInfo);
		Cookies.setCookie("login", display.getLogin(),
				new Date(new Date().getTime() + 1000 * 60 * 60 * 24));
		if (data != null) {
			UserInfo info = new UserInfo();
			info.fromJSONObject(data.asJSONObject());
			UserRole role = info.getRole();
			if (UserRole.Visitor.equals(role))
				tree.next(Actions.Guest);
			else
				tree.next(Actions.Agent);
		}
	}

	public void onClose() {
		tree.close();
	}

	// ----------------------------------------------------
	// private ServerAPI api = new ServerAPI();
	//
	// private void keepAlive() {
	// JSONObject data = new JSONObject();
	// data.put("keepAlivePeriodSecs", new JSONNumber(10));
	// api.execute(RequestType.KeepAlive, data, this);
	// }
	//
	// private void reLogin() {
	// JSONObject data = new JSONObject();
	// data.put("reloginDelaySecs", new JSONNumber(10));
	// api.execute(RequestType.Relogin, data, this);
	// }

	@Override
	public void onOK(JSONObject result) {
		// if (RequestType.KeepSessionAlive.equals(api.getType())) {
		// return;
		// }
		User.id = String.valueOf((int) result.get("userId").isNumber()
				.doubleValue());
		User.SID = result.get("sid").isString().stringValue();
		JSONObject data = new JSONObject();
		data.put("sid", new JSONString(User.SID));
		api.execute(RequestType.KeepSessionAlive, data, this);
		finishLogin();
	}

	@Override
	public void onError() {
		if ("agent".equals(role)) {
			role = "superadmin";
			startLogin();
		} else
			Window.alert("Error while logining");
	}

}
