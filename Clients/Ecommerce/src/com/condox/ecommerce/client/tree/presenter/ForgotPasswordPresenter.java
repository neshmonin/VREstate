package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.I_Login;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.condox.ecommerce.client.tree.node.ForgotPasswordNode;
import com.condox.ecommerce.client.tree.node.LoginNode;
import com.google.gwt.user.client.ui.Widget;

public class ForgotPasswordPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(ForgotPasswordPresenter presenter);

//		String getUserLogin();

//		String getUserPassword();

		Widget asWidget();
	}

	private I_Display display = null;
	private ForgotPasswordNode node = null;

	public ForgotPasswordPresenter(I_Display newDisplay, ForgotPasswordNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

//	private String uid = "";
//	private String pwd = "";
	
	// Events
	public void onLogin() {
		node.setState(NodeStates.Guest);
		node.next();
		
//		uid = display.getUserLogin().trim();
//		pwd = display.getUserPassword().trim();
//		EcommerceTree.set(Field.UserLogin, new Data(uid));
//		EcommerceTree.set(Field.UserPassword, new Data(pwd));
//		
////		if (!model.isValid()) {
////			Window.alert("Not valid! Please, check and try again!");
////			return;
////		}
//		
//		User.UserRole role = User.UserRole.Agent;
//		if ((uid == null || uid.isEmpty()) ||
//			("web".equalsIgnoreCase(uid) && "web".equalsIgnoreCase(pwd)))
//			role = User.UserRole.Visitor;
//
//		User.Login(this, uid, pwd, role);
	}

	public void onSubmit() {
		node.setState(NodeStates.Submit);
		node.next();
	}

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
