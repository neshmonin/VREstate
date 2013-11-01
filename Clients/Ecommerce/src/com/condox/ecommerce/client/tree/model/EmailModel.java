package com.condox.ecommerce.client.tree.model;

import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.presenter.EmailPresenter;
import com.condox.ecommerce.client.tree.view.EmailView;
import com.google.gwt.user.client.ui.HasWidgets;

public class EmailModel extends TreeNode {

	public static String simpleName = "EmailModel";

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		String mail = EcommerceTree.get(Field.Email).asString();
		result = prime * result + ((mail == null) ? 0 : mail.hashCode());
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		return this.hashCode() == obj.hashCode();
	}

	public EmailModel() {}

	// @Override
	public boolean isValid() {
		boolean valid = true;
		return valid;
	}

	private HasWidgets container = null;

	@Override
	public void go(HasWidgets container) {
		this.container = container;
		EmailPresenter presenter = new EmailPresenter(new EmailView(), this);
		presenter.go(container);
		super.go(container);
	}

	public void next() {
//		Tree.getNextNode().go(container);
	}

	public void prev() {
		I_TreeNode node = EcommerceTree.getPrevNode();
		if (node != null)
			node.go(container);
	}

	@Override
	public String getNavURL() {
		Data emailData = EcommerceTree.get(Field.Email);
		if (emailData == null)
			return "Email";
		
		return emailData.asString();
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}
}
