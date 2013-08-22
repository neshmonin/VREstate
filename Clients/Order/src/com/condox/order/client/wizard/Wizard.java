package com.condox.order.client.wizard;

import com.google.gwt.dom.client.Style.Unit;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.StackLayoutPanel;

public class Wizard implements I_Wizard {

	private StackLayoutPanel stackLayoutPanel = new StackLayoutPanel(Unit.EM);
	private HasWidgets container = null;
	
	public Wizard(HasWidgets container) {
//		stackLayoutPanel.add(new HTMLPanel("New Panel"), new HTML("New Widget"), 2.0);
//		stackLayoutPanel.setSize("100%", "100%");
//		container.add(stackLayoutPanel);
		this.container = container;
	}
	
	@Override
	public void go(I_WizardStep startPoint) {
//		startPoint.go(getContainer());
//		startPoint.go(RootLayoutPanel.get());
		startPoint.go(container);
	}

	@Override
	public HasWidgets getContainer() {
		return (HasWidgets) stackLayoutPanel.getWidget(0);
	}

}
