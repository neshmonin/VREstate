package com.condox.order.client.context;

import java.util.HashMap;
import java.util.Map;

public class ContextNode {
	protected ContextNode parent;
	protected Map<IContext, ContextNode> children;
	protected IContext context;

	public ContextNode(IContext context) {
		this.context = context;
		children = new HashMap<IContext, ContextNode>();
	}

	public ContextNode getChild(ContextNode defaultChild) {
		if (children.containsKey(context))
			return children.get(context);
		else {
			children.put(context, defaultChild);
			defaultChild.parent = this;
			return defaultChild;
		}
	}
}