package com.condox.order.client.view.factory;

import com.google.gwt.user.client.ui.HasWidgets;

public class MyViewContainer implements IViewContainer {
	private HasWidgets container;
	
	public MyViewContainer(HasWidgets container) {
		this.container = container;
	}

	@Override
	public void setView(IView view) {
		if (container != null) {
			container.clear();
			if (view != null) {
				container.clear();
				container.add(view.asWidget());
			}
		}
	}
}
