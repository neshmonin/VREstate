package com.condox.orders.client.pages.submit.successfull;

import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.History;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;

public class SubmitSuccessfull extends Composite {

	private static SubmitSuccessfullUiBinder uiBinder = GWT
			.create(SubmitSuccessfullUiBinder.class);
	@UiField HTML txtMessage;
	@UiField Button button;

	interface SubmitSuccessfullUiBinder extends
			UiBinder<Widget, SubmitSuccessfull> {
	}

	public SubmitSuccessfull() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	public SubmitSuccessfull(String firstName) {
		initWidget(uiBinder.createAndBindUi(this));
	}

	public SubmitSuccessfull(DialogBox box) {
		this.box = box;
		initWidget(uiBinder.createAndBindUi(this));
	}
	
	private DialogBox box = null;
	
	public void setCustomerEmail(String email) {
		txtMessage.setHTML("Your request submitted successfully."/* +
				"<BR>A confirmation email has been sent to " + email*/);
	}
	@UiHandler("button")
	void onButtonClick(ClickEvent event) {
		box.hide();
		History.newItem("buildings");
	}
}
