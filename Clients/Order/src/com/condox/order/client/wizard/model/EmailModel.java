package com.condox.order.client.wizard.model;

import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.WizardStep;
import com.condox.order.client.wizard.presenter.EmailPresenter;
import com.condox.order.client.wizard.view.EmailView;
import com.google.gwt.user.client.ui.HasWidgets;

public class EmailModel extends WizardStep {

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
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
		EmailModel other = (EmailModel) obj;
		if (mail == null) {
			if (other.mail != null)
				return false;
		} else if (!mail.equals(other.mail))
			return false;
		return true;
	}

	public EmailModel(I_WizardStep parent) {
		super(parent);
	}

	private String mail = "";

	// GETTERS
	public String getOwnerMail() {
		return this.mail;
	}

	// SETTERS
	public void setOwnerMail(String mail) {
		this.mail = mail;
	}

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
//		getNextStep().go(container);
	}

	public void prev() {
		if (getPrevStep() != null)
			getPrevStep().go(container);
	}

	@Override
	protected I_WizardStep createNextStep() {
		children.put(this, new SummaryModel(this));
		return children.get(this);
	}

	@Override
	public String getNavURL() {
		return "Email";
	}
	
}
