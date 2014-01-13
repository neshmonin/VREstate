package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.ForgotPasswordPresenter;
import com.condox.ecommerce.client.tree.view.ForgotPasswordView;

public class ForgotPasswordNode extends AbstractNode {

	@Override
	public void go(EcommerceTree tree) {
		super.go(tree);
		ForgotPasswordPresenter presenter = new ForgotPasswordPresenter(new ForgotPasswordView(), this);
		presenter.go(tree.container);
	}

}
