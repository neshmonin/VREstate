package com.condox.ecommerce.client.tree.view;

import com.condox.clientshared.abstractview.Log;
import com.condox.ecommerce.client.I_Presenter;
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
	@UiField TextBox firstName;
	@UiField TextBox lastName;
	@UiField TextBox email;
	@UiField TextBox phone;

	interface ProfileStep1ViewUiBinder extends UiBinder<Widget, ProfileStep1View> {
	}

	private ProfileStep1Presenter presenter = null;
	private UserInfo info = null;

	public ProfileStep1View() {
		initWidget(uiBinder.createAndBindUi(this));
		Timer updateView = new Timer() {

			@Override
			public void run() {
				String first = firstName.getValue();
				String mail = email.getValue();
				
				boolean empty = false;
				empty |= first.isEmpty();
				empty |= mail.isEmpty();
				
				boolean changed = false;
				changed |= !first.equals(info.getFirstName());
				changed |= !email.equals(info.getEmail());
				
				buttonApply.setEnabled(!empty && changed);
			}};
//		updateView.scheduleRepeating(500);
	}

	@Override
	public void setPresenter(ProfileStep1Presenter presenter) {
		this.presenter = presenter;
	}
	
	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		if (presenter != null)
			presenter.onCancel();
	}
	@UiHandler("buttonApply")
	void onButtonApplyClick(ClickEvent event) {
		if (presenter != null) {
			info.setNickName(firstName.getValue());
			info.setEmail(email.getValue());
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
		firstName.setValue(info.getPersonalInfo().getFirstName());
		lastName.setValue(info.getPersonalInfo().getLastName());
		email.setValue(info.getPersonalInfo().getEmail());
		phone.setValue(info.getPersonalInfo().getPhone());
		
	}

	@Override
	public UserInfo getUserInfo() {
		info.getPersonalInfo().setFirstName(firstName.getText());
		info.getPersonalInfo().setLastName(lastName.getText());
		info.getPersonalInfo().setEmail(email.getText());
		info.getPersonalInfo().setPhone(phone.getText());
		return info;
	}
	@UiHandler("firstName")
	void onFirstNameChange(ChangeEvent event) {
		buttonApply.setEnabled(true);
	}
	@UiHandler("lastName")
	void onLastNameChange(ChangeEvent event) {
		buttonApply.setEnabled(true);
	}
	@UiHandler("email")
	void onEmailChange(ChangeEvent event) {
		buttonApply.setEnabled(true);
	}
	@UiHandler("phone")
	void onPhoneChange(ChangeEvent event) {
		buttonApply.setEnabled(true);
	}
}
