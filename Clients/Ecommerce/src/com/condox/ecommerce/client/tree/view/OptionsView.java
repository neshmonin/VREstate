package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.OptionsPresenter;
import com.condox.ecommerce.client.tree.presenter.OptionsPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.user.client.ui.TextBox;

public class OptionsView extends Composite implements I_Display {

	private static OptionsViewUiBinder uiBinder = GWT
			.create(OptionsViewUiBinder.class);
	@UiField RadioButton rbForSale;
	@UiField RadioButton rbForRent;
	@UiField Label strMLS;
	@UiField Button buttonCancel;
	@UiField Button buttonPrev;
	@UiField Button buttonNext;
	@UiField TextBox textPrice;
	@UiField TextBox textVirtualTourUrl;
	@UiField TextBox textMoreInfoUrl;

	interface OptionsViewUiBinder extends UiBinder<Widget, OptionsView> {
	}

	private OptionsPresenter presenter = null;

	public OptionsView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(OptionsPresenter presenter) {
		this.presenter = presenter;
	}

	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		if (presenter != null)
			presenter.onCancel();
	}
	@UiHandler("buttonPrev")
	void onButtonPrevClick(ClickEvent event) {
		if (presenter != null)
			presenter.onPrev();
	}
	@UiHandler("buttonNext")
	void onButtonNextClick(ClickEvent event) {
		if (presenter != null)
			presenter.onNext();
	}

	@Override
	public String getMLS() {
		return strMLS.getText();
	}

	@Override
	public String getPrice() {
		return textPrice.getValue();
	}

	@Override
	public String getVirtualTourUrl() {
		return textMoreInfoUrl.getValue();
	}

	@Override
	public String getMoreInfoUrl() {
		return textMoreInfoUrl.getValue();
	}
	
}
