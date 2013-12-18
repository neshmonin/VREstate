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

public class ForgotPasswordView extends Composite implements I_Display {

	private static ForgotPasswordViewUiBinder uiBinder = GWT
			.create(ForgotPasswordViewUiBinder.class);
	@UiField TextBox textUserEmail;

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

	@UiHandler("buttonEnter")
	void onButtonEnterClick(ClickEvent event) {
		if (presenter != null)
			presenter.onSubmit();
	}

	@Override
	public String getEmail() {
		return textUserEmail.getValue();
	}

	@Override
	public void setEmail(String value) {
		textUserEmail.setValue(value);
	}
}
