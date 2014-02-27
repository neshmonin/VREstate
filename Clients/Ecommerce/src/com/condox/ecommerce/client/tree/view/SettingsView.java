package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.presenter.SettingsPresenter;
import com.condox.ecommerce.client.tree.presenter.SettingsPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.user.client.ui.PasswordTextBox;

public class SettingsView extends Composite implements I_Display {

	private static LoginViewUiBinder uiBinder = GWT
			.create(LoginViewUiBinder.class);
	@UiField Button buttonClose;
	@UiField Button buttonChangeEmail;
	@UiField Button buttonChangePassword;
	@UiField TextBox textEmail;
	@UiField PasswordTextBox textOldPassword;
	@UiField PasswordTextBox textNewPassword;
	@UiField PasswordTextBox textNewPassword2;

	interface LoginViewUiBinder extends UiBinder<Widget, SettingsView> {
	}

	private SettingsPresenter presenter = null;
	private String oldPrimaryEmail = "";

	public SettingsView() {
		initWidget(uiBinder.createAndBindUi(this));
		Timer updateView = new Timer() {

			@Override
			public void run() {
				String email = textEmail.getValue();
				boolean valid = true;
				// Validating email
				valid &= !oldPrimaryEmail.equals(email);
				valid &= email.matches("[a-z0-9_-]+(\\.[a-z0-9_-]+)*@[a-z0-9_-]+(\\.[a-z0-9_-]+)+");
				buttonChangeEmail.setEnabled(valid);
				//Validating password
				valid = true;
				valid &= !textOldPassword.getValue().isEmpty();
				valid &= !textNewPassword.getValue().isEmpty();
				valid &= !textNewPassword2.getValue().isEmpty();
				valid &= textNewPassword.getValue().equals(textNewPassword2.getValue());
				buttonChangePassword.setEnabled(valid);
			}
			
		};
		updateView.scheduleRepeating(1000);
	}

	@Override
	public void setPresenter(SettingsPresenter presenter) {
		this.presenter = presenter;
	}

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
	
	@UiHandler("textEmail")
	void onTextEmailChange(ChangeEvent event) {
	}

	@Override
	public void setEmail(String newEmail) {
		textEmail.setValue(newEmail);
		oldPrimaryEmail = newEmail;
	}
}
