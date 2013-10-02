package com.condox.order.client;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.Wizard;
import com.condox.order.client.wizard.model.LoginModel;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.user.client.History;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class Order implements EntryPoint, ValueChangeHandler<String> {
	/**
	 * This is the entry point method.
	 */
	public void onModuleLoad() {
		Options.Init();
		History.addValueChangeHandler(this);
		/*
		 * RootLayoutPanel rootLayoutPanel = RootLayoutPanel.get();
		 * 
		 * DockLayoutPanel dockLayoutPanel = new DockLayoutPanel(Unit.EM);
		 * rootLayoutPanel.add(dockLayoutPanel);
		 * 
		 * LayoutPanel navigatePanel = new LayoutPanel();
		 * dockLayoutPanel.addWest(navigatePanel, 15.0);
		 * 
		 * LayoutPanel containerPanel = new LayoutPanel();
		 * dockLayoutPanel.add(containerPanel);
		 * 
		 * Step wizard = new LoginModel(null); Navigator navigator = new
		 * Navigator(navigatePanel); wizard.go(containerPanel, navigator);
		 */

		// MainPage main = new MainPage();
		// RootLayoutPanel.get().add(main);

		// Wizard wizard = new Wizard(null);
		// I_WizardStep start = new LoginModel(null);
		// wizard.go(start);

		// ContactEditor editor = new ContactEditor(new Person());
		/*
		 * PersonEditingWorkflow editor = new PersonEditingWorkflow(); Person
		 * person = new Person(); editor.edit(person);
		 */

		/*
		 * BuildingModel building = new BuildingModel(); FloorModel floor = new
		 * FloorModel(); SuiteModel suite = new SuiteModel(); Log.write("==");
		 * building.setValue("value", "building"); building.log();
		 * Log.write("=="); floor.setValue("value", "floor");
		 * building.addChild(floor); floor.log(); Log.write("==");
		 * suite.setValue("value", "suite"); floor.addChild(suite); suite.log();
		 */

	}

	@Override
	public void onValueChange(ValueChangeEvent<String> event) {
		String token = event.getValue();
		Log.write(token);
		if ("login".equals(token))
			startWizard();
		else if("orderNow".equals(token))
			startWizard();
		History.newItem("", false);
	}
	
	private void startWizard() {
		Wizard wizard = new Wizard(null);
		I_WizardStep start = new LoginModel(null);
		wizard.go(start);
	}
}
