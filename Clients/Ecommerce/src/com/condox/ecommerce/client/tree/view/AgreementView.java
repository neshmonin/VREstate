package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.AgreementPresenter;
import com.condox.ecommerce.client.tree.presenter.AgreementPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Button;

public class AgreementView extends Composite implements I_Display {

	private static AgreementViewUiBinder uiBinder = GWT
			.create(AgreementViewUiBinder.class);
	@UiField Button buttonProceed;
	@UiField Button buttonAgree;
	@UiField Button buttonCancel;
	@UiField Button buttonPrev;
	interface AgreementViewUiBinder extends UiBinder<Widget, AgreementView> {
	}

	private AgreementPresenter presenter = null;

	public AgreementView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(AgreementPresenter presenter) {
		this.presenter = presenter;
	}
	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		if (presenter != null)
			presenter.onCancel();
	}
	@UiHandler("buttonAgree")
	void onButtonAgreeClick(ClickEvent event) {
		buttonProceed.setEnabled(true);
		buttonAgree.setEnabled(false);
	}
	@UiHandler("buttonPrev")
	void onButtonPrevClick(ClickEvent event) {
		if (presenter != null)
			presenter.onPrev();
	}
	@UiHandler("buttonProceed")
	void onButtonProceedClick(ClickEvent event) {
		buttonPrev.setVisible(false);
		buttonProceed.setEnabled(false);
		buttonCancel.setText("Close");
		// Change Cancel to Close
		if (presenter != null)
			presenter.onProceed();
	}
}
