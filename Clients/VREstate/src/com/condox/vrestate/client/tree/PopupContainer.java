package com.condox.vrestate.client.tree;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.IsWidget;
import com.google.gwt.user.client.ui.VerticalPanel;

public class PopupContainer implements I_Container {
	private DialogBox popup = new DialogBox();
	private HTMLPanel container = new HTMLPanel("");
	
	public PopupContainer() {
		VerticalPanel vp = new VerticalPanel();
		HorizontalPanel hp = new HorizontalPanel();
		hp.getElement().setId("navBar");
//		hp.add(new Button(""));

		container.setSize("750px", "500px");

		vp.add(hp);
		vp.add(container);
		
		popup.setWidget(vp);
		popup.setGlassEnabled(true);
		popup.setModal(true);
	}

	@Override
	public void add(I_Contained child) {
		container.add((IsWidget)child);
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

	@Override
	public void add(Composite child) {
		
	}

}
