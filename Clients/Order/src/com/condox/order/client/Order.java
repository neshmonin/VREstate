package com.condox.order.client;

import com.condox.clientshared.abstractview.Log;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.Wizard;
import com.condox.order.client.wizard.model.LoginModel;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.user.client.ui.RootLayoutPanel;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class Order implements EntryPoint {
	/**
	 * This is the entry point method.
	 */
	public void onModuleLoad() {
		/*RootLayoutPanel rootLayoutPanel = RootLayoutPanel.get();
		
		DockLayoutPanel dockLayoutPanel = new DockLayoutPanel(Unit.EM);
		rootLayoutPanel.add(dockLayoutPanel);
		
		LayoutPanel navigatePanel = new LayoutPanel();
		dockLayoutPanel.addWest(navigatePanel, 15.0);
		
		LayoutPanel containerPanel = new LayoutPanel();
		dockLayoutPanel.add(containerPanel);
		
		Step wizard = new LoginModel(null);	
		Navigator navigator = new Navigator(navigatePanel);
		wizard.go(containerPanel, navigator);*/
		
		MainPage main = new MainPage();
		RootLayoutPanel.get().add(main);
		Wizard wizard = new Wizard(null);
		I_WizardStep start = new LoginModel(null);
		wizard.go(start);
		
		/*BuildingModel building = new BuildingModel();
		FloorModel floor = new FloorModel();
		SuiteModel suite = new SuiteModel();
		Log.write("==");
		building.setValue("value", "building");
		building.log();
		Log.write("==");
		floor.setValue("value", "floor");
		building.addChild(floor);
		floor.log();
		Log.write("==");
		suite.setValue("value", "suite");
		floor.addChild(suite);
		suite.log();*/
		
	}
}
