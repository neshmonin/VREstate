package com.condox.order.client.wizard;

import com.google.gwt.dom.client.Style.Unit;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTMLPanel;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.StackLayoutPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class Wizard implements I_Wizard {

	private static Wizard instance = null;
	private StackLayoutPanel stackLayoutPanel = new StackLayoutPanel(Unit.EM);

	public Wizard(HasWidgets container) {
		instance = this;
	}
	
	public static void cancel() {
		instance.popup.hide();
	}
	
	private DialogBox popup = new DialogBox();
	
	@Override
	public void go(I_WizardStep startPoint) {
		VerticalPanel vp = new VerticalPanel();
		HorizontalPanel hp = new HorizontalPanel();
		hp.getElement().setId("navBar");
		hp.add(new Button(""));

		HTMLPanel container = new HTMLPanel("");
		container.setSize("750px", "500px");

		vp.add(hp);
		vp.add(container);
		
		popup.setWidget(vp);
		popup.center();
//		startPoint.go(container);
		startPoint.go(container);
	}

	@Override
	public HasWidgets getContainer() {
		return (HasWidgets) stackLayoutPanel.getWidget(0);
	}

}
