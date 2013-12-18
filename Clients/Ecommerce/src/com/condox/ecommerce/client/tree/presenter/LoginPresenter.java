package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.I_Login;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.LoginNode;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter implements I_Presenter, I_Login {

	public static interface I_Display extends I_Contained {
		void setPresenter(LoginPresenter presenter);

		String getUserEmail();

		String getUserPassword();

		Widget asWidget();
	}

	private I_Display display = null;
	private LoginNode node = null;

	public LoginPresenter(I_Display newDisplay, LoginNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

	// Events
	public void onLogin() {
		String email = display.getUserEmail().trim();
		String password = display.getUserPassword().trim();

		// TODO
		UserRole role = null;
		
		if (email.isEmpty() && password.isEmpty()) {
			email = "web";
			password = "web";
			role = UserRole.Visitor;
			
			email = "adminan";
			password = "smelatoronto";
			role = UserRole.SuperAdmin;
			
			
//			email = "eugene.simonov@3dcondox.com";
//			password = "3dcondoms";
//			role = UserRole.SellingAgent;
		} else
			role = UserRole.SuperAdmin;
		
		
		node.setData(Field.UserEmail, new Data(email));
		node.setData(Field.UserPassword, new Data(password));
		node.setData(Field.UserRole, new Data(role.name()));
		User.Login(this, email, password, role);
	}

	public void onForgotPassword() {
		String email = display.getUserEmail().trim();
		String password = display.getUserPassword().trim();
		node.setData(Field.UserEmail, new Data(email));
		node.setData(Field.UserPassword, new Data(password));
		node.setState(NodeStates.ForgotPassword);
		node.next();
	}

	public void onLoginSucceed() {
		Data data = node.getData(Field.UserRole);
		if (data != null) {
			UserRole role = UserRole.valueOf(data.asString());
			if (UserRole.Visitor.equals(role))
				node.setState(NodeStates.Guest);
			else
				node.setState(NodeStates.Agent);
			node.next();
		}
	}

	@Override
	public void onLoginFailed(Throwable exception) {
		Window.alert("Login failed.");
	}

	public void onClose() {
		node.getTree().close();
	}
}
