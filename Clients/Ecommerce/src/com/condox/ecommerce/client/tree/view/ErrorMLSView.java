package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.ErrorMLSPresenter;
import com.condox.ecommerce.client.tree.presenter.ErrorMLSPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.DeckPanel;

public class ErrorMLSView extends Composite implements I_Display {

	private static ErrorMLSViewUiBinder uiBinder = GWT
			.create(ErrorMLSViewUiBinder.class);

	interface ErrorMLSViewUiBinder extends UiBinder<Widget, ErrorMLSView> {
	}

	private ErrorMLSPresenter presenter = null;

	public ErrorMLSView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@UiHandler("buttonEnter")
	void onButtonEnterClick(ClickEvent event) {
		if (presenter != null)
			presenter.onBack();
	}

	@Override
	public void setPresenter(ErrorMLSPresenter presenter) {
		this.presenter = presenter;
	}

}
