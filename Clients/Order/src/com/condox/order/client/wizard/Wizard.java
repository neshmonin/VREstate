package com.condox.order.client.wizard;

import com.google.gwt.dom.client.Style.Unit;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.LayoutPanel;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.StackLayoutPanel;

public class Wizard implements I_Wizard {

	private static Wizard instance = null;
	private StackLayoutPanel stackLayoutPanel = new StackLayoutPanel(Unit.EM);
	private HasWidgets container = null;
	
	public Wizard(HasWidgets container) {
//		stackLayoutPanel.add(new HTMLPanel("New Panel"), new HTML("New Widget"), 2.0);
//		stackLayoutPanel.setSize("100%", "100%");
//		container.add(stackLayoutPanel);
		this.container = container;
		instance = this;
	}
	
	public static void cancel() {
		instance.popup.hide();
	}
	
	private DialogBox popup = new DialogBox();
	
	@Override
	public void go(I_WizardStep startPoint) {
//		startPoint.go(getContainer());
//		startPoint.go(RootLayoutPanel.get());
		HTMLPanel container = new HTMLPanel("");
		container.setSize("750px", "500px");
		
		popup.setWidget(container);
		popup.center();
//		startPoint.go(container);
		startPoint.go(container);
	}

	@Override
	public HasWidgets getContainer() {
		return (HasWidgets) stackLayoutPanel.getWidget(0);
	}

}
