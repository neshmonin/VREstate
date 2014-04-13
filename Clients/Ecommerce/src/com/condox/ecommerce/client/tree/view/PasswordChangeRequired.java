package com.condox.ecommerce.client.tree.view;

import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Button;

public class PasswordChangeRequired extends Composite {

	private static PasswordChangeRequiredUiBinder uiBinder = GWT
			.create(PasswordChangeRequiredUiBinder.class);
	@UiField Button now;
	@UiField Button later;

	interface PasswordChangeRequiredUiBinder extends
			UiBinder<Widget, PasswordChangeRequired> {
	}

	public PasswordChangeRequired() {
		initWidget(uiBinder.createAndBindUi(this));
	}
	
	
	public HasClickHandlers getNow() {
		return now;
	}
	
	public HasClickHandlers getLater() {
		return later;
	}

}
