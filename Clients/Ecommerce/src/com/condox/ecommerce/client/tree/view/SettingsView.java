package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.presenter.SettingsPresenter;
import com.condox.ecommerce.client.tree.presenter.SettingsPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;

public class SettingsView extends Composite implements I_Display {

	private static LoginViewUiBinder uiBinder = GWT
			.create(LoginViewUiBinder.class);
	@UiField Button buttonClose;
	@UiField Button buttonChangeEmail;
	@UiField Button buttonChangePassword;
	@UiField TextBox textEmail;
	@UiField TextBox textOldPassword;
	@UiField TextBox textNewPassword;
	@UiField TextBox textNewPassword2;

	interface LoginViewUiBinder extends UiBinder<Widget, SettingsView> {
	}

	private SettingsPresenter presenter = null;
	private boolean user = false;
	private boolean guest = true;

	public SettingsView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(SettingsPresenter presenter) {
		this.presenter = presenter;
	}

//	@Override
//	public String getUserLogin() {
////		if (user)
////			return textUserLogin.getValue();
////		else if (guest)
////			return "web";
//		return "adminan";
//
//	}
//
//	@Override
//	public String getUserPassword() {
////		if (user)
////			return textUserPassword.getValue();
////		else if (guest)
////			return "web";
//		return "smelatoronto";
//	}
//
//
//	@UiHandler("textUserLogin")
//	void onTextUserLoginValueChange(ValueChangeEvent<String> event) {
//
//	}
//
//	@UiHandler("textUserLogin")
//	void onTextUserLoginKeyUp(KeyUpEvent event) {
//		updateButtonEnter();
//	}
//
//	@UiHandler("textUserPassword")
//	void onTextUserPasswordKeyUp(KeyUpEvent event) {
//		updateButtonEnter();
//	}
//
//	private void updateButtonEnter() {
//		user = !textUserLogin.getValue().isEmpty();
//		user &= !textUserPassword.getValue().isEmpty();
//		guest = textUserLogin.getValue().isEmpty();
//		guest &= textUserPassword.getValue().isEmpty();
//		if (user) {
//			buttonEnter.setEnabled(true);
//			buttonEnter.setText("Order as a User");
//		} else if (guest) {
//			buttonEnter.setEnabled(true);
//			buttonEnter.setText("Order as a Guest");
//		} else {
//			buttonEnter.setEnabled(false);
//			buttonEnter.setText("Order");
//		}
//
//	}
	@UiHandler("buttonClose")
	void onButtonCloseClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}
	@UiHandler("buttonChangeEmail")
	void onButtonChangeEmailClick(ClickEvent event) {
		if (presenter != null) {
			String newEmail = textEmail.getValue();
			presenter.onChangeEmail(newEmail);
		}
	}
	@UiHandler("buttonChangePassword")
	void onButtonChangePasswordClick(ClickEvent event) {
		if (presenter != null) {
			String oldPassword = textOldPassword.getValue();
			String newPassword = textNewPassword.getValue();
			String newPassword2 = textNewPassword2.getValue();
			presenter.onChangePassword(oldPassword, newPassword, newPassword2);
		}
	}
}
