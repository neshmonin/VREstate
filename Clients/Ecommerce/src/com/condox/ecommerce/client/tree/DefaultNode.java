package com.condox.ecommerce.client.tree;

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
