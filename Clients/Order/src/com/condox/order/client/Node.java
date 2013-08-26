package com.condox.order.client;

import java.util.HashMap;
import java.util.Map;

public abstract class Node implements I_Node {

	@Override
	public void setParent(I_Node parent) {
		this.parent = parent;
	}

	private I_Node parent = null;
	private Map<I_Node, I_Node> children = new HashMap<I_Node, I_Node>();

	public I_Node getNext() {
		return children.get(this);
	}
	
	public I_Node getPrev() {
		return parent;
	}
	
	public I_Node addChild(I_Node child) {
		child.setParent(this);
		children.put(this, child);
		return child;
	}
	
	
}
