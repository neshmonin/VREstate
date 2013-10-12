package com.condox.order.client.wizard.view;

import com.condox.order.client.wizard.Wizard;
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
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.dom.client.KeyUpEvent;

public class LoginView extends Composite implements I_Display {

	private static LoginViewUiBinder uiBinder = GWT
			.create(LoginViewUiBinder.class);
	@UiField
	TextBox textUserLogin;
	@UiField
	TextBox textUserPassword;
	@UiField
	Button buttonEnter;
	@UiField
	Button buttonCancel;

	interface LoginViewUiBinder extends UiBinder<Widget, LoginView> {
	}

	private LoginPresenter presenter = null;
	private boolean user = false;
	private boolean guest = true;

	public LoginView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(LoginPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public String getUserLogin() {
		if (user)
			return textUserLogin.getValue();
		else if (guest)
			return "web";
		return "";

	}

	@Override
	public String getUserPassword() {
		if (user)
			return textUserPassword.getValue();
		else if (guest)
			return "web";
		return "";
	}

	@UiHandler("buttonEnter")
	void onButtonEnterClick(ClickEvent event) {
		presenter.onEnter();
	}

	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		Wizard.cancel();
	}

	@UiHandler("textUserLogin")
	void onTextUserLoginValueChange(ValueChangeEvent<String> event) {

	}

	@UiHandler("textUserLogin")
	void onTextUserLoginKeyUp(KeyUpEvent event) {
		updateButtonEnter();
	}

	@UiHandler("textUserPassword")
	void onTextUserPasswordKeyUp(KeyUpEvent event) {
		updateButtonEnter();
	}

	private void updateButtonEnter() {
		user = !textUserLogin.getValue().isEmpty();
		user &= !textUserPassword.getValue().isEmpty();
		guest = textUserLogin.getValue().isEmpty();
		guest &= textUserPassword.getValue().isEmpty();
		if (user) {
			buttonEnter.setEnabled(true);
			buttonEnter.setText("Order as a User");
		} else if (guest) {
			buttonEnter.setEnabled(true);
			buttonEnter.setText("Order as a Guest");
		} else {
			buttonEnter.setEnabled(false);
			buttonEnter.setText("Order");
		}

	}
}
