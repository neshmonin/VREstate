package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.UsingMLSPresenter;
import com.condox.ecommerce.client.tree.presenter.UsingMLSPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.RadioButton;

public class UsingMLSView extends Composite implements I_Display {

	private static UsingMLSViewUiBinder uiBinder = GWT
			.create(UsingMLSViewUiBinder.class);
	@UiField Button buttonCancel;
	@UiField Button buttonNext;
	@UiField RadioButton rbMLS;

	interface UsingMLSViewUiBinder extends UiBinder<Widget, UsingMLSView> {
	}

	private UsingMLSPresenter presenter = null;

	public UsingMLSView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(UsingMLSPresenter presenter) {
		this.presenter = presenter;
	}
	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		if (presenter != null)
			presenter.onCancel();
	}
	@UiHandler("buttonNext")
	void onButtonNextClick(ClickEvent event) {
		if (presenter != null)
			presenter.onNext();
	}

	@Override
	public boolean isUsingMLS() {
		return rbMLS.getValue();
	}
}
