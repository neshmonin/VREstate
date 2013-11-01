package com.condox.ecommerce.client.tree;

import java.util.Map;

import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.google.gwt.user.client.ui.HasWidgets;

public interface I_TreeNode {
	
	Data getData(EcommerceTree.Field name);
	void setData(EcommerceTree.Field name, Data data);
	void setState(State readyState);
	State getState();
	String getStateString();
	
	I_TreeNode getParent();
	void setParent(I_TreeNode parent);
	Map<I_TreeNode, I_TreeNode> getChildren();
	
	void go(HasWidgets container);
	String getNavURL();
}
