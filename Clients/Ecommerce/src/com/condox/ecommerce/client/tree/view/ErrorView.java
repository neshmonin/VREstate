package com.condox.ecommerce.client.tree.view;

import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class ErrorView extends Composite {

	private static ErrorViewUiBinder uiBinder = GWT
			.create(ErrorViewUiBinder.class);

	interface ErrorViewUiBinder extends UiBinder<Widget, ErrorView> {
	}

	public ErrorView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

}
