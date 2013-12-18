package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.PickSuitePresenter;
import com.condox.ecommerce.client.tree.view.PickSuiteView;

public class PickSuiteNode extends AbstractNode {
	

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		PickSuitePresenter presenter = new PickSuitePresenter(new PickSuiteView(), this);
		presenter.go(tree.container);
	}
}
