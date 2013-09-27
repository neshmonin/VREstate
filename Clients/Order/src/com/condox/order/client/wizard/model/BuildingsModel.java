package com.condox.order.client.wizard.model;

import com.condox.clientshared.document.BuildingInfo;
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
	private BuildingInfo selected = null;

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

	public void setSelected(BuildingInfo selected) {
		this.selected = selected;
	}

	public BuildingInfo getSelected() {
		return selected;
	}

	ListBox listBox = new ListBox();


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
		super.go(container);
	}

	public void prev() {
//		Log.write("BuildingsModel.prev");
		if (getPrevStep() != null)
			getPrevStep().go(container);
	}

	public void next() {
		getNextStep().go(container);
	}

	@Override
	protected I_WizardStep createNextStep() {
		I_WizardStep step = this;
		while (step != null) {
			try {
				if (((ProductModel)step).getLayout())
					children.put(this, new SummaryModel(this));
				if (((ProductModel)step).getListing())
					children.put(this, new SuitesModel(this));
			} catch (Exception e) {
				e.printStackTrace();
			}
			step = step.getPrevStep();
		}		
		return children.get(this);
	}

	@Override
	public String getNavURL() {
		return "Building";
	}
}
