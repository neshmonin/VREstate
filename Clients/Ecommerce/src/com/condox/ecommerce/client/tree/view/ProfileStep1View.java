package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.presenter.ProfileStep1Presenter;
import com.condox.ecommerce.client.tree.presenter.ProfileStep1Presenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.event.dom.client.ChangeEvent;

public class ProfileStep1View extends Composite implements I_Display {

	private static ProfileStep1ViewUiBinder uiBinder = GWT
			.create(ProfileStep1ViewUiBinder.class);
	@UiField Button buttonApply;
	@UiField Button buttonCancel;
//	@UiField Button buttonNext;
	@UiField Button buttonFinish;
	@UiField Button buttonClose;
	@UiField TextBox textFirstName;
	@UiField TextBox textLastName;
	@UiField TextBox textEmail;
	@UiField TextBox textPhone;

	interface ProfileStep1ViewUiBinder extends UiBinder<Widget, ProfileStep1View> {
	}

	private ProfileStep1Presenter presenter = null;
	private UserInfo info = null;

	public ProfileStep1View() {
		initWidget(uiBinder.createAndBindUi(this));
		Timer updateView = new Timer() {

			@Override
			public void run() {
				String firstName = textFirstName.getValue();
				String email = textEmail.getValue();
				
				boolean empty = false;
				empty |= firstName.isEmpty();
				empty |= email.isEmpty();
				
				boolean changed = false;
				changed |= !firstName.equals(info.getNickName());
				changed |= !email.equals(info.getEmail());
				
				buttonApply.setEnabled(!empty && changed);
			}};
//		updateView.scheduleRepeating(500);
	}

	@Override
	public void setPresenter(ProfileStep1Presenter presenter) {
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
		if (presenter != null) {
			info.setNickName(textFirstName.getValue());
			info.setEmail(textEmail.getValue());
			presenter.onApply();
		}
		buttonApply.setEnabled(false);
	}
	@UiHandler("buttonFinish")
	void onButtonFinishClick(ClickEvent event) {
		if (presenter != null)
			presenter.onFinish();
	}
//	@UiHandler("buttonNext")
//	void onButtonNextClick(ClickEvent event) {
//		if (presenter != null)
//			presenter.onNext();
//	}

	@Override
	public void setUserInfo(UserInfo newInfo) {
		info = newInfo;
		textFirstName.setValue(info.getNickName());
		textEmail.setValue(info.getEmail());
	}

	@Override
	public UserInfo getUserInfo() {
		return info;
	}
	@UiHandler("textFirstName")
	void onTextFirstNameChange(ChangeEvent event) {
		buttonApply.setEnabled(true);
	}
	@UiHandler("textLastName")
	void onTextLastNameChange(ChangeEvent event) {
		buttonApply.setEnabled(true);
	}
	@UiHandler("textEmail")
	void onTextEmailChange(ChangeEvent event) {
		buttonApply.setEnabled(true);
	}
	@UiHandler("textPhone")
	void onTextPhoneChange(ChangeEvent event) {
		buttonApply.setEnabled(true);
	}
}
