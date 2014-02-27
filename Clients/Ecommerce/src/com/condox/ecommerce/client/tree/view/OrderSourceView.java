package com.condox.ecommerce.client.tree.view;

import java.util.List;

import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.Tree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.presenter.OrderSourcePresenter;
import com.condox.ecommerce.client.tree.presenter.OrderSourcePresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.MultiWordSuggestOracle;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.SuggestBox;
import com.google.gwt.user.client.ui.Widget;

public class OrderSourceView extends Composite implements I_Display {

	private static UsingMLSViewUiBinder uiBinder = GWT
			.create(UsingMLSViewUiBinder.class);
	@UiField
	Button buttonCancel;
	@UiField
	Button buttonNext;
	@UiField
	RadioButton rbMLS;
	@UiField
	Button buttonPrev;
	@UiField
	RadioButton rbAddress;
	@UiField(provided = true)
	SuggestBox sbMLS;
	@UiField
	Label label;

	interface UsingMLSViewUiBinder extends UiBinder<Widget, OrderSourceView> {
	}

	private OrderSourcePresenter presenter = null;
	private MultiWordSuggestOracle oracle = new MultiWordSuggestOracle();

	public OrderSourceView() {
		sbMLS = new SuggestBox(oracle);
		initWidget(uiBinder.createAndBindUi(this));
		sbMLS.setText("");

		Timer validation = new Timer() {

			@Override
			public void run() {
				if (rbMLS.getValue()) {
					sbMLS.setEnabled(true);
				} else {
					sbMLS.setEnabled(false);
					sbMLS.setValue("");
				}

				boolean valid = true;
				valid &= (rbAddress.getValue() || !sbMLS.getValue().isEmpty());
				valid &= (sbMLS.getValue().isEmpty() || sbMLS.getValue()
						.matches("[A-Z]{1,1}[0-9]{7,7}"));
				buttonNext.setEnabled(valid);
			}
		};
		validation.scheduleRepeating(100);
		// --------------------------------
	}

	@Override
	public void setPresenter(OrderSourcePresenter presenter) {
		this.presenter = presenter;
	}

	@UiHandler("buttonCancel")
	void onButtonCancelClick(ClickEvent event) {
		if (presenter != null)
			presenter.onCancel();
	}

	@UiHandler("buttonNext")
	void onButtonNextClick(ClickEvent event) {
		if (presenter != null)
			presenter.onNext();
	}

	@Override
	public boolean isUsingMLS() {
		return rbMLS.getValue();
	}

	@UiHandler("buttonPrev")
	void onButtonPrevClick(ClickEvent event) {
		if (presenter != null)
			presenter.onPrev();
	}

	@Override
	public void setUsingMLS(boolean value) {
		// TODO Auto-generated method stub
		rbMLS.setValue(value);
		rbAddress.setValue(!value);
	}

	@Override
	public String getMLS() {
		return sbMLS.getValue();
	}

	@Override
	public void setMLS(String value) {
		sbMLS.setValue(value);
	}

	@Override
	public void setMLSSuggestions(List<String> value) {
		oracle.clear();
		// oracle.addAll(value);
	}

	@Override
	public void setUserRole(UserRole role) {
		switch (role) {
		case Visitor:
			label.setText("Please either specify the MLS# for your listing, or, if you do not remember it, provide the address");
			rbMLS.setText("Use MLS#");
			rbAddress
					.setText("Do not remember the MLS# - let's use the address");
			break;
		case Agent:
			label.setText("You can create either a copy of an MLS listing, or make a completely new unique Interactive 3D Listing");
			rbMLS.setText("Create a copy of MLS listing");
			rbAddress.setText("Create a unique Interactive 3D Listing");
			break;
		default:
			label.setText("Please either specify the MLS# for your listing, or, if you do not remember it, provide the address");
			rbMLS.setText("Use MLS#");
			rbAddress
					.setText("Do not remember the MLS# - let's use the address");
			break;

		}
	}
}
