package com.condox.order.client.wizard;

import com.google.gwt.user.client.ui.HasWidgets;

public interface I_WizardStep {
	I_WizardStep getPrevStep();
	I_WizardStep getNextStep();
	StepTypes getStepType();
	void go(HasWidgets container);
	String getNavURL();
	
	enum StepTypes {
		LoginModel,
		ProductModel,
		BuildingsModel,
		SuitesModel,
		MLSModel,
		ListingOptionsModel,
		SummaryModel,
		EmailModel
	}
}
