package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.AgreementPresenter;
import com.condox.ecommerce.client.tree.presenter.UpdateProfile1Presenter;
import com.condox.ecommerce.client.tree.presenter.UpdateProfile1Presenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.user.client.ui.TextBox;

public class UpdateProfile1View extends Composite implements I_Display {

	private static UpdateProfile1ViewUiBinder uiBinder = GWT
			.create(UpdateProfile1ViewUiBinder.class);
	@UiField Button buttonApply;
	@UiField Button buttonCancel;
	@UiField Button buttonNext;
	@UiField Button buttonFinish;
	@UiField Button buttonClose;
	@UiField TextBox textFirstName;
	@UiField TextBox textLastName;
	@UiField TextBox textEmail;
	@UiField TextBox textPhone;

	interface UpdateProfile1ViewUiBinder extends UiBinder<Widget, UpdateProfile1View> {
	}

	private UpdateProfile1Presenter presenter = null;

	public UpdateProfile1View() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(UpdateProfile1Presenter presenter) {
		this.presenter = presenter;
	}
	@UiHandler("buttonClose")
	void onButtonCloseClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}
	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		if (presenter != null)
			presenter.onCancel();
	}
	@UiHandler("buttonApply")
	void onButtonApplyClick(ClickEvent event) {
		if (presenter != null)
			presenter.onApply();
	}
	@UiHandler("buttonFinish")
	void onButtonFinishClick(ClickEvent event) {
		if (presenter != null)
			presenter.onFinish();
	}
	@UiHandler("buttonNext")
	void onButtonNextClick(ClickEvent event) {
		if (presenter != null)
			presenter.onNext();
	}
}
