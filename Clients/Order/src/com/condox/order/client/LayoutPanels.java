package com.condox.order.client;

import com.condox.order.client.utils.Log;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasText;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.LayoutPanel;

public class LayoutPanels extends Composite  {

	private static LayoutPanelsUiBinder uiBinder = GWT
			.create(LayoutPanelsUiBinder.class);
	@UiField LayoutPanel container;

	interface LayoutPanelsUiBinder extends UiBinder<Widget, LayoutPanels> {
	}

	public LayoutPanels() {
		initWidget(uiBinder.createAndBindUi(this));
		Timer timer = new Timer() {

			@Override
			public void run() {
				Log.write(": " + container.getOffsetWidth());
			}};
		timer.scheduleRepeating(3000);
	}

}
