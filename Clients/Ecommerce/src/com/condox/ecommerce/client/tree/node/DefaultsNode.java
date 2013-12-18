package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.google.gwt.user.client.Timer;

public class DefaultsNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
//		setState(NodeStates.Login);
		tree.next();
	}

}
