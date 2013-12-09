package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.tree.presenter.HelloAgentPresenter;
import com.condox.ecommerce.client.tree.presenter.HelloAgentPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.cellview.client.DataGrid;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;

public class HelloAgentView extends Composite implements I_Display {

	private static HelloAgentViewUiBinder uiBinder = GWT
			.create(HelloAgentViewUiBinder.class);
	@UiField(provided=true) DataGrid<Object> dataGrid = new DataGrid<Object>();
	@UiField Hyperlink hyperlink;

	interface HelloAgentViewUiBinder extends UiBinder<Widget, HelloAgentView> {
	}

	private HelloAgentPresenter presenter = null;

	public HelloAgentView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(HelloAgentPresenter presenter) {
		this.presenter = presenter;
	}
	@UiHandler("hyperlink")
	void onHyperlinkClick(ClickEvent event) {
		if (presenter != null)
			presenter.onLogout();
	}
}
