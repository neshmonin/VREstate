package com.condox.order.client;

import com.condox.clientshared.abstractview.Log;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.LayoutPanel;
import com.google.gwt.user.client.ui.Widget;

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
