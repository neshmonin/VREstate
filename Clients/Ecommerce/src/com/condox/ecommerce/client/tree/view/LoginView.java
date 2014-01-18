package com.condox.ecommerce.client.tree.view;

import com.condox.clientshared.communication.User.UserRole;
import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.presenter.LoginPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.KeyCodes;
import com.google.gwt.event.dom.client.KeyUpEvent;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.PasswordTextBox;
import com.google.gwt.user.client.ui.FocusPanel;
import com.google.gwt.user.client.ui.RadioButton;

public class LoginView extends Composite implements I_Display {

	private static LoginViewUiBinder uiBinder = GWT
			.create(LoginViewUiBinder.class);
	@UiField
	TextBox textUserEmail;
	@UiField
	Button buttonEnter;
	@UiField Hyperlink hyperlink;
	@UiField Button buttonClose;
	@UiField PasswordTextBox textUserPassword;
	@UiField FocusPanel focusPanel;
	@UiField RadioButton rbVisitor;
	@UiField RadioButton rbSuperAdmin;
	@UiField RadioButton rbSellingAgent;

	interface LoginViewUiBinder extends UiBinder<Widget, LoginView> {
	}

	private LoginPresenter presenter = null;

	public LoginView() {
		initWidget(uiBinder.createAndBindUi(this));
		rbVisitor.setValue(true, true);
	}

	@Override
	public void setPresenter(LoginPresenter presenter) {
		this.presenter = presenter;
	}

	@UiHandler("buttonEnter")
	void onButtonEnterClick(ClickEvent event) {
		presenter.onLogin();
	}

	@UiHandler("hyperlink")
	void onHyperlinkClick(ClickEvent event) {
		if (presenter != null)
			presenter.onForgotPassword();
	}

	@Override
	public String getUserEmail() {
		return textUserEmail.getValue();
	}

	@Override
	public String getUserPassword() {
		return textUserPassword.getValue();
	}
	@UiHandler("buttonClose")
	void onButtonCloseClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}
	@UiHandler("focusPanel")
	void onFocusPanelKeyUp(KeyUpEvent event) {
		if (event.getNativeKeyCode() == 13)
			presenter.onLogin();
	}
	
	private void changeRole() {
		if (rbVisitor.getValue()) {
			textUserEmail.setText("web");
			textUserPassword.setText("web");
		} else if (rbSuperAdmin.getValue()) {
			textUserEmail.setText("adminan");
			textUserPassword.setText("smelatoronto");
		} else if (rbSellingAgent.getValue()) {
			textUserEmail.setText("eugene.simonov@3dcondox.com");
			textUserPassword.setText("3dcondoms");
		}
	}

	@UiHandler("rbVisitor")
	void onRoleValueChange(ValueChangeEvent<Boolean> event) {
		changeRole();
	}
	
	@UiHandler("rbSuperAdmin")
	void onRbSuperAdminValueChange(ValueChangeEvent<Boolean> event) {
		changeRole();
	}
	@UiHandler("rbSellingAgent")
	void onRbSellingAgentValueChange(ValueChangeEvent<Boolean> event) {
		changeRole();
	}

	@Override
	public UserRole getUserRole() {
		if (rbVisitor.getValue())
			return UserRole.Visitor;
		if (rbSuperAdmin.getValue())
			return UserRole.SuperAdmin;
		if (rbSellingAgent.getValue())
			return UserRole.SellingAgent;
		return null;
	}
}
