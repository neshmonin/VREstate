package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.presenter.LoginPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.KeyCodes;
import com.google.gwt.event.dom.client.KeyUpEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.FocusPanel;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PasswordTextBox;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;

public class LoginView extends Composite implements I_Display {

	private static LoginViewUiBinder uiBinder = GWT
			.create(LoginViewUiBinder.class);
	@UiField
	TextBox login;
	@UiField
	Button enter;
	@UiField
	Hyperlink forgotPassword;
	@UiField
	Button close;
	@UiField
	PasswordTextBox password;
	@UiField
	FocusPanel focusPanel;
	@UiField
	HorizontalPanel hpfp;

	interface LoginViewUiBinder extends UiBinder<Widget, LoginView> {
	}
	
	private LoginPresenter presenter = null;
	private PopupPanel logining = new PopupPanel();

	public LoginView() {
		Timer update = new Timer() {

			@Override
			public void run() {
				if (login.getText().isEmpty() && password.getText().isEmpty())
					enter.setText("Order as a Guest");
				else
					enter.setText("Login");

				if (login.getText().isEmpty())
					forgotPassword.setVisible(false);
				else
					forgotPassword.setVisible(true);

			}

		};
		update.scheduleRepeating(500);
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(LoginPresenter presenter) {
		this.presenter = presenter;
	}

	@UiHandler("enter")
	void onEnterClick(ClickEvent event) {
		enter();
	}

	@UiHandler("forgotPassword")
	void onForgotPasswordClick(ClickEvent event) {
		if (presenter != null)
			presenter.onForgotPassword();
	}

	@UiHandler("close")
	void onCloseClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}

	@Override
	public String getLogin() {
		return login.getText();
	}

	@Override
	public String getPassword() {
		// TODO Auto-generated method stub
		return password.getText();
	}

	@UiHandler("focusPanel")
	void onFocusPanelKeyUp(KeyUpEvent event) {
		if (event.getNativeKeyCode() == KeyCodes.KEY_ENTER)
			enter();
	}
	
	private void enter() {
		logining.clear();
		logining.setModal(true);
		logining.setGlassEnabled(true);
		logining.add(new Label("Logining, please wait..."));
		logining.center();
		
		if (presenter != null) {
			presenter.onLogin();
			new Timer() {

				@Override
				public void run() {
					logining.hide();
				}}.schedule(5000);
		}
	}

	@Override
	public void beforeClose() {
		logining.hide();
	}

	@Override
	public void setLogin(String login) {
		this.login.setText(login);
	}
}
