package com.condox.clientshared.tree;

import java.util.Map;

import com.condox.clientshared.container.I_Container;

public interface I_TreeNode {
	
	Data getData(String name);
	void setData(String name, Data data);
	void setState(String readyState);
	String getState();
	String getStateString();
	
	I_TreeNode getParent();
	void setParent(I_TreeNode parent);
	Map<I_TreeNode, I_TreeNode> getChildren();
	
	void go(I_Container container);
	String getNavURL();
}
