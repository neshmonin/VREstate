//package com.condox.ecommerce.client.tree.view;
//
//import com.condox.ecommerce.client.tree.EcommerceTree;
//import com.condox.ecommerce.client.tree.presenter.EmailPresenter;
//import com.condox.ecommerce.client.tree.presenter.EmailPresenter.I_Display;
//import com.google.gwt.core.client.GWT;
//import com.google.gwt.event.dom.client.ClickEvent;
//import com.google.gwt.event.dom.client.KeyUpEvent;
//import com.google.gwt.event.logical.shared.ValueChangeEvent;
//import com.google.gwt.uibinder.client.UiBinder;
//import com.google.gwt.uibinder.client.UiField;
//import com.google.gwt.uibinder.client.UiHandler;
//import com.google.gwt.user.client.ui.Button;
//import com.google.gwt.user.client.ui.Composite;
//import com.google.gwt.user.client.ui.TextBox;
//import com.google.gwt.user.client.ui.Widget;
//
//public class EmailView extends Composite implements I_Display {
//
//	private static EmailViewUiBinder uiBinder = GWT
//			.create(EmailViewUiBinder.class);
//	@UiField Button buttonNext;
//	@UiField Button buttonPrev;
//	@UiField Button buttonCancel;
//	@UiField TextBox textMail;
//	
//	interface EmailViewUiBinder extends UiBinder<Widget, EmailView> {
//	}
//	
//	private EmailPresenter presenter = null;
//
//	public EmailView() {
//		initWidget(uiBinder.createAndBindUi(this));
//	}
//
//	@Override
//	public void setPresenter(EmailPresenter presenter) {
//		this.presenter = presenter;
//	}
//	@UiHandler("buttonNext")
//	void onButtonNextClick(ClickEvent event) {
////		Window.alert("{Sending data to server}");
////		Wizard.cancel();
//		presenter.setOwnerEmail(textMail.getText());
//		presenter.onNext();
//	}
//	@UiHandler("buttonPrev")
//	void onButtonPrevClick(ClickEvent event) {
//		presenter.onPrev();
//	}
//	@UiHandler("buttonCancel")
//	void onButtonCancelClick(ClickEvent event) {
//		EcommerceTree.cancel();
//	}
//	@UiHandler("textMail")
//	void onValueChange(ValueChangeEvent<String> event) {
//		buttonNext.setEnabled(textMail.getValue().matches("^[a-zA-Z0-9_.]+@[a-zA-Z0-9_]+?\\.[a-zA-Z]{2,3}$"));
//	}
//	@UiHandler("textMail")
//	void onTextFilterKeyUp(KeyUpEvent event) {
//		buttonNext.setEnabled(textMail.getValue().matches("^[a-zA-Z0-9_.]+@[a-zA-Z0-9_]+?\\.[a-zA-Z]{2,3}$"));
//	}
//
//	@Override
//	public void setOwnerEmail(String ownerEmail) {
//		// TODO Auto-generated method stub
//		textMail.setValue(ownerEmail);
//	}
//}
