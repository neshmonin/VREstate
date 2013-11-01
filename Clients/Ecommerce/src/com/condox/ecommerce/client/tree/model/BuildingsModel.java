package com.condox.ecommerce.client.tree.model;

import com.condox.clientshared.document.BuildingInfo;
import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.presenter.BuildingsPresenter;
import com.condox.ecommerce.client.tree.view.BuildingsView;
import com.google.gwt.user.client.ui.HasWidgets;

public class BuildingsModel extends TreeNode {

	public static String simpleName = "BuildingsModel";

	public BuildingsModel() {}

	private int selectedIndex = 0;
	private BuildingInfo selected = null;

	public int getSelectedId() {
		return selectedIndex;
	}

	public void setSelectedId(int id) {
		selectedIndex = id;
	}

	/*
	 * @Override public boolean isValid() { boolean valid = true; // Selected
	 * must be one and only one! int selected = 0; for (int i = 0; i <
	 * listBox.getItemCount(); i++) if (listBox.isItemSelected(i)) selected++;
	 * valid &= (selected == 1); return valid; }
	 */

	public void setSelected(BuildingInfo selected) {
		this.selected = selected;
	}

	public BuildingInfo getSelected() {
		return selected;
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + selectedIndex;
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
		BuildingsModel other = (BuildingsModel) obj;
		if (selectedIndex != other.selectedIndex)
			return false;
		return true;
	}

	@Override
	public void go(HasWidgets container) {
		// super.go(container, navigator);
//		Window.alert("BModel id=" + selectedIndex);
		this.container = container;
		new BuildingsPresenter(new BuildingsView(), this).go(container);
		super.go(container);
	}

	public void prev() {
//		Log.write("BuildingsModel.prev");
		I_TreeNode node = EcommerceTree.getPrevNode();
		if (node != null)
			node.go(container);
	}

	public void next() {
		I_TreeNode node = EcommerceTree.getNextNode();
		node.go(container);
	}

	@Override
	public String getNavURL() {
		Data addressData = EcommerceTree.get(Field.Address);
		if (addressData == null)
			return "Building";
		
		return addressData.asString();
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}

}
