package com.condox.orders.client.page;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.dom.client.Style.Unit;

public class MainPage extends AbstractPage {
	public MainPage() {
		
		Button btnA = new Button("A");
		add(btnA);
		setWidgetRightWidth(btnA, 0.0, Unit.PX, 175.0, Unit.PX);
		setWidgetTopBottom(btnA, 45.0, Unit.PX, 223.0, Unit.PX);
		
		Button btnB = new Button("B");
		add(btnB);
		setWidgetLeftRight(btnB, 64.0, Unit.PX, 286.0, Unit.PX);
		setWidgetTopBottom(btnB, 184.0, Unit.PX, 84.0, Unit.PX);
	}
}
