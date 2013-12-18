package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.UpdateProfile2Presenter;
import com.condox.ecommerce.client.tree.presenter.UpdateProfile2Presenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.user.client.ui.Image;
import com.google.gwt.user.client.ui.FileUpload;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.shared.EventHandler;
import com.google.gwt.event.shared.GwtEvent;
import com.google.gwt.user.client.ui.FormPanel;
import com.google.gwt.user.client.ui.FormPanel.SubmitCompleteEvent;

public class UpdateProfile2View extends Composite implements I_Display {

	private static UpdateProfile2ViewUiBinder uiBinder = GWT
			.create(UpdateProfile2ViewUiBinder.class);
	@UiField Button buttonApply;
	@UiField Button buttonCancel;
	@UiField Button buttonFinish;
	@UiField Button buttonClose;
	@UiField Image image;

	interface UpdateProfile2ViewUiBinder extends UiBinder<Widget, UpdateProfile2View> {
	}

	private UpdateProfile2Presenter presenter = null;

	public UpdateProfile2View() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(UpdateProfile2Presenter presenter) {
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
	@UiHandler("buttonPrev")
	void onButtonPrevClick(ClickEvent event) {
		if (presenter != null)
			presenter.onPrev();
	}
	@UiHandler("buttonFinish")
	void onButtonFinishClick(ClickEvent event) {
		if (presenter != null)
			presenter.onFinish();
	}
	@UiHandler("image")
	void onImageClick(ClickEvent event) {
		if (presenter != null)
			presenter.onSelectAvatar();
	}
}
