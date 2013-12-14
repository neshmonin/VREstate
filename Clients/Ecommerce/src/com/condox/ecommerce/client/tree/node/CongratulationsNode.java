package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.CongratulationsPresenter;
import com.condox.ecommerce.client.tree.view.CongratulationsView;

public class CongratulationsNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		CongratulationsPresenter presenter = new CongratulationsPresenter(new CongratulationsView(), this);
		presenter.go(tree.container);
	}
}
