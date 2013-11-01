package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.I_Login;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.condox.ecommerce.client.tree.model.LoginModel;
import com.google.gwt.http.client.URL;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class LoginPresenter implements I_Presenter, I_Login {

	public static interface I_Display {
		void setPresenter(LoginPresenter presenter);

		String getUserLogin();

		String getUserPassword();

		Widget asWidget();
	}

	private I_Display display = null;
	private LoginModel model = null;

	public LoginPresenter(I_Display display, LoginModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(this.display.asWidget());
	}

	public void onEnter() {
		final String uid = display.getUserLogin();
		final String pwd = display.getUserPassword();
		EcommerceTree.set(Field.UserLogin, new Data(uid));
		EcommerceTree.set(Field.UserPassword, new Data(pwd));
		
		if (!model.isValid()) {
			Window.alert("Not valid! Please, check and try again!");
			return;
		}
		
		String role = "visitor";
		String url = Options.getUserLogin(uid, pwd, role);
		EcommerceTree.transitState(State.Guest); // for role == "visitor"

		url = URL.encode(url);

		// GET.send(url);
		User.Login(this);
	}

	@Override
	public void onLoginSucceed(){
		model.next();
	}

	@Override
	public void onLoginFailed(Throwable exception) {}
}
