package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.ForgotPasswordPresenter;
import com.condox.ecommerce.client.tree.presenter.ForgotPasswordPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.KeyUpEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Button;

public class ForgotPasswordView extends Composite implements I_Display {

	private static ForgotPasswordViewUiBinder uiBinder = GWT
			.create(ForgotPasswordViewUiBinder.class);
	@UiField TextBox login;
	@UiField Button close;
	@UiField Button submit;

	interface ForgotPasswordViewUiBinder extends UiBinder<Widget, ForgotPasswordView> {
	}

	private ForgotPasswordPresenter presenter = null;

	public ForgotPasswordView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(ForgotPasswordPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public String getLogin() {
		return login.getValue();
	}

	@Override
	public void setLogin(String value) {
		login.setValue(value);
	}
	@UiHandler("close")
	void onCloseClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}
	@UiHandler("submit")
	void onSubmitClick(ClickEvent event) {
		if (presenter != null)
			presenter.onSubmit();
	}
}
