package com.condox.ecommerce.client.tree.node;

import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.view.LoginView;
import com.google.gwt.user.client.Timer;

public class LoginNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		LoginPresenter presenter = new LoginPresenter(new LoginView(), this);
		presenter.go(tree.container);
	}
}
