package com.condox.vrestate.client.my;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.tree.TreeNode;

public class NodeFactory {
	public static TreeNode create(String type) {
		Log.write("nodeType: " + type);
		if (DefaultNode.simpleName.equals(type))
			return new DefaultNode();
		if (LoginPresenter.simpleName.equals(type))
			return new LoginPresenter(new LoginView());
		return null;
	} 
}
