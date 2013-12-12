package com.condox.ecommerce.client.tree;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.google.gwt.event.dom.client.MouseDownEvent;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.Widget;

public class PopupContainer implements I_Container {
	private DialogBox popup = new DialogBox() {
		protected void beginDragging(MouseDownEvent event) {
			event.preventDefault();
		};
	};
	private HTMLPanel container = new HTMLPanel("");
	
	public PopupContainer() {
		container.setSize("100%", "100%");
		popup.setModal(true);
		popup.setGlassEnabled(true);
		popup.setWidget(container);
	}

	@Override
	public void add(I_Contained newChild) {
		Widget child = (Widget)newChild;
		if (child == null) 
			clear();
		else 
			container.add(child);
		update();
	}

	@Override
	public void clear() {
		container.clear();
		update();
	}

	private void update() {
		if (container.getWidgetCount() == 0)
			popup.hide();
		else
			popup.center();
	}

}
