package com.condox.vrestate.client.my2;

import java.util.HashMap;
import java.util.Map;

import com.condox.clientshared.tree.Data;

public class Node implements I_Node {

	protected I_Node parent = null;

	protected Map<I_Node, I_Node> children = new HashMap<I_Node, I_Node>();
	
	protected Map<String, Data> dataRepository = new HashMap<String, Data>();
	
	public Node(I_Node parent) {
		this.parent = parent;
	}
	
	public Node() {
		this(null);
	}

	@Override
	public I_Node getParent() {
		return parent;
	}
	
	@Override
	public I_Node getChild() {
		if (children.containsKey(this))
			return children.get(this);
		else
			return null;	// use factory
	}

	@Override
	public Data getData(String name) {
		if (dataRepository.containsKey(name))
			return dataRepository.get(name);
		else if (parent != null)
			return parent.getData(name);
		else
			return null;
	}

	@Override
	public void setData(String name, Data value) {
		dataRepository.put(name, value);
	}
}
