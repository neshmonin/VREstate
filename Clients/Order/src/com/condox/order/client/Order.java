package com.condox.order.client;

import com.condox.order.client.context.BuildingsContext;
import com.condox.order.client.context.Tree;
import com.condox.order.client.view.factory.MyViewContainer;
import com.condox.order.client.view.factory.ViewFactory;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.user.client.ui.RootPanel;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class Order implements EntryPoint {
	/**
	 * This is the entry point method.
	 */
	public void onModuleLoad() {
		ViewFactory viewFactory = new ViewFactory();
		viewFactory.setViewContainer(new MyViewContainer(RootPanel.get()));
		Tree tree = new Tree(viewFactory);
		
		tree.next(new BuildingsContext());
	}
}