package com.condox.ecommerce.client.tree;

import java.util.HashMap;
import java.util.Map;

import com.condox.clientshared.abstractview.Log;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.google.gwt.user.client.ui.HasWidgets;

public abstract class TreeNode implements I_TreeNode {
	
	protected I_TreeNode parent = null;
	@Override
	public I_TreeNode getParent() {
		return parent;
	}
	@Override
	public void setParent(I_TreeNode parent) {
		this.parent = parent;
	}
	//
	
	protected HasWidgets container = null;
	//
	protected Map<I_TreeNode, I_TreeNode> children = new HashMap<I_TreeNode, I_TreeNode>();
	@Override
	public Map<I_TreeNode, I_TreeNode> getChildren() {
		return children;
	}
	
	protected Map<EcommerceTree.Field, Data> dataRepository = new HashMap<EcommerceTree.Field, Data>();

	@Override
	public Data getData(EcommerceTree.Field name) {
		if (dataRepository.containsKey(name))
			return dataRepository.get(name);
		
		return null;
	}
	
	@Override
	public void setData(EcommerceTree.Field name, Data data) {
		dataRepository.put(name, data);
	}

	@Override
	public void go(HasWidgets container) {
		I_TreeNode item = this;
		String str = item.getNavURL();
		item = item.getParent();
		while (item != null) {
			if (!item.getNavURL().isEmpty())
				str = item.getNavURL() + " &#187; " + str;
			item = item.getParent();
		}
//		str = "&#187;";
		Log.write(str);
//		Document.get().getElementById("navBar").setInnerHTML(str);
		setNavText(str);
	}
	
	private native void setNavText(String str) /*-{
		$doc.getElementById('navBar').rows[0].cells[0].innerHTML = str;
	}-*/;
	
	public abstract String getNavURL();

	// ************************
	public void next(I_TreeNode nextNode) {
		if (children.get(this) == null)
			children.put(this, nextNode);
		children.get(this).go(container);
	}

	private State state = State.NotReady;
	@Override
	public void setState(State readyState) {
		state = readyState;
	}
	
	@Override
	public State getState() {
		return state;
	}
}
