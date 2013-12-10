//package com.condox.ecommerce.client.tree.view;
//
//import com.condox.ecommerce.client.tree.EcommerceTree;
//import com.condox.ecommerce.client.tree.presenter.ListingOptionsPresenter;
//import com.condox.ecommerce.client.tree.presenter.ListingOptionsPresenter.I_Display;
//import com.google.gwt.core.client.GWT;
//import com.google.gwt.event.dom.client.ClickEvent;
//import com.google.gwt.event.dom.client.KeyUpEvent;
//import com.google.gwt.uibinder.client.UiBinder;
//import com.google.gwt.uibinder.client.UiField;
//import com.google.gwt.uibinder.client.UiHandler;
//import com.google.gwt.user.client.Window;
//import com.google.gwt.user.client.ui.Button;
//import com.google.gwt.user.client.ui.Composite;
//import com.google.gwt.user.client.ui.TextBox;
//import com.google.gwt.user.client.ui.Widget;
//
//public class ListingOptionsView extends Composite implements I_Display {
//
//	private static ListingOptionsViewUiBinder uiBinder = GWT
//			.create(ListingOptionsViewUiBinder.class);
//	@UiField
//	Button buttonCancel;
//	@UiField
//	Button buttonPrev;
//	@UiField
//	Button buttonNext;
//	@UiField
//	TextBox textVirtualTourUrl;
//	@UiField
//	TextBox textMoreInfoUrl;
//	@UiField
//	Button buttonValidateVirtualTour;
//	@UiField
//	Button buttonValidateMoreInfo;
//	@UiField TextBox textMLS;
//
//	interface ListingOptionsViewUiBinder extends
//			UiBinder<Widget, ListingOptionsView> {
//	}
//
//	private ListingOptionsPresenter presenter = null;
//
//	public ListingOptionsView() {
//		initWidget(uiBinder.createAndBindUi(this));
//	}
//
//	@Override
//	public void setPresenter(ListingOptionsPresenter presenter) {
//		this.presenter = presenter;
//	}
//
//	@UiHandler("buttonPrev")
//	void onButtonPrevClick(ClickEvent event) {
//		presenter.onPrev();
//	}
//
//	@UiHandler("buttonValidateVirtualTour")
//	void onButtonValidateVirtualTourClick(ClickEvent event) {
//		Window.open(textVirtualTourUrl.getText(), "Virtual Tour Testing", "");
//	}
//
//	@UiHandler("buttonNext")
//	void onButtonNextClick(ClickEvent event) {
//		presenter.onNext();
//	}
//
//	@UiHandler("buttonCancel")
//	void onButtonCancelClick(ClickEvent event) {
//		EcommerceTree.cancel();
//	}
//
//	@UiHandler("textVirtualTourUrl")
//	void onTextVirtualTourUrlKeyUp(KeyUpEvent event) {
//		buttonValidateVirtualTour.setEnabled(!textVirtualTourUrl.getValue()
//				.isEmpty());
//	}
//	@UiHandler("textMoreInfoUrl")
//	void onTextMoreInfoUrlKeyUp(KeyUpEvent event) {
//		buttonValidateMoreInfo.setEnabled(!textMoreInfoUrl.getValue()
//				.isEmpty());
//	}
//	@UiHandler("buttonValidateMoreInfo")
//	void onButtonValidateMoreInfoClick(ClickEvent event) {
//		Window.open(textMoreInfoUrl.getText(), "More Info Testing", "");
//	}
//	@UiHandler("textMLS")
//	void onTextMLSKeyUp(KeyUpEvent event) {
//		String mls = textMLS.getValue();
//		buttonNext.setEnabled(mls.matches("[a-zA-Z][0-9]{7}"));
//	}
//
//	@Override
//	public String getMls() {
//		return textMLS.getValue();
//	}
//
//	@Override
//	public String getVirtualTourURL() {
//		return textVirtualTourUrl.getValue();
//	}
//
//	@Override
//	public String getMoreInfoURL() {
//		return textMoreInfoUrl.getValue();
//	}
//
//
//	@Override
//	public void setVirtualTourURL(String value) {
//		textVirtualTourUrl.setValue(value);
//		buttonValidateVirtualTour.setEnabled(!value.isEmpty());
//	}
//
//	@Override
//	public void setMoreInfoURL(String value) {
//		textMoreInfoUrl.setValue(value);
//		buttonValidateMoreInfo.setEnabled(!value.isEmpty());
//	}
//
//	@Override
//	public void setMLS(String value) {
//		textMLS.setValue(value);
//		textMLS.setEnabled(value.isEmpty());
//		buttonNext.setEnabled(value.matches("[a-zA-Z][0-9]{7}"));
//	}
//}
