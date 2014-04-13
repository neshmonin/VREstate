package com.condox.ecommerce.client.tree;

import java.util.HashMap;
import java.util.Map;

import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.google.gwt.json.client.JSONObject;

public class EcommerceNode {
	private EcommerceNode parent = null;
	private String name = null;
	private Map<EcommerceNode, EcommerceNode> children = new HashMap<EcommerceNode, EcommerceNode>();
	private Actions action = null;
	protected Map<Field, Data> dataRepository = new HashMap<Field, Data>();
	protected Map<Field, JSONObject> jsonDataRepository = new HashMap<Field, JSONObject>();

	public EcommerceNode(String name) {
		this.name = name;
	}

	// Getters & setters

	public EcommerceNode getParent() {
		return parent;
	}

	public void setParent(EcommerceNode newParent) {
		parent = newParent;
	}

	public String getName() {
		return name;
	}
	
	public void setAction(Actions action) {
		this.action = action;
	}
	
	// Children
	
	public EcommerceNode addChild(EcommerceNode newChild) {
		// if (children.get(this) == null) {
		newChild.setParent(this);
		children.put(this, newChild);
		// }
		return children.get(this);
	}

	public void removeChild(EcommerceNode childToRemove) {
		children.remove(childToRemove);
	}

	public String getLeaf() {
		String leaf = name;

		if (action != null)
			leaf += "." + action;

		if (parent != null)
			leaf = parent.getLeaf() + "/" + leaf;
		return leaf;
	}

	// Data
	protected Data getData(Field key) {
		if (dataRepository.containsKey(key))
			return dataRepository.get(key);
		return null;
	}
	

	protected void setData(Field key, Data data) {
		dataRepository.put(key, data);
	}

	protected JSONObject getJSONData(Field key) {
		if (jsonDataRepository.containsKey(key))
			return jsonDataRepository.get(key);
		return null;
	}
	
	protected void setJSONData(Field key, JSONObject value) {
		jsonDataRepository.put(key, value);
	}
	

	// Else 
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((action == null) ? 0 : action.hashCode());
		result = prime * result
				+ ((dataRepository == null) ? 0 : dataRepository.hashCode());
		result = prime * result + ((name == null) ? 0 : name.hashCode());
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
		EcommerceNode other = (EcommerceNode) obj;
		if (action != other.action)
			return false;
		if (dataRepository == null) {
			if (other.dataRepository != null)
				return false;
		} else if (!dataRepository.equals(other.dataRepository))
			return false;
		if (name == null) {
			if (other.name != null)
				return false;
		} else if (!name.equals(other.name))
			return false;
		return true;
	}

	// //--------------------------
	// // protected List<Data> getCachedData(Field key) {
	// // List<Data> result = new ArrayList<Data>();
	// // if (parent != null)
	// // for (AbstractNode item : parent.children.values())
	// // if (item.getData(key) != null)
	// // result.add(item.getData(key));
	// // return result;
	// // }
	//
	// // Action
	// public States getState() {
	// return state;
	// }
	//
	// public void setState(States newState) {
	// state = newState;
	// }
	//
	// // Name
	// public String getName() {
	// String name = this.getClass().getName();
	// name = name.substring(name.lastIndexOf('.') + 1);
	// return name;
	// }
	//
	// // ??
	// public String getLeaf() {
	// String leaf = getName();
	//
	// if (state != null)
	// leaf += "." + state;
	//
	// if (parent != null)
	// leaf = parent.getLeaf() + "/" + leaf;
	// return leaf;
	// }
	//
	//
	// public void go(EcommerceTree newTree) {
	// tree = newTree;
	// };
	//
	// public void next() {
	// if (tree != null)
	// tree.next();
	// }
	//
	// public void next(States state) {
	// // setAction(state);
	// if (tree != null)
	// tree.next();
	// }
	//
	// @Override
	// public int hashCode() {
	// final int prime = 31;
	// int result = 1;
	// result = prime * result + ((state == null) ? 0 : state.hashCode());
	// result = prime * result
	// + ((dataRepository == null) ? 0 : dataRepository.hashCode());
	// return result;
	// }
	//
	// @Override
	// public boolean equals(Object obj) {
	// if (this == obj)
	// return true;
	// if (obj == null)
	// return false;
	// if (getClass() != obj.getClass())
	// return false;
	// AbstractNode other = (AbstractNode) obj;
	// if (state != other.state)
	// return false;
	// if (dataRepository == null) {
	// if (other.dataRepository != null)
	// return false;
	// } else if (!dataRepository.equals(other.dataRepository))
	// return false;
	// return true;
	// }
	//
	// // public String getName() {
	// // String name = this.getClass().getSimpleName() + "." + state;
	// // return parent != null? parent.getName() + name : name;
	// // }
}
