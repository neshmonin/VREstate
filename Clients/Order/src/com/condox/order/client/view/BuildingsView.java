package com.condox.order.client.view;

import com.condox.order.client.context.SuitesContext;
import com.condox.order.client.context.Tree;
import com.condox.order.client.view.factory.IView;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.Widget;

public class BuildingsView extends Composite implements IView {

	private static BuildingsViewUiBinder uiBinder = GWT
			.create(BuildingsViewUiBinder.class);
	@UiField ListBox listBuildings;
	@UiField Button btnNext;

	interface BuildingsViewUiBinder extends UiBinder<Widget, BuildingsView> {
	}

	private Tree tree;
	public BuildingsView(Tree tree) {
		this();
		
		this.tree = tree;
		listBuildings.clear();
		for (int i = 50; i > 0; i--)
			listBuildings.insertItem("Building #" + i, 0);
	
		try {
			Integer index = Integer.valueOf(tree.getValue("selectedBuilding"));
			if (index != null)
				listBuildings.setSelectedIndex(index-1);
		} catch (NumberFormatException e) {
			e.printStackTrace();
		}
	}
	
	public BuildingsView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@UiHandler("listBuildings")
	void onListBuildingsChange(ChangeEvent event) {
//		Log.write("change");
		tree.setValue("selectedBuilding", String.valueOf(listBuildings.getSelectedIndex() + 1));
		btnNext.setEnabled(tree.isValid());
	}
	@UiHandler("btnNext")
	void onBtnNextClick(ClickEvent event) {
		tree.next(new SuitesContext());
	}
}
