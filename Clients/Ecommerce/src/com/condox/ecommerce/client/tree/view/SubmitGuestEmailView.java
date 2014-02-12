package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.SubmitGuestEmailPresenter;
import com.condox.ecommerce.client.tree.presenter.SubmitGuestEmailPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.KeyCodes;
import com.google.gwt.event.dom.client.KeyUpEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.FocusPanel;

public class SubmitGuestEmailView extends Composite implements I_Display {

	private static SubmitGuestEmailViewUiBinder uiBinder = GWT
			.create(SubmitGuestEmailViewUiBinder.class);
	@UiField
	TextBox login;
	@UiField
	Button close;
	@UiField
	Button submit;
	@UiField
	FocusPanel focusPanel;

	interface SubmitGuestEmailViewUiBinder extends
			UiBinder<Widget, SubmitGuestEmailView> {
	}

	private SubmitGuestEmailPresenter presenter = null;

	public SubmitGuestEmailView() {
		initWidget(uiBinder.createAndBindUi(this));
		login.setFocus(true);
	}

	@Override
	public void setPresenter(SubmitGuestEmailPresenter presenter) {
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
		if (presenter != null) {
			close.setEnabled(false);
			submit.setText("Submitted");
			submit.setEnabled(false);
			presenter.onSubmit();
		}
	}

	@UiHandler("focusPanel")
	void onFocusPanelKeyUp(KeyUpEvent event) {
		if (event.getNativeKeyCode() == KeyCodes.KEY_ENTER) {
			close.setEnabled(false);
			submit.setText("Submitted");
			submit.setEnabled(false);
			presenter.onSubmit();
		}
	}
}
