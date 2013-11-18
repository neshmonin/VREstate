package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.MLSPresenter;
import com.condox.ecommerce.client.tree.presenter.MLSPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Element;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;

public class MLSView extends Composite implements I_Display {

	private static MLSViewUiBinder uiBinder = GWT.create(MLSViewUiBinder.class);
	private MLSPresenter presenter = null;

	@UiField
	RadioButton rbMLS;
	@UiField
	RadioButton rbNoMLS;
	@UiField
	Button buttonCancel;
	@UiField
	Button buttonPrev;
	@UiField
	Button buttonNext;
	@UiField
	TextBox textMLS;

	interface MLSViewUiBinder extends UiBinder<Widget, MLSView> {
	}

	public MLSView() {
		initWidget(uiBinder.createAndBindUi(this));
		setMLSChangeHandler(textMLS.getElement());
		textMLS.setFocus(true);
	}

	@UiHandler("buttonPrev")
	void onButtonPrevClick(ClickEvent event) {
		presenter.onPrev();
	}

	@UiHandler("buttonNext")
	void onButtonNextClick(ClickEvent event) {
		presenter.onNext();
	}

	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		EcommerceTree.cancel();
	}

	@Override
	public void setPresenter(MLSPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public String getMLS() {
		if (rbMLS.getValue())
			return textMLS.getValue();
		return "";
	}

	@Override
	public void setMLS(String value) {
		textMLS.setValue(value);
		render();
	}

	@UiHandler("rbMLS")
	void onRbMLSValueChange(ValueChangeEvent<Boolean> event) {
		render();
		textMLS.setFocus(true);
	}

	@UiHandler("rbNoMLS")
	void onRbNoMLSValueChange(ValueChangeEvent<Boolean> event) {
		render();
	}

	private native void setMLSChangeHandler(Element element)/*-{
		var instance = this;
		function render() {
			instance.@com.condox.ecommerce.client.tree.view.MLSView::render()();
		}
		element.oninput = render;
		element.onpaste = render;
		setInterval(render, 100);
	}-*/;

	private void render() {
		if (rbMLS.getValue()) {
			textMLS.setEnabled(true);
			buttonNext.setEnabled(textMLS.getValue()
					.matches("[a-zA-Z][0-9]{7}"));
		}
		if (rbNoMLS.getValue()) {
			textMLS.setValue("");
			textMLS.setEnabled(false);
			buttonNext.setEnabled(true);
		}
	}
}
