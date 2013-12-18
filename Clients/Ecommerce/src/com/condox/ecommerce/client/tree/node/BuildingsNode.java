package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.BuildingsPresenter;
import com.condox.ecommerce.client.tree.view.BuildingsView;

public class BuildingsNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		BuildingsPresenter presenter = new BuildingsPresenter(new BuildingsView(), this);
		presenter.go(tree.container);
	}
}
