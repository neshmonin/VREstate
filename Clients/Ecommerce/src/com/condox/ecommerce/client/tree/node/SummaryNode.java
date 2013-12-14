package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.OptionsPresenter;
import com.condox.ecommerce.client.tree.presenter.SummaryPresenter;
import com.condox.ecommerce.client.tree.view.OptionsView;
import com.condox.ecommerce.client.tree.view.SummaryView;

public class SummaryNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		SummaryPresenter presenter = new SummaryPresenter(new SummaryView(), this);
		presenter.go(tree.container);
	}
}
