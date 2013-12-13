package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.UpdateProfile1Presenter;
import com.condox.ecommerce.client.tree.view.UpdateProfile1View;

public class UpdateProfile1Node extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		UpdateProfile1Presenter presenter = new UpdateProfile1Presenter(new UpdateProfile1View(), this);
		presenter.go(tree.container);
	}
}
