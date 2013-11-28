package com.condox.ecommerce.client.tree.model;

import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTreeNode;
import com.condox.ecommerce.client.tree.presenter.HistoryPresenter;
import com.condox.ecommerce.client.tree.view.HistoryView;

public class HistoryNode extends EcommerceTreeNode {

	public static String simpleName = "HistoryNode";
	public HistoryNode() {}

//	@Override
	public boolean isValid() {
		/*
		 * boolean valid = true; valid &= !sid.isEmpty();
		 */
		boolean valid = true;
//		valid &= "web".equals(EcommerceTree.get(Field.UserHistory).asString());
//		valid &= "web".equals(EcommerceTree.get(Field.UserPassword).asString());
		return valid;
	}

	private I_Container container = null;
	@Override
	public void go(I_Container container) {
		this.container = container;
		HistoryPresenter presenter = new HistoryPresenter(new HistoryView(), this);
		presenter.go(container);
		super.go(container);
	}

	public void next() {
		I_TreeNode node = EcommerceTree.getNextNode();
		node.go(container);
	}

	@Override
	public String getNavURL() {
//		Data HistoryData = EcommerceTree.get(Field.UserHistory);
//		if (HistoryData == null)
//			return "History";
//		
//		return HistoryData.asString() == "web" ? "<Guest>" : HistoryData.asString();
		return "";
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}
}
