package com.condox.order.client.wizard.view;

import com.condox.order.client.wizard.Wizard;
import com.condox.order.client.wizard.presenter.ListingOptionsPresenter;
import com.condox.order.client.wizard.presenter.ListingOptionsPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.event.logical.shared.ValueChangeEvent;

public class ListingOptionsView extends Composite implements I_Display {

	private static ListingOptionsViewUiBinder uiBinder = GWT
			.create(ListingOptionsViewUiBinder.class);
	@UiField Button buttonCancel;
	@UiField Button buttonPrev;
	@UiField Button buttonNext;
	@UiField TextBox textVirtualTourUrl;
	@UiField TextBox textMoreInfoUrl;
	@UiField CheckBox checkBox;
	@UiField Button button;
	
	interface ListingOptionsViewUiBinder extends UiBinder<Widget, ListingOptionsView> {
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
	@UiHandler("checkBox")
	void onCheckBoxValueChange(ValueChangeEvent<Boolean> event) {
		if (event.getValue())
			textVirtualTourUrl.setText("http://your.old.link");
		textVirtualTourUrl.setEnabled(!event.getValue());
	}
	@UiHandler("button")
	void onButtonClick(ClickEvent event) {
		Window.open(textVirtualTourUrl.getText(), "Virtual Tour Testing","");
	}
	@UiHandler("buttonNext")
	void onButtonNextClick(ClickEvent event) {
		presenter.onNext();
	}
	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		Wizard.cancel();
	}
}
