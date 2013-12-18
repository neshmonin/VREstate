package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.node.OptionsNode;
import com.google.gwt.user.client.ui.Widget;

public class OptionsPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(OptionsPresenter presenter);

		SuiteInfo setSuiteInfo(SuiteInfo newInfo);
		
		String getVirtualTourUrl();
		
		String getMoreInfoUrl();
		
		Widget asWidget();
	}

	private I_Display display = null;
	private OptionsNode node = null;
	private EcommerceTree tree = null;

	public OptionsPresenter(I_Display newDisplay, OptionsNode newNode) {
		node = newNode;
		tree = node.getTree();
		display = newDisplay;
		display.setPresenter(this);
	}

	@Override
	public void go(I_Container container) {
		// TODO update data
		Data data = tree.getData(Field.SuiteSelected);
		if (data != null) {
			SuiteInfo info = new SuiteInfo();
			info.fromJSONObject(data.asJSONObject());
			display.setSuiteInfo(info);
		}
		container.clear();
		container.add(display);
	}
	
	// Navigation events
	public void onCancel() {
		node.next(Actions.Cancel);
	}

	public void onPrev() {
		Data data = tree.getData(Field.UsingMLS);
		if (data != null) {
			if (data.asBoolean())
				node.next(Actions.UsingMLS);
			else
				node.next(Actions.UsingAddress);
		}
	}
	public void onNext() {
		saveData();
		node.next(Actions.Next);
	}
	
	// Data utils
	private void saveData() {
		tree.setData(Field.VirtualTourUrl, new Data(display.getVirtualTourUrl()));
		tree.setData(Field.MoreInfoUrl, new Data(display.getMoreInfoUrl()));
//		node.setData(Field.Address, new Data(display.getSelectedSuite().getAddress()));
//		node.setData(Field.MLS, new Data(display.getSelectedSuite().getMLS()));
	}
	
	public boolean isUsingMLS() {
		Data data = tree.getData(Field.UsingMLS);
		if (data != null)
			return data.asBoolean();
		else
			return false;
	}

}
