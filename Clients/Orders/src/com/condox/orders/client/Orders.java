package com.condox.orders.client;

import com.condox.orders.client.pages.buildings.Building;
import com.condox.orders.client.pages.buildings.SelectBuilding;
import com.condox.orders.client.pages.suits.SelectSuite;
import com.condox.orders.client.pages.suits.Suite;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.dom.client.Style.Unit;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.user.client.History;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.RootLayoutPanel;
import com.google.gwt.user.client.ui.TabLayoutPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.user.client.ui.DockLayoutPanel;
import com.google.gwt.user.client.ui.FlowPanel;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.SimplePanel;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.MenuBar;
import com.google.gwt.user.client.ui.MenuItem;
import com.google.gwt.user.client.Command;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.LayoutPanel;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.user.client.ui.PushButton;
import com.google.gwt.user.client.ui.Image;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.dom.client.ClickEvent;

public class Orders implements EntryPoint, ValueChangeHandler<String>  {
	
	private DockLayoutPanel mainPanel = new DockLayoutPanel(Unit.PX);
	private TabLayoutPanel containerPanel = new TabLayoutPanel(0.0, Unit.EM);
	private SelectBuilding page_buildings = /*null;*/new SelectBuilding();
	private SelectSuite page_suits = /*null;*/new SelectSuite();
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
		
		PushButton menu0 = new PushButton("3d products");
		menu0.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Window.open("https://vrt.3dcondox.com/vre/orders/index.html", "_blank", "");
			}
		});
		menu0.setStyleName("my-menu-item");
		layoutPanel_1.add(menu0);
		layoutPanel_1.setWidgetTopHeight(menu0, 50.0, Unit.PX, 45.0, Unit.PX);
		layoutPanel_1.setWidgetLeftRight(menu0, 10.0, Unit.PX, 12.0, Unit.PX);
		
		PushButton menu1 = new PushButton("3d condo explorer");
		menu1.addClickHandler(new ClickHandler() {
			public void onClick(ClickEvent event) {
				Window.open("http://www.3dcondox.com/", "_blank", "");
			}
		});
		menu1.setStyleName("my-menu-item");
		layoutPanel_1.add(menu1);
		layoutPanel_1.setWidgetLeftWidth(menu1, 10.0, Unit.PX, 178.0, Unit.PX);
		layoutPanel_1.setWidgetTopHeight(menu1, 95.0, Unit.PX, 45.0, Unit.PX);
		
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
		layoutPanel_1.setWidgetTopHeight(menu2, 145.0, Unit.PX, 45.0, Unit.PX);
		mainPanel.add(containerPanel);
	}

	public void LoginUser() {
		Log.write("LoginUser -> User.Login(this);");
		User.Login(this);
	};

	public void StartGE() {
//		RootLayoutPanel.get().add(containerPanel);
		containerPanel.add(page_buildings);
		containerPanel.setTabText(0, "Buildings");
		containerPanel.add(page_suits);
		containerPanel.setTabText(1, "Suits");
		
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
			page_buildings.Update();
			History.newItem("buildings");
		} else if (token.equals("buildings")) {
			Show(page_buildings);
		} else if (token.equals("suits")) {
			page_suits.Update();
			Show(page_suits);
		}
	}
}