package com.condox.order.client.context;

import com.condox.order.client.presenter.PresenterFactory;
import com.condox.order.client.utils.Log;

public class ContextTree {
	private ContextNode root;
	private ContextNode selected;
	private PresenterFactory factory;

	public ContextTree(PresenterFactory factory) {
		this.factory = factory;
		this.factory.tree = this;
	}

	public void next(IContext context) {
		ContextNode node = new ContextNode(context);
		if (selected != null)
			node = selected.getChild(node);
		selected = node;
		if (root == null)
			root = selected;
		factory.getPresenter(selected.context).go(factory.container);
	}

	public void prev() {
		if ((selected != null) && (selected.parent != null)) {
			selected = selected.parent;
			factory.getPresenter(selected.context).go(factory.container);
		}
	}

	public void setValue(String key, String value) {
		if (selected != null)
			if (selected.context != null)
				selected.context.setValue(key, value);
	}

	public String getValue(String key) {
		ContextNode node = selected;
		while (node != null) {
			if (node.context != null)
				if (node.context.containsValue(key))
					return node.context.getValue(key);
			node = node.parent;
		}
		return "<< VALUE NOT FOUND: " + key + " >>";
	}

	public void log() {
		String result = "log: ";
		ContextNode node = selected;
		while (node != null) {
			result += node.context.toString() + "=>";
			node = node.parent;
		}
		Log.write(result);
	}
	
	/*
	 * user.name
	 * user.password
	 * user.sid
	 * building.id
	 * building.name
	 * building.street
	 * suite.name
	 * suite.floorplan
	 * */

}
