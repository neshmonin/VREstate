package com.condox.ecommerce.client.tree.presenter;

import java.util.Date;

import com.condox.clientshared.communication.I_Login;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.google.gwt.user.client.Cookies;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter implements I_Presenter, I_Login {

	public static interface I_Display extends I_Contained {
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
	public void go(I_Container container) {
		String login = Cookies.getCookie("login");
		if (login != null)
			display.setLogin(login);
		container.clear();
		container.add(display);
	}

	@Override
	public void onLoginFailed(Throwable exception) {
		if ((role == UserRole.Visitor) || (role == UserRole.SuperAdmin)) {
			display.beforeClose();
			Window.alert("Login error");
		} else
			onLogin();
	}

	private UserRole role = null;

	// Events
	public void onLogin() {
		String login = display.getLogin().trim();
		String password = display.getPassword().trim();

		if (login.isEmpty() && password.isEmpty()) {
			login = "web";
			password = "web";
			role = UserRole.Visitor;
		} else {
			if (role == UserRole.Agent)
				role = UserRole.SuperAdmin;
			else
				role = UserRole.Agent;
		}

//		UserInfo info = new UserInfo(tree.getJSONData(null, Field.UserInfo));
		UserInfo info = new UserInfo();
		info.setLogin(login);
		info.setPassword(password);
		info.setRole(role);
//		tree.setJSONData(Field.UserInfo, info.toJSONObject());
		tree.setData(Field.UserInfo, new Data(info));
		User.Login(this, login, password, role);
	}

	public void onForgotPassword() {
		String login = display.getLogin().trim();
		String password = display.getPassword().trim();
		UserRole role = UserRole.Agent; // ?
		//
//		UserInfo userInfo = UserInfo.fromJSON(tree.getJSONData(Field.UserInfo));
		UserInfo userInfo = new UserInfo();
		userInfo.setLogin(login);
		userInfo.setPassword(password);
		userInfo.setRole(role);
//		tree.setJSONData(Field.UserInfo, userInfo.toJSONObject());

		tree.setData(Field.UserInfo, new Data(userInfo));
		//
		tree.next(Actions.ForgotPassword);
	}

	public void onLoginSucceed() {
		display.beforeClose();
		Data data = tree.getData(Field.UserInfo);
		Cookies.setCookie("login", display.getLogin(), new Date(new Date().getTime() + 1000 * 60 * 60 * 24 ));
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

}
