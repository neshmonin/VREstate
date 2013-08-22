package com.condox.order.client.wizard.model;

import com.condox.clientshared.abstractview.Log;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.WizardStep;
import com.condox.order.client.wizard.presenter.BuildingsPresenter;
import com.condox.order.client.wizard.view.BuildingsView;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.ListBox;

public class BuildingsModel extends WizardStep {

	public BuildingsModel(I_WizardStep parent) {
		super(parent);
	}

	private int selectedIndex = 0;

	public int getSelectedId() {
		return selectedIndex;
	}

	public void setSelectedId(int id) {
		selectedIndex = id;
	}

	/*
	 * @Override public boolean isValid() { boolean valid = true; // Selected
	 * must be one and only one! int selected = 0; for (int i = 0; i <
	 * listBox.getItemCount(); i++) if (listBox.isItemSelected(i)) selected++;
	 * valid &= (selected == 1); return valid; }
	 */

	ListBox listBox = new ListBox();

	/**
	 * @wbp.parser.entryPoint
	 */
	/*
	 * @Override public void go(final HasWidgets container, final Navigator
	 * navigator) {
	 * 
	 * // GWT.log("BuildingsModel.go()"); HTMLPanel panel = new HTMLPanel("");
	 * panel.setSize("100%", "100%");
	 * 
	 * VerticalPanel verticalPanel = new VerticalPanel();
	 * panel.add(verticalPanel); verticalPanel.setSize("100%", "100%");
	 * verticalPanel
	 * .setHorizontalAlignment(HasHorizontalAlignment.ALIGN_CENTER);
	 * verticalPanel.setVerticalAlignment(HasVerticalAlignment.ALIGN_MIDDLE);
	 * 
	 * VerticalPanel verticalPanel_1 = new VerticalPanel();
	 * verticalPanel_1.setSpacing(10);
	 * verticalPanel_1.setVerticalAlignment(HasVerticalAlignment.ALIGN_BOTTOM);
	 * verticalPanel.add(verticalPanel_1);
	 * 
	 * HTML htmlNewHtml = new HTML("Selected building:", true);
	 * verticalPanel_1.add(htmlNewHtml); listBox.setMultipleSelect(true);
	 * listBox.clear(); listBox.addItem("Building #1");
	 * listBox.addItem("Building #2"); listBox.addItem("Building #3");
	 * listBox.addItem("Building #4"); listBox.addItem("Building #5");
	 * listBox.addItem("Building #6"); listBox.addItem("Building #7");
	 * listBox.addItem("Building #8"); listBox.addItem("Building #9");
	 * listBox.addItem("Building #10"); try {
	 * listBox.setItemSelected(selectedIndex, true); } catch (Exception e) { //
	 * TODO Auto-generated catch block e.printStackTrace(); }
	 * verticalPanel_1.add(listBox); listBox.setSize("100%", "168px");
	 * verticalPanel_1.setCellWidth(listBox, "100%");
	 * verticalPanel_1.setCellHeight(listBox, "100%");
	 * listBox.setVisibleItemCount(5);
	 * 
	 * HorizontalPanel horizontalPanel = new HorizontalPanel();
	 * horizontalPanel.setSpacing(5); verticalPanel_1.add(horizontalPanel);
	 * 
	 * Button btnNewButton = new Button("New button");
	 * horizontalPanel.add(btnNewButton); btnNewButton.setText("<< Prev");
	 * btnNewButton.addClickHandler(new ClickHandler() { public void
	 * onClick(ClickEvent event) { // prev view prev(container, navigator); }
	 * }); btnNewButton.setWidth("100%");
	 * 
	 * Button btnNewButton_1 = new Button("New button");
	 * btnNewButton_1.addClickHandler(new ClickHandler() { public void
	 * onClick(ClickEvent event) { selectedIndex = listBox.getSelectedIndex();
	 * // next view next(container, navigator); } });
	 * btnNewButton_1.setText("Next >>"); horizontalPanel.add(btnNewButton_1);
	 * btnNewButton_1.setWidth("100%");
	 * 
	 * container.clear(); container.add(panel);
	 * 
	 * // new LoginPresenter().go(container, this); //
	 * PresenterFactory.createPresenter(this).go(); super.go(container,
	 * navigator); }
	 */

	/*
	 * private void prev(HasWidgets container, Navigator navigator) {
	 * selectedIndex = listBox.getSelectedIndex(); I_Step prevStep = getPrev();
	 * if (prevStep != null) prevStep.go(container, navigator); }
	 * 
	 * private void next(HasWidgets container, Navigator navigator) { if
	 * (isValid()) { // next view I_Step nextStep = getNext(); if (nextStep ==
	 * null) addChild(new SuitesModel(this)); nextStep = getNext();
	 * nextStep.go(container, navigator); } else
	 * Window.alert("Not valid! Please, verify!"); }
	 */

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + selectedIndex;
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		BuildingsModel other = (BuildingsModel) obj;
		if (selectedIndex != other.selectedIndex)
			return false;
		return true;
	}

	/*
	 * @Override public String getCaption() { try { return " " +
	 * listBox.getItemText(selectedIndex) + " >> "; } catch (Exception e) { //
	 * TODO Auto-generated catch block e.printStackTrace(); } return null; }
	 */
	@Override
	public void go(HasWidgets container) {
		// super.go(container, navigator);
//		Window.alert("BModel id=" + selectedIndex);
		this.container = container;
		new BuildingsPresenter(new BuildingsView(), this).go(container);
	}

	public void prev() {
//		Log.write("BuildingsModel.prev");
		if (getPrevStep() != null)
			getPrevStep().go(container);
	}

	public void next() {
//		Log.write("BuildingsModel.next");
		getNextStep().go(container);
	}

	@Override
	protected I_WizardStep createNextStep() {
		Log.write("new SuitesModel");
		children.put(this, new SuitesModel(this));
		return children.get(this);
	}
}
