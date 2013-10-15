package com.condox.order.client.wizard.model;

import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.WizardStep;
import com.condox.order.client.wizard.presenter.MLSPresenter;
import com.condox.order.client.wizard.view.MLSView;
import com.google.gwt.user.client.ui.HasWidgets;

public class MLSModel extends WizardStep {

	public MLSModel(I_WizardStep parent) {
		super(parent);
	}

	private String mls = "";

	// GETTERS
	public String getMLS() {
		return mls;
	}

	// SETTERS
	public void setMLS(String value) {
		mls = value;
	}

	private HasWidgets container = null;

	@Override
	public void go(HasWidgets container) {
		this.container = container;
		new MLSPresenter(new MLSView(), this).go(container);
		super.go(container);
	}

	public void prev() {
		if (getPrevStep() != null)
			getPrevStep().go(container);
	}

	public void next() {
		getNextStep().go(container);
	}

	@Override
	protected I_WizardStep createNextStep() {
		if (mls.isEmpty())
			children.put(this, new BuildingsModel(this));
		else
			children.put(this, new ListingOptionsModel(this));
		return children.get(this);
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((mls == null) ? 0 : mls.hashCode());
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
		MLSModel other = (MLSModel) obj;
		if (mls == null) {
			if (other.mls != null)
				return false;
		} else if (!mls.equals(other.mls))
			return false;
		return true;
	}

	@Override
	public String getNavURL() {
		return "MLS#";
	}

	@Override
	public StepTypes getStepType() {
		return StepTypes.MLSModel;
	}
}
