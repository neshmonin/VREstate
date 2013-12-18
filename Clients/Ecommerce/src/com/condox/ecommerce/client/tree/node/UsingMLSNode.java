package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.UsingMLSPresenter;
import com.condox.ecommerce.client.tree.view.UsingMLSView;

public class UsingMLSNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		UsingMLSPresenter presenter = new UsingMLSPresenter(new UsingMLSView(), this);
		presenter.go(tree.container);
	}
}
