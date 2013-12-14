package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.NewOrderPresenter;
import com.condox.ecommerce.client.tree.presenter.NewOrderPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;

public class NewOrderView extends Composite implements I_Display {

	private static NewOrderViewUiBinder uiBinder = GWT
			.create(NewOrderViewUiBinder.class);
	@UiField Button buttonCancel;
	@UiField Button buttonNext;

	interface NewOrderViewUiBinder extends UiBinder<Widget, NewOrderView> {
	}

	private NewOrderPresenter presenter = null;

	public NewOrderView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(NewOrderPresenter presenter) {
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
}
