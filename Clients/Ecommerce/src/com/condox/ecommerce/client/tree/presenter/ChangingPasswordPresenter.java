package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.ChangingPasswordNode;
import com.google.gwt.user.client.ui.Widget;

public class ChangingPasswordPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(ChangingPasswordPresenter presenter);

//		String getUserLogin();
//
//		String getUserPassword();

		Widget asWidget();
	}

	private I_Display display = null;
	private ChangingPasswordNode node = null;

	public ChangingPasswordPresenter(I_Display newDisplay, ChangingPasswordNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

	public void onBackToLogin() {
		node.setState(NodeStates.Ok);
		node.next();
	}

//	private String uid = "";
//	private String pwd = "";
	
	// Events
//	public void onLogin() {
//		node.setState(NodeStates.Guest);
//		node.next();
////		uid = display.getUserLogin().trim();
////		pwd = display.getUserPassword().trim();
////		EcommerceTree.set(Field.UserLogin, new Data(uid));
////		EcommerceTree.set(Field.UserPassword, new Data(pwd));
////		
//////		if (!model.isValid()) {
//////			Window.alert("Not valid! Please, check and try again!");
//////			return;
//////		}
////		
////		User.UserRole role = User.UserRole.Agent;
////		if ((uid == null || uid.isEmpty()) ||
////			("web".equalsIgnoreCase(uid) && "web".equalsIgnoreCase(pwd)))
////			role = User.UserRole.Visitor;
////
////		User.Login(this, uid, pwd, role);
//	}
//
//	public void onForgotPassword() {
//		node.setState(NodeStates.ForgotPassword);
//		node.next();
//	}

//	@Override
//	public void onLoginSucceed() {
////		if (("web".equalsIgnoreCase(uid)) && ("web".equalsIgnoreCase(pwd)))
////			EcommerceTree.transitState(State.Guest);
////		else
////			EcommerceTree.transitState(State.Agent);
////		EcommerceTree.set(Field.UserId, new Data(User.id));
////		
////		model.next();
//	}
//
//	@Override
//	public void onLoginFailed(Throwable exception) {}
}
