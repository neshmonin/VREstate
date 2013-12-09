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

public class ForgotPasswordView extends Composite implements I_Display {

	private static ForgotPasswordViewUiBinder uiBinder = GWT
			.create(ForgotPasswordViewUiBinder.class);

	interface ForgotPasswordViewUiBinder extends UiBinder<Widget, ForgotPasswordView> {
	}

	private ForgotPasswordPresenter presenter = null;
	private boolean user = false;
	private boolean guest = true;

	public ForgotPasswordView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(ForgotPasswordPresenter presenter) {
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
//	@UiHandler("buttonEnter")
//	void onButtonEnterClick(ClickEvent event) {
//		if (presenter != null)
//			presenter.onSubmit();
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
	@UiHandler("buttonEnter")
	void onButtonEnterClick(ClickEvent event) {
		if (presenter != null)
			presenter.onSubmit();
	}
}
