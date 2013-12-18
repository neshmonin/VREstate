package com.condox.ecommerce.client.tree.node;

import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.presenter.NewOrderPresenter;
import com.condox.ecommerce.client.tree.view.LoginView;
import com.condox.ecommerce.client.tree.view.NewOrderView;
import com.google.gwt.user.client.Timer;

public class NewOrderNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		NewOrderPresenter presenter = new NewOrderPresenter(new NewOrderView(), this);
		presenter.go(tree.container);
	}
}
