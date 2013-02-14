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
		RootLayoutPanel rootLayoutPanel = RootLayoutPanel.get();
		rootLayoutPanel.setStyleName("my-body");
		rootLayoutPanel.add(mainPanel);
//		containerPanel.add(dockLayoutPanel, "New Widget", false);
		History.addValueChangeHandler(this);
		Log.write("onModuleLoad -> Options.Init(this);");
		Options.Init(this);
		
		LayoutPanel layoutPanel = new LayoutPanel();
		mainPanel.addNorth(layoutPanel, 50.0);
		
		FlowPanel flowPanel = new FlowPanel();
		flowPanel.setStyleName("my-menu");
		mainPanel.addWest(flowPanel, 250.0);
		
		Button btnNewButton = new Button("3d products");
		btnNewButton.setStyleName("menu-item");
		flowPanel.add(btnNewButton);
		btnNewButton.setSize("100%", "35px");
		
		Button btndCondoExplorer = new Button("3d condo explorer");
		btndCondoExplorer.setStyleName("menu-item");
		flowPanel.add(btndCondoExplorer);
		btndCondoExplorer.setSize("100%", "35px");
		
		Button btnFuturePage = new Button("future page");
		btnFuturePage.setStyleName("menu-item");
		flowPanel.add(btnFuturePage);
		btnFuturePage.setSize("100%", "35px");
		
		Button btnFuturePage_1 = new Button("future page");
		btnFuturePage_1.setStyleName("menu-item");
		flowPanel.add(btnFuturePage_1);
		btnFuturePage_1.setSize("100%", "35px");
		
		Button btnFuturePage_2 = new Button("future page");
		btnFuturePage_2.setStyleName("menu-item");
		flowPanel.add(btnFuturePage_2);
		btnFuturePage_2.setSize("100%", "35px");
		
		Button btnFuturePage_3 = new Button("future page");
		btnFuturePage_3.setStyleName("menu-item");
		flowPanel.add(btnFuturePage_3);
		btnFuturePage_3.setSize("100%", "35px");
		
		Button btnContactUs = new Button("contact us");
		btnContactUs.setStyleName("menu-item");
		flowPanel.add(btnContactUs);
		btnContactUs.setSize("100%", "35px");
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