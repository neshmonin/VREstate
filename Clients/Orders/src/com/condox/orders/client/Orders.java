package com.condox.orders.client;

import com.condox.orders.client.page.LoginPanel;
import com.condox.orders.client.page.building.Building;
import com.condox.orders.client.page.building.SelectBuilding;
import com.condox.orders.client.page.suite.SelectSuite;
import com.condox.orders.client.page.suite.Suite;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.dom.client.Style.Unit;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.user.client.History;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.DockLayoutPanel;
import com.google.gwt.user.client.ui.Image;
import com.google.gwt.user.client.ui.LayoutPanel;
import com.google.gwt.user.client.ui.PushButton;
import com.google.gwt.user.client.ui.RootLayoutPanel;
import com.google.gwt.user.client.ui.TabLayoutPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.DeckPanel;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Label;

public class Orders implements EntryPoint, ValueChangeHandler<String>  {
	
	private TabLayoutPanel panels = new TabLayoutPanel(1.5, Unit.EM);
	private SelectBuilding selectBuildingPage = new SelectBuilding();
	private SelectSuite selectSuitePage = new SelectSuite();
	
	private DockLayoutPanel mainPanel = new DockLayoutPanel(Unit.PX);
	private TabLayoutPanel containerPanel = new TabLayoutPanel(0.0, Unit.EM);
	public static Building selectedBuilding = null;
	public static Suite selectedSuite = null;

	/**
	 * @wbp.parser.entryPoint
	 */
	@Override
	public void onModuleLoad() {
		Log.write("DEBUG_MODE: " + Config.DEBUG_MODE);
		RootLayoutPanel rootLayoutPanel = RootLayoutPanel.get();
		rootLayoutPanel.setStyleName("my-body");
		rootLayoutPanel.add(mainPanel);
//		containerPanel.add(dockLayoutPanel, "New Widget", false);
		History.addValueChangeHandler(this);
		Log.write("onModuleLoad -> Options.Init(this);");
		Options.Init(this);
		
		LayoutPanel layoutPanel = new LayoutPanel();
		layoutPanel.setStyleName("my-header");
		mainPanel.addNorth(layoutPanel, 116.0);
		
		Image image = new Image("templates/cube.jpg");
		layoutPanel.add(image);
		layoutPanel.setWidgetLeftWidth(image, 0.0, Unit.PX, 200.0, Unit.PX);
		layoutPanel.setWidgetTopHeight(image, 0.0, Unit.PX, 116.0, Unit.PX);
		
		Image image_1 = new Image("templates/mainheader.jpg");
		layoutPanel.add(image_1);
		layoutPanel.setWidgetTopHeight(image_1, 0.0, Unit.PX, 116.0, Unit.PX);
		layoutPanel.setWidgetLeftWidth(image_1, 200.0, Unit.PX, 800.0, Unit.PX);
		
		LayoutPanel layoutPanel_1 = new LayoutPanel();
		layoutPanel_1.setStyleName("my-menu");
		mainPanel.addWest(layoutPanel_1, 200.0);
		
		PushButton menu1 = new PushButton("3d condo explorer");
		menu1.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Window.open("http://www.3dcondox.com/", "_blank", "");
			}
		});
		menu1.setStyleName("my-menu-item");
		layoutPanel_1.add(menu1);
		layoutPanel_1.setWidgetLeftWidth(menu1, 10.0, Unit.PX, 178.0, Unit.PX);
		layoutPanel_1.setWidgetTopHeight(menu1, 10.0, Unit.PX, 45.0, Unit.PX);
		
		PushButton menu2 = new PushButton("future page");
		menu2.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Window.open("http://www.3dcondox.com/contact1.html", "_blank", "");
			}
		});
		menu2.setHTML("contact us");
		menu2.setStyleName("my-menu-item");
		layoutPanel_1.add(menu2);
		layoutPanel_1.setWidgetLeftWidth(menu2, 10.0, Unit.PX, 178.0, Unit.PX);
		layoutPanel_1.setWidgetTopHeight(menu2, 55.0, Unit.PX, 45.0, Unit.PX);

		PushButton menu0 = new PushButton("future page");
		menu0.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
//				Window.open("http://www.3dcondox.com/contact1.html", "_blank", "");
				LoginPanel login = new LoginPanel();
				login.Show();
			}
		});
		menu0.setHTML("login");
		menu0.setStyleName("my-menu-item");
		menu0.setVisible(false);
		menu0.setEnabled(false);
		layoutPanel_1.add(menu0);
		layoutPanel_1.setWidgetLeftWidth(menu0, 10.0, Unit.PX, 178.0, Unit.PX);
		layoutPanel_1.setWidgetTopHeight(menu0, 100.0, Unit.PX, 45.0, Unit.PX);
		
		panels.setAnimationDuration(1000);
		
		LayoutPanel selectSuitePanel = new LayoutPanel();
		panels.add(selectSuitePanel, "New Widget", false);
		
		Button btnNewButton_2 = new Button("New button");
		btnNewButton_2.setText("suite");
		selectSuitePanel.add(btnNewButton_2);
		selectSuitePanel.setWidgetLeftWidth(btnNewButton_2, 138.0, Unit.PX, 220.0, Unit.PX);
		selectSuitePanel.setWidgetTopHeight(btnNewButton_2, 111.0, Unit.PX, 32.0, Unit.PX);
		
		LayoutPanel loginPanel = new LayoutPanel();
		panels.add(loginPanel, "Login", false);
		
		Button btnNewButton = new Button("New button");
		btnNewButton.setText("login");
		loginPanel.add(btnNewButton);
		loginPanel.setWidgetLeftWidth(btnNewButton, 160.0, Unit.PX, 167.0, Unit.PX);
		loginPanel.setWidgetTopHeight(btnNewButton, 53.0, Unit.PX, 76.0, Unit.PX);
		mainPanel.addSouth(panels, 0.0);
		containerPanel.setStyleName("my-container");
		mainPanel.add(containerPanel);
	}

	public void LoginUser() {
		Log.write("LoginUser -> User.Login(this);");
		User.Login(this);
	};

	public void StartGE() {
		containerPanel.add(selectBuildingPage);
		containerPanel.setTabText(0, "Buildings");
		containerPanel.add(selectSuitePage);
		containerPanel.setTabText(1, "Suits");
		
		
		// **************************
//		panels.add(selectBuildingPage);
//		panels.add(selectSuitePage);
//		panels.setTabText(panels.getWidgetIndex(selectBuildingPage), "Building");
//		panels.setTabText(panels.getWidgetIndex(selectSuitePage), "Suite");
//		panels.selectTab(selectBuildingPage);
		// **************************
		
		
//		LoginPanel login = new LoginPanel();
//		login.Show();
		if (History.getToken().isEmpty())
			History.fireCurrentHistoryState();
		else
			History.newItem("");
	};
	
	@Override
	public void onValueChange(ValueChangeEvent<String> event) {
//		Window.alert(event.getValue());
		changePage(event.getValue());
	}

	private void Show(Widget widget) {
//		containerPanel.add(widget);
		containerPanel.selectTab(widget);
	}

	private void changePage(String token) {
		if (token.isEmpty()) {
			selectBuildingPage.Update();
			History.newItem("buildings");
		} else if (token.equals("buildings")) {
			Show(selectBuildingPage);
		} else if (token.equals("suits")) {
//			Window.alert(token);
			selectSuitePage.Update();
			Show(selectSuitePage);
		}
	}
}