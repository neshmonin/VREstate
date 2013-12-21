package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.AgreementPresenter;
import com.condox.ecommerce.client.tree.view.AgreementView;

public class AgreementNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		AgreementPresenter presenter = new AgreementPresenter(new AgreementView(), this);
		presenter.go(tree.container);
	}
}
