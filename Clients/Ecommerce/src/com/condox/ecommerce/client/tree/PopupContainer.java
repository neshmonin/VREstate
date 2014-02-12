package com.condox.ecommerce.client.tree;

import java.util.Iterator;

import com.condox.clientshared.container.I_Contained;
import com.google.gwt.event.dom.client.MouseDownEvent;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.ui.Widget;

public class PopupContainer implements HasWidgets {
	
	private DialogBox popup = new DialogBox() {
		protected void beginDragging(MouseDownEvent event) {
			event.preventDefault();
		};
	};
	
	private HTMLPanel container = new HTMLPanel("");
	
	public PopupContainer() {
		VerticalPanel vp = new VerticalPanel();

		vp.add(container);

//		popup.setModal(true);
		popup.setGlassEnabled(true);
		popup.setWidget(vp);
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

	@Override
	public void add(Widget child) {
		if (child == null) 
			clear();
		else 
			container.add(child);
		update();
	}

	@Override
	public Iterator<Widget> iterator() {
		return container.iterator();
	}

	@Override
	public boolean remove(Widget child) {
		return container.remove(child);
	}

}
