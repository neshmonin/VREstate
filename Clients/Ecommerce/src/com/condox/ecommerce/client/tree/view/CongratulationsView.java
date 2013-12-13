package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.CongratulationsPresenter;
import com.condox.ecommerce.client.tree.presenter.CongratulationsPresenter.I_Display;
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
import com.google.gwt.user.client.ui.HTML;

public class CongratulationsView extends Composite implements I_Display {

	private static CongratulationsViewUiBinder uiBinder = GWT
			.create(CongratulationsViewUiBinder.class);
	@UiField Button buttonCancel;
	@UiField Button buttonPrev;
	@UiField Button buttonNext;

	interface CongratulationsViewUiBinder extends UiBinder<Widget, CongratulationsView> {
	}

	private CongratulationsPresenter presenter = null;

	public CongratulationsView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(CongratulationsPresenter presenter) {
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
	public void setData(String data) {
	}
}
