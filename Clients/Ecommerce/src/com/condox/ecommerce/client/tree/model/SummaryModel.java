package com.condox.ecommerce.client.tree.model;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.I_Container;
import com.condox.ecommerce.client.tree.I_TreeNode;
import com.condox.ecommerce.client.tree.TreeNode;
import com.condox.ecommerce.client.tree.presenter.SummaryPresenter;
import com.condox.ecommerce.client.tree.view.SummaryView;

public class SummaryModel extends TreeNode {

	public static String simpleName = "SummaryModel";
	public SummaryModel() {}

	private String role = "";
	private String uid = "";
	private String pwd = "";
	private String sid = "";
	// GETTERS
	public String getRole() {
		return this.role;
	}

	public String getUserLogin() {
		return this.uid;
	}

	public String getUserPassword() {
		return this.pwd;
	}

	public String getUserSid() {
		return this.sid;
	}

	// SETTERS
	public void setUserLogin(String uid) {
		this.uid = uid;
	}

	public void setUserPassword(String pwd) {
		this.pwd = pwd;
	}

	public void setUserSid(String sid) {
		this.sid = sid;
	}

//	@Override
	public boolean isValid() {
		/*
		 * boolean valid = true; valid &= !sid.isEmpty();
		 */
		boolean valid = true;
		valid &= "web".equals(uid);
		valid &= "web".equals(pwd);
		return valid;
	}

	private I_Container container = null;
	@Override
	public void go(I_Container container) {
		this.container = container;
		SummaryPresenter presenter = new SummaryPresenter(new SummaryView(), this);
		presenter.go(container);
		super.go(container);
	}

	public void next() {
		I_TreeNode node = EcommerceTree.getNextNode();
		node.go(container);
	}
	
	public void prev() {
		I_TreeNode node = EcommerceTree.getPrevNode();
		if (node != null)
			node.go(container);
	}

	@Override
	public String getNavURL() {
		return "";
	}

	@Override
	public String getStateString() {
		return simpleName + "." + getState();
	}

}
