package com.condox.order.client.wizard.model;

import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.Wizard;
import com.condox.order.client.wizard.WizardStep;
import com.condox.order.client.wizard.presenter.SummaryPresenter;
import com.condox.order.client.wizard.view.SummaryView;
import com.google.gwt.user.client.ui.HasWidgets;

public class SummaryModel extends WizardStep {

	public SummaryModel(I_WizardStep parent) {
		super(parent);
		// TODO Auto-generated constructor stub
	}

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

	private HasWidgets container = null;
	@Override
	public void go(HasWidgets container) {
		this.container = container;
		SummaryPresenter presenter = new SummaryPresenter(new SummaryView(), this);
		presenter.go(container);
		super.go(container);
	}

	public void next() {
		getNextStep().go(container);
//		Wizard.cancel();
	}
	public void prev() {
		if (getPrevStep() != null)
			getPrevStep().go(container);
	}

	@Override
	protected I_WizardStep createNextStep() {
		children.put(this, new EmailModel(this));
		return children.get(this);
//		return null;
	}
	
	
}
