package com.condox.vrestate.client.tree;

import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class LoginView extends Composite implements LoginPresenter.I_Display {

	private static LoginViewUiBinder uiBinder = GWT
			.create(LoginViewUiBinder.class);
	@UiField Button button;

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
	@UiHandler("button")
	void onButtonClick(ClickEvent event) {
		presenter.onGuestLogin();
	}
}
