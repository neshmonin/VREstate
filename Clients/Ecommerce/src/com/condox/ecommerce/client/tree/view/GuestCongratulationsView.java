package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.GuestCongratulationsPresenter;
import com.condox.ecommerce.client.tree.presenter.GuestCongratulationsPresenter.I_Display;
import com.condox.ecommerce.client.tree.presenter.SubmitGuestEmailPresenter;
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
import com.google.gwt.user.client.ui.HTMLPanel;

public class GuestCongratulationsView extends Composite implements I_Display {

	private static GuestCongratulationsViewUiBinder uiBinder = GWT
			.create(GuestCongratulationsViewUiBinder.class);
	@UiField Button ok;
	@UiField HTML email;

	interface GuestCongratulationsViewUiBinder extends UiBinder<Widget, GuestCongratulationsView> {
	}

	private GuestCongratulationsPresenter presenter = null;

	public GuestCongratulationsView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(GuestCongratulationsPresenter presenter) {
		this.presenter = presenter;
		email.setHTML("<center><b>" + SubmitGuestEmailPresenter.guestEmail + "</b></center>");
	}

	@Override
	public void setData(String data) {
	}
	
	@UiHandler("ok")
	void onOkClick(ClickEvent event) {
		if (presenter != null)
			presenter.onOK();
	}
}
