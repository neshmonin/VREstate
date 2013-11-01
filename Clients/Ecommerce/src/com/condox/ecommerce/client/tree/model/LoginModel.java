package com.condox.ecommerce.client.tree.model;

import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.TreeNode;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.view.LoginView;
import com.google.gwt.user.client.ui.HasWidgets;

public class LoginModel extends TreeNode {

	public static String simpleName = "LoginModel";
	public LoginModel() {}

//	@Override
	public boolean isValid() {
		/*
		 * boolean valid = true; valid &= !sid.isEmpty();
		 */
		boolean valid = true;
		valid &= "web".equals(EcommerceTree.get(Field.UserLogin).asString());
		valid &= "web".equals(EcommerceTree.get(Field.UserPassword).asString());
		return valid;
	}

	private HasWidgets container = null;
	@Override
	public void go(HasWidgets container) {
		this.container = container;
		LoginPresenter presenter = new LoginPresenter(new LoginView(), this);
		presenter.go(container);
		super.go(container);
	}

	public void next() {
		I_TreeNode node = EcommerceTree.getNextNode();
		node.go(container);
	}

	@Override
	public String getNavURL() {
		Data LoginData = EcommerceTree.get(Field.UserLogin);
		if (LoginData == null)
			return "Login";
		
		return LoginData.asString() == "web" ? "<Guest>" : LoginData.asString();
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}
}
