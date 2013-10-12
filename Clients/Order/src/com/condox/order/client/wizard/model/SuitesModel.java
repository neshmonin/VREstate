package com.condox.order.client.wizard.model;

import com.condox.clientshared.document.SuiteInfo;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.WizardStep;
import com.condox.order.client.wizard.presenter.SuitesPresenter;
import com.condox.order.client.wizard.view.SuitesView;
import com.google.gwt.user.client.ui.HasWidgets;

public class SuitesModel extends WizardStep {

	public SuitesModel(I_WizardStep parent) {
		super(parent);
	}

	private int selectedIndex = 0;
	private SuiteInfo selected = null;

	public void setSelected(SuiteInfo obj) {
		selected = obj;
	}

	public SuiteInfo getSelected() {
		return selected;
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + getSelectedIndex();
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
		SuitesModel other = (SuitesModel) obj;
		if (getSelectedIndex() != other.getSelectedIndex())
			return false;
		return true;
	}

	@Override
	public void go(HasWidgets container) {
		this.container = container;
		new SuitesPresenter(new SuitesView(), this).go(container);
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
		children.put(this, new ListingOptionsModel(this));
		return children.get(this);
	}

	public void setSelectedIndex(int selectedIndex) {
		this.selectedIndex = selectedIndex;
	}

	public int getSelectedIndex() {
		return selectedIndex;
	}

	@Override
	public String getNavURL() {
		return "Suites";
	}

}
