package com.condox.ecommerce.client.tree.view;

import com.condox.clientshared.container.I_Contained;
import com.condox.ecommerce.client.tree.presenter.I_OrderDetailsPresenter;
import com.condox.ecommerce.client.tree.presenter.OrderDetailsPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.event.logical.shared.HasValueChangeHandlers;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.user.client.ui.Frame;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.InlineHyperlink;
import com.google.gwt.user.client.ui.TextBox;

public class OrderDetailsView<T> extends Composite implements I_Display,
		I_Contained {

	private static OrderDetailsViewUiBinder uiBinder = GWT
			.create(OrderDetailsViewUiBinder.class);
	@UiField
	CheckBox enable;
	@UiField
	Button delete;
	@UiField
	HTML summary;
	@UiField TextBox orderURL;

	interface OrderDetailsViewUiBinder extends
			UiBinder<Widget, OrderDetailsView> {
	}

	public OrderDetailsView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	private I_OrderDetailsPresenter<T> presenter;

	public void setPresenter(I_OrderDetailsPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public HasClickHandlers getDelete() {
		return delete;
	}

	@Override
	public HasValueChangeHandlers<Boolean> getEnabled() {
		return enable;
	}

	private ViewOrderInfo info;
	@Override
	public void setData(ViewOrderInfo info) {
		this.info = info;
//		String preview = info.getVirtualTourUrl();
//		if (preview.startsWith("http://"))
//			preview = preview.replaceFirst("http://", "https://");
//		vtPreview.setUrl(preview);
//		
//		String moreInfo = info.getMoreInfoUrl();
//		if (moreInfo.startsWith("http://"))
//			moreInfo = moreInfo.replaceFirst("http://", "https://");
//		miPreview.setUrl(moreInfo);
		orderURL.setText(info.getUrl());
		enable.setValue(info.isEnabled());
		if (enable.getValue())
			enable.setText("Enabled");
		else
			enable.setText("Disabled");
	}

	@Override
	public void setData(String html) {
		summary.setHTML(html);
	}
}
