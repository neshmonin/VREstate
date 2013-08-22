package com.condox.order.client.wizard.view;

import com.condox.order.client.wizard.presenter.LoginPresenter;
import com.condox.order.client.wizard.presenter.LoginPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.TextBox;

public class LoginView extends Composite implements I_Display {

	private static LoginViewUiBinder uiBinder = GWT
			.create(LoginViewUiBinder.class);
	@UiField TextBox textUserLogin;
	@UiField TextBox textUserPassword;
	@UiField Button buttonEnter;
	@UiField Button buttonGuestEnter;
	
	interface LoginViewUiBinder extends UiBinder<Widget, LoginView> {
	}
	
	private LoginPresenter presenter = null;

	public LoginView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(LoginPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public String getUserLogin() {
		return textUserLogin.getValue();
	}

	@Override
	public String getUserPassword() {
		return textUserPassword.getValue();
	}
	@UiHandler("buttonEnter")
	void onButtonEnterClick(ClickEvent event) {
		presenter.onEnter();
	}
	@UiHandler("buttonGuestEnter")
	void onButtonGuestEnterClick(ClickEvent event) {
		textUserLogin.setValue("web");
		textUserPassword.setValue("web");
		presenter.onEnter();
	}
}
