package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.UpdateProfile2Presenter;
import com.condox.ecommerce.client.tree.view.UpdateProfile2View;

public class UpdateProfile2Node extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		UpdateProfile2Presenter presenter = new UpdateProfile2Presenter(new UpdateProfile2View(), this);
		presenter.go(tree.container);
	}
}
