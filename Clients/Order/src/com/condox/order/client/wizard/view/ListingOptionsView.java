package com.condox.order.client.wizard.view;

import com.condox.order.client.wizard.Wizard;
import com.condox.order.client.wizard.presenter.ListingOptionsPresenter;
import com.condox.order.client.wizard.presenter.ListingOptionsPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.KeyUpEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;

public class ListingOptionsView extends Composite implements I_Display {

	private static ListingOptionsViewUiBinder uiBinder = GWT
			.create(ListingOptionsViewUiBinder.class);
	@UiField
	Button buttonCancel;
	@UiField
	Button buttonPrev;
	@UiField
	Button buttonNext;
	@UiField
	TextBox textVirtualTourUrl;
	@UiField
	TextBox textMoreInfoUrl;
	@UiField
	Button buttonValidateVirtualTour;
	@UiField
	Button buttonValidateMoreInfo;

	interface ListingOptionsViewUiBinder extends
			UiBinder<Widget, ListingOptionsView> {
	}

	private ListingOptionsPresenter presenter = null;

	public ListingOptionsView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(ListingOptionsPresenter presenter) {
		this.presenter = presenter;
	}

	@UiHandler("buttonPrev")
	void onButtonPrevClick(ClickEvent event) {
		presenter.onPrev();
	}

	@UiHandler("buttonValidateVirtualTour")
	void onButtonValidateVirtualTourClick(ClickEvent event) {
		Window.open(textVirtualTourUrl.getText(), "Virtual Tour Testing", "");
	}

	@UiHandler("buttonNext")
	void onButtonNextClick(ClickEvent event) {
		presenter.onNext();
	}

	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		Wizard.cancel();
	}

	@UiHandler("textVirtualTourUrl")
	void onTextVirtualTourUrlKeyUp(KeyUpEvent event) {
		buttonValidateVirtualTour.setEnabled(!textVirtualTourUrl.getValue()
				.isEmpty());
	}
	@UiHandler("textMoreInfoUrl")
	void onTextMoreInfoUrlKeyUp(KeyUpEvent event) {
		buttonValidateMoreInfo.setEnabled(!textMoreInfoUrl.getValue()
				.isEmpty());
	}
	@UiHandler("buttonValidateMoreInfo")
	void onButtonValidateMoreInfoClick(ClickEvent event) {
		Window.open(textMoreInfoUrl.getText(), "More Info Testing", "");
	}
}
