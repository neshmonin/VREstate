package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.ChangingPasswordPresenter;
import com.condox.ecommerce.client.tree.view.ChangingPasswordView;

public class ChangingPasswordNode extends AbstractNode {

	@Override
	public void go(EcommerceTree tree) {
		super.go(tree);
		ChangingPasswordPresenter presenter = new ChangingPasswordPresenter(new ChangingPasswordView(), this);
		presenter.go(tree.container);
		
	}

}
