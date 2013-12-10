//package com.condox.ecommerce.client.tree.view;
//
//import com.condox.ecommerce.client.tree.EcommerceTree;
//import com.condox.ecommerce.client.tree.presenter.SummaryPresenter;
//import com.condox.ecommerce.client.tree.presenter.SummaryPresenter.I_Display;
//import com.google.gwt.core.client.GWT;
//import com.google.gwt.event.dom.client.ClickEvent;
//import com.google.gwt.uibinder.client.UiBinder;
//import com.google.gwt.uibinder.client.UiField;
//import com.google.gwt.uibinder.client.UiHandler;
//import com.google.gwt.user.client.ui.Button;
//import com.google.gwt.user.client.ui.Composite;
//import com.google.gwt.user.client.ui.HTML;
//import com.google.gwt.user.client.ui.Widget;
//
//public class SummaryView extends Composite implements I_Display {
//
//	private static SummaryViewUiBinder uiBinder = GWT
//			.create(SummaryViewUiBinder.class);
//	@UiField Button buttonNext;
//	@UiField Button buttonPrev;
//	@UiField Button buttonCancel;
//	@UiField HTML html;
//	
//	interface SummaryViewUiBinder extends UiBinder<Widget, SummaryView> {
//	}
//	
//	private SummaryPresenter presenter = null;
//
//	public SummaryView() {
//		initWidget(uiBinder.createAndBindUi(this));
//		
//	}
//
//	@Override
//	public void setPresenter(SummaryPresenter presenter) {
//		this.presenter = presenter;
//		//************************
//		html.setHTML(presenter.getSummary());
//		//************************
//	}
//	@UiHandler("buttonNext")
//	void onButtonNextClick(ClickEvent event) {
//		presenter.onNext();
////		Window.alert("{Sending data to server}");
////		Wizard.cancel();
//	}
//	@UiHandler("buttonPrev")
//	void onButtonPrevClick(ClickEvent event) {
//		presenter.onPrev();
//	}
//	@UiHandler("buttonCancel")
//	void onButtonCancelClick(ClickEvent event) {
//		EcommerceTree.cancel();
//	}
//}
