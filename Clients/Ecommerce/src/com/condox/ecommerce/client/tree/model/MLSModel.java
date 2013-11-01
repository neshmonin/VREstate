package com.condox.ecommerce.client.tree.model;

import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.presenter.MLSPresenter;
import com.condox.ecommerce.client.tree.view.MLSView;
import com.google.gwt.user.client.ui.HasWidgets;

public class MLSModel extends TreeNode {

	public static String simpleName = "MLSModel";

	public MLSModel() {	}

	private HasWidgets container = null;

	@Override
	public void go(HasWidgets container) {
		this.container = container;
		new MLSPresenter(new MLSView(), this).go(container);
		super.go(container);
	}

	public void prev() {
		I_TreeNode node = EcommerceTree.getPrevNode();
		if (node != null)
			node.go(container);
	}

	public void next() {
		I_TreeNode node = EcommerceTree.getNextNode();
		node.go(container);
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		String mls = EcommerceTree.get(Field.MLS).asString();
		result = prime * result + ((mls == null) ? 0 : mls.hashCode());
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

		return this.hashCode() == obj.hashCode();
	}

	@Override
	public String getNavURL() {
		Data MLSData = EcommerceTree.get(Field.MLS);
		if (MLSData == null)
			return "MLS#";
		
		return MLSData.asString().isEmpty() ? "Address" : MLSData.asString();
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}
}
