package com.condox.order.client;

import com.condox.order.client.context.IMyContext;
import com.condox.order.client.context.IMyContext.Types;
import com.condox.order.client.context.MyContext;
import com.condox.order.client.context.MyTree;
import com.condox.order.client.presenter.PresenterFactory;
import com.condox.order.client.view.factory.MyViewContainer;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.user.client.ui.RootLayoutPanel;
import com.google.gwt.user.client.ui.RootPanel;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class Order implements EntryPoint {
	/**
	 * This is the entry point method.
	 */
	public void onModuleLoad() {
		MainPage mainPage = new MainPage();
		RootLayoutPanel.get().clear();
		RootLayoutPanel.get().add(mainPage);
		
		PresenterFactory factory = new PresenterFactory();
		factory.setViewContainer(new MyViewContainer(mainPage.containerPanel));
		MyTree tree = new MyTree(factory);
		
		IMyContext root = new MyContext(Types.LOGIN);
		tree.next(root);
		
		
		
		/*MainPage mainPage = new MainPage();
		RootLayoutPanel.get().clear();
		RootLayoutPanel.get().add(mainPage);
		
		ViewFactory viewFactory = new ViewFactory();
		viewFactory.setViewContainer(new MyViewContainer(RootPanel.get(),mainPage.containerPanel));
		Tree tree = new Tree(viewFactory);
		
		tree.next(new WelcomeContext());*/
		
		
		/*     root
		 *      |
		 *      A  (A1, A2, A3)
		 *    / | \
		 *   B  C  D*/
		
		/*// temp vars
		IMyContext context;
		MyNode node;
		// root node
		context = new MyContext();
		node = new MyNode(context);
		MyTree tree = new MyTree(node);
		// A node
		context = new MyContext();
		context.setValue("", "A1");
		tree.goChild(context);
		// B node
		context = new MyContext();
		context.setValue("", "B");
		tree.goChild(context);
		// back to parent
		tree.goParent();
		tree.getContext().setValue("","A2");
		
		context = new MyContext();
		context.setValue("", "C");
		tree.goChild(context);
		
		tree.goParent();
		tree.getContext().setValue("", "A1");
		tree.goChild(null);*/
		
	  /*  EventBus eventBus = new SimpleEventBus();
	    MyTree tree = new MyTree(new MyNode());
	    AppController appViewer = new AppController(eventBus, tree);
	    appViewer.go(RootLayoutPanel.get());*/
	}
}