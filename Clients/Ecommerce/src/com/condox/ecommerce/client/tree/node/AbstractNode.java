package com.condox.ecommerce.client.tree.node;

import java.util.HashMap;
import java.util.Map;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;

public abstract class AbstractNode {
	
	private EcommerceTree tree = null;
	private AbstractNode parent = null;
	private Map<AbstractNode, AbstractNode> children = new HashMap<AbstractNode, AbstractNode>();
	protected Map<Field, Data> dataRepository = new HashMap<Field, Data>();
	private NodeStates state = null;
	
	public EcommerceTree getTree() {
		return tree;
	}
	
	public AbstractNode getParent() {
		return parent;
	}
	
	public void setParent(AbstractNode newParent) {
		parent = newParent;
	}
	
	public boolean hasParent() {
		return parent != null;
	}
	
	public AbstractNode addChild(AbstractNode newChild) {
		//---------
//		Log.write("this: " + this);
//		Log.write("children keys: " + children.keySet());
//		Log.write("children values: " + children.values());
//		Log.write("children get(this): " + children.get(this));
		//---------
		if (children.get(this) != null) return children.get(this);
		newChild.setParent(this);
		children.put(this, newChild);
		return newChild;
	}
	
	public Data getData(Field key) {
		if (dataRepository.containsKey(key))
			return dataRepository.get(key);
		return null;
	}

	public void setData(Field key, Data data) {
		dataRepository.put(key, data);
	}
	
	public NodeStates getState() {
		return state;
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
	}
	
	public void next(NodeStates state) {
		setState(state);
		if (tree != null)
			tree.next();
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result
				+ ((dataRepository == null) ? 0 : dataRepository.hashCode());
		result = prime * result + ((state == null) ? 0 : state.hashCode());
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		AbstractNode other = (AbstractNode) obj;
		if (dataRepository == null) {
			if (other.dataRepository != null)
				return false;
		} else if (!dataRepository.equals(other.dataRepository))
			return false;
		if (state != other.state)
			return false;
		return true;
	}


	
	
}
