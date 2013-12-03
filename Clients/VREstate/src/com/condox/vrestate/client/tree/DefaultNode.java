package com.condox.vrestate.client.tree;

import com.condox.clientshared.tree.TreeNode;

public class DefaultNode extends TreeNode{
	public static String simpleName = "Root";
	public DefaultNode(){}

	@Override
	public String getStateString() {
		return simpleName;
	}

	@Override
	public String getNavURL() {
		return "";
	}

}
