package com.condox.ecommerce.client.tree.view;

import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Button;

public class ProgressView extends Composite {

	private static ProgressViewUiBinder uiBinder = GWT
			.create(ProgressViewUiBinder.class);
	@UiField Button progressBar;

	interface ProgressViewUiBinder extends UiBinder<Widget, ProgressView> {
	}

	public ProgressView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

}
