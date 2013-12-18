//package com.condox.ecommerce.client.tree.view;
//
//import com.condox.ecommerce.client.tree.EcommerceTree;
//import com.condox.ecommerce.client.tree.presenter.ProductPresenter;
//import com.condox.ecommerce.client.tree.presenter.ProductPresenter.I_Display;
//import com.google.gwt.core.client.GWT;
//import com.google.gwt.event.dom.client.ClickEvent;
//import com.google.gwt.event.logical.shared.ValueChangeEvent;
//import com.google.gwt.uibinder.client.UiBinder;
//import com.google.gwt.uibinder.client.UiField;
//import com.google.gwt.uibinder.client.UiHandler;
//import com.google.gwt.user.client.ui.Button;
//import com.google.gwt.user.client.ui.Composite;
//import com.google.gwt.user.client.ui.RadioButton;
//import com.google.gwt.user.client.ui.Widget;
//
//public class ProductView extends Composite implements I_Display {
//
//	private static ProductViewUiBinder uiBinder = GWT
//			.create(ProductViewUiBinder.class);
//	private ProductPresenter presenter = null;
//
//	@UiField
//	RadioButton rbListing;
//	@UiField
//	RadioButton rbLayout;
//	@UiField
//	RadioButton rbListingPrivate;
//	@UiField
//	RadioButton rbListingShared;
//	@UiField Button buttonCancel;
//	@UiField Button buttonPrev;
//	@UiField Button buttonNext;
//
//	interface ProductViewUiBinder extends UiBinder<Widget, ProductView> {
//	}
//
//	public ProductView() {
//		initWidget(uiBinder.createAndBindUi(this));
//	}
//
//	@UiHandler("rbListing")
//	void onRbListingValueChange(ValueChangeEvent<Boolean> event) {
//		rbListingPrivate.setEnabled(event.getValue());
//		rbListingShared.setEnabled(event.getValue());
//	}
//
//	@UiHandler("rbLayout")
//	void onRbLayoutValueChange(ValueChangeEvent<Boolean> event) {
//		rbListingPrivate.setEnabled(!event.getValue());
//		rbListingShared.setEnabled(!event.getValue());
//	}
//
//	@Override
//	public void setPresenter(ProductPresenter presenter) {
//		this.presenter = presenter;
//	}
//
//	@UiHandler("buttonPrev")
//	void onButtonPrevClick(ClickEvent event) {
//		presenter.onPrev();
//	}
//	@UiHandler("buttonNext")
//	void onButtonNextClick(ClickEvent event) {
//		presenter.onNext();
//	}
//
//	@Override
//	public boolean getListing() {
//		return rbListing.getValue();
//	}
//
//	@Override
//	public boolean getPrivate() {
//		return rbListingPrivate.getValue();
//	}
//
//	@Override
//	public boolean getShared() {
//		return rbListingShared.getValue();
//	}
//
//	@Override
//	public boolean getLayout() {
//		return rbLayout.getValue();
//	}
//
//	@Override
//	public void setListing(boolean value) {
//		rbListing.setValue(value,true);
//	}
//
//	@Override
//	public void setPrivate(boolean value) {
//		rbListingPrivate.setValue(value);
//	}
//
//	@Override
//	public void setShared(boolean value) {
//		rbListingShared.setValue(value);
//	}
//
//	@Override
//	public void setLayout(boolean value) {
//		rbLayout.setValue(value,true);
//	}
//	@UiHandler("buttonCancel")
//	void onButtonCancelClick(ClickEvent event) {
//		EcommerceTree.cancel();
//	}
//}
