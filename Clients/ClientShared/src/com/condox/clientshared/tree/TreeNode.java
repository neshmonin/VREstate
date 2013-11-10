package com.condox.clientshared.tree;

import java.util.HashMap;
import java.util.Map;

import com.condox.clientshared.container.I_Container;

public abstract class TreeNode implements I_TreeNode {

	private I_TreeNode parent = null;
	private Map<I_TreeNode, I_TreeNode> children = new HashMap<I_TreeNode, I_TreeNode>();
	protected Map<String, Data> dataRepository = new HashMap<String, Data>();
	protected I_Container container = null;
	protected String state = "";

	@Override
	public I_TreeNode getParent() {
		return parent;
	}

	@Override
	public void setParent(I_TreeNode parent) {
		this.parent = parent;
	}

	@Override
	public Map<I_TreeNode, I_TreeNode> getChildren() {
		return children;
	}

	@Override
	public Data getData(String name) {
		if (dataRepository.containsKey(name))
			return dataRepository.get(name);
		return null;
	}

	@Override
	public void setData(String name, Data data) {
		dataRepository.put(name, data);
	}

	@Override
	public String getState() {
		return state;
	}
	
	@Override
	public void setState(String state) {
		this.state = state;
	}


	@Override
	public String getStateString() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void go(I_Container container) {
		this.container = container;
	}

	@Override
	public String getNavURL() {
		// TODO Auto-generated method stub
		return null;
	}
	
	public void next(I_TreeNode nextNode) {
		if (children.get(this) == null)
			children.put(this, nextNode);
		children.get(this).go(container);
	}

}
