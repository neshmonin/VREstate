package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.ShowHistoryPresenter;
import com.condox.ecommerce.client.tree.presenter.ShowHistoryPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;

public class ShowHistoryView extends Composite implements I_Display {

	private static ShowHistoryViewUiBinder uiBinder = GWT
			.create(ShowHistoryViewUiBinder.class);
	@UiField Hyperlink linkClose;
	@UiField HTML htmlHistory;

	interface ShowHistoryViewUiBinder extends UiBinder<Widget, ShowHistoryView> {
	}

	private ShowHistoryPresenter presenter = null;

	public ShowHistoryView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(ShowHistoryPresenter presenter) {
		this.presenter = presenter;
	}
	@UiHandler("linkClose")
	void onLinkCloseClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}

	@Override
	public void setHistoryData(String html) {
		htmlHistory.setHTML(html);
	}
}
