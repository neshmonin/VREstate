package com.condox.orders.client.page;

import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HasText;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.TextBox;

public class LoginPanel extends Composite {

	private static LoginPanelUiBinder uiBinder = GWT
			.create(LoginPanelUiBinder.class);
	@UiField TextBox textPassword;
	@UiField Button button;

	interface LoginPanelUiBinder extends UiBinder<Widget, LoginPanel> {
	}

	public LoginPanel() {
		initWidget(uiBinder.createAndBindUi(this));
		box.add(this);
		box.setGlassEnabled(true);
		box.setText("Please, login:");
	}
	
	private DialogBox box = new DialogBox();

//	public LoginPanel(String firstName) {
//		initWidget(uiBinder.createAndBindUi(this));
//		box.add(this);
//		box.setGlassEnabled(true);
//	}

	public void Show() {
		box.center();
		box.show();
	}
	
	

	@UiHandler("button")
	void onButtonClick(ClickEvent event) {
		box.hide();
	}
}
