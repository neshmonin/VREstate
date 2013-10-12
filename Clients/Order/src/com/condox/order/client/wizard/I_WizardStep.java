package com.condox.order.client.wizard;

import com.google.gwt.user.client.ui.HasWidgets;

public interface I_WizardStep {
	I_WizardStep getPrevStep();
	I_WizardStep getNextStep();
	void go(HasWidgets container);
	String getNavURL();
}
