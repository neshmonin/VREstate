package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.HelloAgentPresenter;
import com.condox.ecommerce.client.tree.view.HelloAgentView;

public class HelloAgentNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		HelloAgentPresenter presenter = new HelloAgentPresenter(new HelloAgentView(), this);
		presenter.go(tree.container);
	}
}
