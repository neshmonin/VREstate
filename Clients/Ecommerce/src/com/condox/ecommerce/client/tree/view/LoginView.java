package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.presenter.LoginPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.event.dom.client.KeyCodes;
import com.google.gwt.event.dom.client.KeyUpEvent;
import com.google.gwt.event.shared.EventHandler;
import com.google.gwt.event.shared.GwtEvent;
import com.google.gwt.event.shared.GwtEvent.Type;
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
		// Styling
		enter.setStyleDependentName("navigation", true);
		close.setStyleDependentName("navigation", true);
	}

	@Override
	public void setPresenter(LoginPresenter presenter) {
		this.presenter = presenter;
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
			enter.fireEvent(new GwtEvent<ClickHandler>() {

				@Override
				public com.google.gwt.event.shared.GwtEvent.Type<ClickHandler> getAssociatedType() {
					return ClickEvent.getType();
				}

				@Override
				protected void dispatch(ClickHandler handler) {
					handler.onClick(null);
				}
			});
	}

	@Override
	public void beforeClose() {

	}

	@Override
	public void setLogin(String login) {
		this.login.setText(login);
	}

	@Override
	public HasClickHandlers getLoginButton() {
		return enter;
	}
}
