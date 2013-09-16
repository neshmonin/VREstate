package com.condox.order.client.wizard;

import java.util.HashMap;
import java.util.Map;

import com.google.gwt.dom.client.Document;
import com.google.gwt.http.client.URL;
import com.google.gwt.user.client.Element;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.HasWidgets;

public abstract class WizardStep implements I_WizardStep {

	protected I_WizardStep parent = null;
	//
	protected HasWidgets container = null;
	//
	protected Map<I_WizardStep, I_WizardStep> children = new HashMap<I_WizardStep, I_WizardStep>();

	public WizardStep(I_WizardStep parent) {
		this.parent = parent;
	}

	@Override
	public I_WizardStep getPrevStep() {
		return parent;
	}

	@Override
	public I_WizardStep getNextStep() {
		// Log.write("" + children.toString());
		if (children.get(this) != null)
			return children.get(this);
		else
			return createNextStep();
	}

	protected I_WizardStep createNextStep() {
		return null;
	}

	@Override
	public void go(HasWidgets container) {
		I_WizardStep item = this;
		String str = getNavURL();
		item = item.getPrevStep();
		while (item != null) {
			str = item.getNavURL() + " &#187; " + str;
			item = item.getPrevStep();
		}
//		str = "&#187;";
		Document.get().getElementById("navBar").setInnerHTML(str);
	}
	
	public abstract String getNavURL();

	// ************************
	public void next(I_WizardStep nextStep) {
		if (children.get(this) == null)
			children.put(this, nextStep);
		children.get(this).go(container);
	}
}
