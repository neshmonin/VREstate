package com.condox.ecommerce.client.tree.view;

import com.condox.clientshared.communication.User.UserRole;
import com.condox.ecommerce.client.tree.presenter.SummaryPresenter;
import com.condox.ecommerce.client.tree.presenter.SummaryPresenter.I_Display;
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
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.cellview.client.CellTable;

public class SummaryView extends Composite implements I_Display {

	private static SummaryViewUiBinder uiBinder = GWT
			.create(SummaryViewUiBinder.class);
	@UiField Button buttonCancel;
	@UiField Button buttonPrev;
	@UiField Button buttonNext;
	@UiField HTML htmlSummary;
	@UiField HTML html;

	interface SummaryViewUiBinder extends UiBinder<Widget, SummaryView> {
	}

	private SummaryPresenter presenter = null;

	public SummaryView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(SummaryPresenter presenter) {
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
		htmlSummary.setHTML(data);
	}

	@Override
	public void setUserRole(UserRole role) {
		if (!UserRole.Visitor.equals(role))
			html.setText("");
	}
}
