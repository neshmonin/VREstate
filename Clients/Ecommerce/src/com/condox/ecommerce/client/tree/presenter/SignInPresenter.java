package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.I_Login;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.condox.ecommerce.client.tree.model.LoginModel;
import com.google.gwt.user.client.ui.Widget;

public class SignInPresenter implements I_Presenter, I_Login {

	public static interface I_Display extends I_Contained {
		void setPresenter(SignInPresenter presenter);

		String getUserLogin();

		String getUserPassword();

		Widget asWidget();
	}

	private I_Display display = null;
	private LoginModel model = null;

	public SignInPresenter(I_Display display, LoginModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

	private String uid = "";
	private String pwd = "";
	
	public void onLogin() {
		uid = display.getUserLogin().trim();
		pwd = display.getUserPassword().trim();
		EcommerceTree.set(Field.UserLogin, new Data(uid));
		EcommerceTree.set(Field.UserPassword, new Data(pwd));
		
//		if (!model.isValid()) {
//			Window.alert("Not valid! Please, check and try again!");
//			return;
//		}
		
		User.UserRole role = User.UserRole.Agent;
		if ((uid == null || uid.isEmpty()) ||
			("web".equalsIgnoreCase(uid) && "web".equalsIgnoreCase(pwd)))
			role = User.UserRole.Visitor;

		User.Login(this, uid, pwd, role);
	}

	@Override
	public void onLoginSucceed() {
		if (("web".equalsIgnoreCase(uid)) && ("web".equalsIgnoreCase(pwd)))
			EcommerceTree.transitState(State.Guest);
		else
			EcommerceTree.transitState(State.Agent);
		EcommerceTree.set(Field.UserId, new Data(User.id));
		
		model.next();
	}

	@Override
	public void onLoginFailed(Throwable exception) {}
}
