package com.condox.order.client.view;

import com.google.gwt.user.client.ui.HasWidgets;

public class ViewContainer implements IViewContainer {
	private HasWidgets container;
	
	public ViewContainer(HasWidgets container) {
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
