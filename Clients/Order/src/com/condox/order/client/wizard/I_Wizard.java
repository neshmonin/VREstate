package com.condox.order.client.wizard;

import com.google.gwt.user.client.ui.HasWidgets;


public interface I_Wizard {
	void go(I_WizardStep startPoint);
	HasWidgets getContainer();
}
