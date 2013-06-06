package com.condox.order.client.view.factory;

import com.condox.order.client.context.IContext;
import com.condox.order.client.context.Tree;
import com.condox.order.client.view.BuildingsView;
import com.condox.order.client.view.OrderTypeView;
import com.condox.order.client.view.SuitesView;
import com.condox.order.client.view.WelcomeView;

public class ViewFactory {

	private IViewContainer container;

	public void setViewContainer(IViewContainer container) {
		this.container = container;
	}

	public void showView(Tree tree) {
		IContext context = tree.getSelectedContext();
		if (context != null) {
			IView view = null;
			switch (context.getType()) {
			case WELCOME:
				view = new WelcomeView(tree);
				break;
			case BUILDINGS:
				view = new BuildingsView(tree);
				break;
			case SUITES:
				view = new SuitesView(tree);
				break;
			case ORDER_TYPE:
				view = new OrderTypeView(tree);
				break;
			}
			if (view != null)
				if (container != null)
					container.setView(view);
		}
	}
}
