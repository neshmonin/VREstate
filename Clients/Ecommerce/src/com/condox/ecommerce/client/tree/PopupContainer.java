package com.condox.ecommerce.client.tree;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.dom.client.MouseDownEvent;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.HasHorizontalAlignment;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.ui.Widget;

public class PopupContainer implements I_Container {
	
	private DialogBox popup = new DialogBox() {
		protected void beginDragging(MouseDownEvent event) {
			event.preventDefault();
		};
	};
	
	private HTMLPanel container = new HTMLPanel("");
	
	public PopupContainer() {
		VerticalPanel vp = new VerticalPanel();

//		Button btnClose = new Button("Close");
//		btnClose.addClickHandler(new ClickHandler() {
//
//			@Override
//			public void onClick(ClickEvent event) {
//				popup.hide();
//			}
//		});
//
//		vp.add(btnClose);
//		vp.setCellHorizontalAlignment(btnClose,
//				HasHorizontalAlignment.ALIGN_RIGHT);

		vp.add(container);

		popup.setModal(true);
		popup.setGlassEnabled(true);
		popup.setWidget(vp);
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
