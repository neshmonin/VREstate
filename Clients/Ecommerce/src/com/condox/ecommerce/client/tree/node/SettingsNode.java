package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.SettingsPresenter;
import com.condox.ecommerce.client.tree.view.SettingsView;

public class SettingsNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		SettingsPresenter presenter = new SettingsPresenter(new SettingsView(), this);
		presenter.go(tree.container);
	}
}
