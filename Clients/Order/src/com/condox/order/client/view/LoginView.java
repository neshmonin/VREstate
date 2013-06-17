package com.condox.order.client.view;

import com.condox.order.client.presenter.LoginPresenter;
import com.condox.order.client.presenter.LoginPresenter.IDisplay;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasText;
import com.google.gwt.user.client.ui.ResizeComposite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.TextBox;

public class LoginView extends ResizeComposite implements LoginPresenter.IDisplay {

	private static LoginViewUiBinder uiBinder = GWT
			.create(LoginViewUiBinder.class);
	@UiField TextBox textName;
	@UiField TextBox textPassword;
	@UiField Button buttonLogin;
	@UiField Button buttonGuestLogin;
	private LoginPresenter presenter;
	private boolean guestMode = false;

	interface LoginViewUiBinder extends UiBinder<Widget, LoginView> {
	}

	public LoginView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(LoginPresenter presenter) {
		this.presenter = presenter;
	}
	@UiHandler("buttonGuestLogin")
	void onButtonGuestLoginClick(ClickEvent event) {
		guestMode = true;
		presenter.onLogin();
	}

	@Override
	public String getName() {
		return textName.getText();
	}

	@Override
	public String getPassword() {
		return textPassword.getText();
	}

	@Override
	public Boolean isGuestMode() {
		return guestMode;
	}
}
