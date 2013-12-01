package com.condox.ecommerce.client.tree.model;

import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTreeNode;
import com.condox.ecommerce.client.tree.presenter.HelloPresenter;
import com.condox.ecommerce.client.tree.view.HelloView;

public class HelloNode extends EcommerceTreeNode {

	public static String simpleName = "HelloNode";
	public HelloNode() {}

//	@Override
	public boolean isValid() {
		/*
		 * boolean valid = true; valid &= !sid.isEmpty();
		 */
		boolean valid = true;
//		valid &= "web".equals(EcommerceTree.get(Field.UserHello).asString());
//		valid &= "web".equals(EcommerceTree.get(Field.UserPassword).asString());
		return valid;
	}

	private I_Container container = null;
	@Override
	public void go(I_Container container) {
		this.container = container;
		HelloPresenter presenter = new HelloPresenter(new HelloView(), this);
		presenter.go(container);
		super.go(container);
	}

	public void next() {
		I_TreeNode node = EcommerceTree.getNextNode();
		node.go(container);
	}

	@Override
	public String getNavURL() {
//		Data HelloData = EcommerceTree.get(Field.UserHello);
//		if (HelloData == null)
//			return "Hello";
//		
//		return HelloData.asString() == "web" ? "<Guest>" : HelloData.asString();
		return "";
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}
}
