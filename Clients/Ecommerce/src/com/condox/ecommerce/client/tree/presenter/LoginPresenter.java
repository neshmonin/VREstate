package com.condox.ecommerce.client.tree.presenter;

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
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter implements I_Presenter, I_Login {

	public static interface I_Display extends I_Contained {
		void setPresenter(LoginPresenter presenter);

		String getUserLogin();

		String getUserPassword();

		UserRole getUserRole();

		Widget asWidget();
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
		container.clear();
		container.add(display);
	}
	
	@Override
	public void onLoginFailed(Throwable exception) {
		Window.alert("Login failed.");
	}

	// Events
	public void onLogin() {
		String login = display.getUserLogin().trim();
		String password = display.getUserPassword().trim();
		UserRole role = display.getUserRole();

		UserInfo userInfo = new UserInfo();
		userInfo.setLogin(login);
		userInfo.setPassword(password);
		userInfo.setRole(role);
		tree.setData(Field.UserInfo, new Data(userInfo));
		User.Login(this, login, password, role);
	}

	public void onForgotPassword() {
		String login = display.getUserLogin().trim();
		String password = display.getUserPassword().trim();
		UserRole role = display.getUserRole();
		
		UserInfo userInfo = new UserInfo();
		userInfo.setLogin(login);
		userInfo.setPassword(password);
		userInfo.setRole(role);
		tree.setData(Field.UserInfo, new Data(userInfo));

		tree.next(Actions.ForgotPassword);
	}

	public void onLoginSucceed() {
		Data data = tree.getData(Field.UserInfo);
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
