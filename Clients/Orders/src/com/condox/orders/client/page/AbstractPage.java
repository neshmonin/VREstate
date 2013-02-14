package com.condox.orders.client.page;

import com.google.gwt.user.client.ui.LayoutPanel;
import com.google.gwt.user.client.ui.RootLayoutPanel;

public class AbstractPage extends LayoutPanel {
	
	public void Load(AbstractPage new_page) {
		RootLayoutPanel.get().clear();
		if (new_page != null) {
//			new_page.setSize("100%", "100%");
			RootLayoutPanel.get().add(new_page);
		}
	}
}
