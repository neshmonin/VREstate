package com.condox.order.client.presenter;

import com.condox.order.client.context.IContext;
import com.condox.order.client.context.ContextTree;
import com.condox.order.client.view.IViewContainer;
import com.condox.order.client.view.LoginView;
import com.condox.order.client.view.BuildingsView;
import com.condox.order.client.view.SubmitView;
import com.condox.order.client.view.SuitesView;
import com.google.gwt.event.shared.EventBus;

public class PresenterFactory {
	public IViewContainer container;
	public ContextTree tree;
	private EventBus eventBus = null;

	public void setViewContainer(IViewContainer container) {
		this.container = container;
	}

	public IPresenter getPresenter(IContext context) {
		if (context != null) {
			IPresenter presenter = null;
			switch (context.getType()) {
			case LOGIN:
				presenter = new LoginPresenter(eventBus, tree, new LoginView());
				break;
			case BUILDINGS:
				presenter = new BuildingsPresenter(eventBus, tree,
						new BuildingsView());
				break;
			case SUITES:
				presenter = new SuitesPresenter(eventBus, tree,
						new SuitesView());
				break;
			case SUBMIT:
				presenter = new SubmitPresenter(eventBus, tree,
						new SubmitView());
				break;
			}
			return presenter;
		}
		return null;
	}
}
