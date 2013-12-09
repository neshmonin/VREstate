package com.condox.ecommerce.client.tree.node;

import com.condox.clientshared.abstractview.Log;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;

public abstract class AbstractNode {
	
	private EcommerceTree tree = null;
	private AbstractNode parent = null;
	private NodeStates state = null;
	
	public void setParent(AbstractNode newParent) {
//		Log.write("setParent():");
		parent = newParent;
	}
	
	public AbstractNode getParent() {
		return parent;
	}
	
	public void setState(NodeStates newState) {
		state = newState;
	}
	
	public String getName() {
		String name = this.getClass().getName();
		name = name.substring(name.lastIndexOf('.') + 1);
		return name; 
	}
	
	public String getLeaf() {
//		Log.write("getLeaf():");
		String leaf = getName();
		
		if (state != null)
			leaf += "." + state;
		
		if (parent != null)
			leaf = parent.getLeaf() + "/" + leaf;
//		Log.write("leaf = " + leaf);
		return leaf;
	}
	
	
	public void go(EcommerceTree newTree) {
		tree = newTree;
	};
	
	public void next() {
		if (tree != null)
			tree.next();
	};
}
