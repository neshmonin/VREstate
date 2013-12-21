package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.ShowHistoryPresenter;
import com.condox.ecommerce.client.tree.view.ShowHistoryView;

public class ShowHistoryNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		ShowHistoryPresenter presenter = new ShowHistoryPresenter(new ShowHistoryView(), this);
		presenter.go(tree.container);
	}
}
