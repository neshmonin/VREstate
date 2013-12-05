package com.condox.ecommerce.client.tree.model;

import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTreeNode;
import com.condox.ecommerce.client.tree.presenter.ProductPresenter;
import com.condox.ecommerce.client.tree.view.ProductView;

public class ProductModel extends EcommerceTreeNode {

	public static String simpleName = "ProductModel";

	public ProductModel() {}

	/*@Override
	public boolean isValid() {
		boolean valid = true;
		return valid;
	}*/

	private I_Container container = null;

	/**
	 * @wbp.parser.entryPoint
	 */
	@Override
	public void go(I_Container container) {
//		super.go(container, navigator);
		this.container = container;
		new ProductPresenter(new ProductView(), this).go(container);
		super.go(container);
	}
	
	public void prev() {
//		Log.write("ProductModel.prev");
		I_TreeNode node = EcommerceTree.getPrevNode();
		if (node != null)
			node.go(container);
	}
	
	public void next() {
//		Log.write("ProductModel.next");
		I_TreeNode node = EcommerceTree.getNextNode();
		node.go(container);
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		String productType = EcommerceTree.get(Field.ProductType).asString();
		result = prime * result + productType.hashCode();
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
	//***************************

	@Override
	public String getNavURL() {
		return "Product type";
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}
}
