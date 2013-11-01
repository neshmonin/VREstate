package com.condox.ecommerce.client.tree.model;

import com.condox.clientshared.document.SuiteInfo;
import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.presenter.SuitesPresenter;
import com.condox.ecommerce.client.tree.view.SuitesView;
import com.google.gwt.user.client.ui.HasWidgets;

public class SuitesModel extends TreeNode {

	public static String simpleName = "SuitesModel";

	public SuitesModel() {}

	private int selectedIndex = 0;
	private SuiteInfo selected = null;

	public void setSelected(SuiteInfo obj) {
		selected = obj;
	}

	public SuiteInfo getSelected() {
		return selected;
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + getSelectedIndex();
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
		SuitesModel other = (SuitesModel) obj;
		if (getSelectedIndex() != other.getSelectedIndex())
			return false;
		return true;
	}

	@Override
	public void go(HasWidgets container) {
		this.container = container;
		new SuitesPresenter(new SuitesView(), this).go(container);
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

	public void setSelectedIndex(int selectedIndex) {
		this.selectedIndex = selectedIndex;
	}

	public int getSelectedIndex() {
		return selectedIndex;
	}

	@Override
	public String getNavURL() {
		Data suiteData = EcommerceTree.get(Field.SuiteName);
		if (suiteData == null)
			return "Suite";
		
		return suiteData.asString();
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}

}
