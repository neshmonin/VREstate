package com.condox.vrestate.client.my2;

import com.condox.vrestate.client.my2.VREstatePresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasText;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.Label;

public class VREstateView extends Composite implements I_Display {

	private static VREstateViewUiBinder uiBinder = GWT
			.create(VREstateViewUiBinder.class);
	@UiField Label label;

	interface VREstateViewUiBinder extends UiBinder<Widget, VREstateView> {
	}

	public VREstateView() {
		initWidget(uiBinder.createAndBindUi(this));
	}
	
	private VREstatePresenter presenter = null;

	@Override
	public void setPresenter(VREstatePresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public void setUserInfo(String info) {
		label.setText(info);
	}
}
