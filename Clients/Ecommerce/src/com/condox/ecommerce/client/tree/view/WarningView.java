package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.WarningPresenter;
import com.condox.ecommerce.client.tree.presenter.WarningPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.HTML;

public class WarningView extends Composite implements I_Display {

	private static WarningViewUiBinder uiBinder = GWT
			.create(WarningViewUiBinder.class);
	@UiField Button ok;
	@UiField HTML warning;

	interface WarningViewUiBinder extends UiBinder<Widget, WarningView> {
	}
	
	private WarningPresenter presenter;

	public WarningView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public HasClickHandlers getOK() {
		return ok;
	}

	@Override
	public void setPresenter(WarningPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public void setMessage(String value) {
		warning.setHTML(value);
	}

}
