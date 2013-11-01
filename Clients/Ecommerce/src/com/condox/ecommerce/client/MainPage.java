package com.condox.ecommerce.client;

import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Image;
import com.google.gwt.user.client.ui.LayoutPanel;
import com.google.gwt.user.client.ui.PushButton;
import com.google.gwt.user.client.ui.Widget;

public class MainPage extends Composite {

	private static MainPageUiBinder uiBinder = GWT
			.create(MainPageUiBinder.class);
	@UiField Image image;
	@UiField LayoutPanel menuPanel;
	@UiField PushButton loginMenu;
	@UiField PushButton pushButton;
	@UiField PushButton pushButton_1;
//	@UiField LayoutPanel logPanel;

	interface MainPageUiBinder extends UiBinder<Widget, MainPage> {
	}

	public MainPage() {
		initWidget(uiBinder.createAndBindUi(this));
//		logPanel.getElement().setId("log");
	}
	@UiHandler("pushButton")
	void onPushButtonClick(ClickEvent event) {
		Window.open("http://www.3dcondox.com/", "_blank", "");
	}
	@UiHandler("pushButton_1")
	void onPushButton_1Click(ClickEvent event) {
		Window.open("http://www.3dcondox.com/contact1.html", "_blank", "");
	}
	@UiHandler("loginMenu")
	void onLoginMenuClick(ClickEvent event) {
		EcommerceTree tree = new EcommerceTree(null);
		tree.activate();
	}
	
}
