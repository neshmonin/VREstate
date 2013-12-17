package com.condox.ecommerce.client.tree.node;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.UpdateAvatarPresenter;
import com.condox.ecommerce.client.tree.view.UpdateAvatarView;

public class UpdateAvatarNode extends AbstractNode {

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		UpdateAvatarPresenter presenter = new UpdateAvatarPresenter(new UpdateAvatarView(), this);
		presenter.go(tree.container);
	}
}
