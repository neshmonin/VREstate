package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.document.BuildingInfo;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class SummaryPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(SummaryPresenter presenter);

		void setData(String data);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(HasWidgets container) {
		// TODO update data
		loadData();
		container.clear();
		container.add(display.asWidget());
	}
	
	// Navigation events
	public void onCancel() {
		tree.next(Actions.Cancel);
	}

	public void onPrev() {
		tree.next(Actions.Prev);
	}
	
	public void onNext() {
		saveData();
		tree.next(Actions.Next);
	}
	
	// Data utils
	private String getString(Field key) {
		Data data = tree.getData(key);
		String s = (data == null)? "" : data.asString();
		return s.isEmpty()? "" : s;
	}
	
	private int getInteger(Field key) {
		Data data = tree.getData(key);
		int i = (data == null)? -1 : data.asInteger();
		return Math.max(0, i);
	}
	
	private void loadData() {
		String listing = "<none>";
		String mls = "<none>";
		int price = 0;
		String virtual_tour = "<none>";
		String more_info = "<none>";
		String status = "<none>";
		//-----------------
		Data data = tree.getData(Field.BuildingInfo);
		if (data != null) {
			BuildingInfo info = new BuildingInfo();
			info.fromJSONObject(data.asJSONObject());
			listing = info.getName();
		}
		data = tree.getData(Field.SuiteInfo);
		if (data != null) {
			SuiteInfo info = new SuiteInfo();
			info.fromJSONObject(data.asJSONObject());
			mls = info.getMLS();
			price = info.getPrice();
			virtual_tour = info.getVirtualTourURL();
			more_info = info.getMoreInfoURL();
			switch (info.getStatus()) {
			case AvailableRent:
				status = "For rent - price $" + price + "/m";
				break;
			case AvailableResale:
				status = "For sale - price $" + price;
				break;
			default:
				break;
	}
		}
		//-----------------
		
		String html = ""; 
		html += "Listing: " + (listing.isEmpty()? "&lt;empty&gt;" : listing) + "<br>";
		html +=	"MLS# " + (mls.isEmpty()? "&lt;none&gt;" : mls)  + "<br>";
		html += status.isEmpty()? "" : status + "<br>";
		html +=	"Third Party Virtual Tour<br>";
		html += "<div style=\"margin-left:20px\">";
		if (virtual_tour.isEmpty())
			html += "&lt;none&gt;";
		else
			html += "<a href=\"" + virtual_tour + "\" target = \"_blank\">" + virtual_tour + "</a>";
		html += "</div>";

		html +=	"More Info Link<br>";
		html += "<div style=\"margin-left:20px\">";
		if (more_info.isEmpty())
			html += "&lt;none&gt;";
		else
			html += "<a href=\"" + more_info + "\" target = \"_blank\">" + more_info + "</a>";
		html += "</div>";
		
		display.setData(html);
		
	}
	
	private void saveData() {
//		node.setData(Field.SuiteId, new Data(display.getSelectedSuite().getId()));
//		node.setData(Field.SuiteName, new Data(display.getSelectedSuite().getName()));
//		node.setData(Field.Address, new Data(display.getSelectedSuite().getAddress()));
//		node.setData(Field.MLS, new Data(display.getSelectedSuite().getMLS()));
	}

	@Override
	public void setView(Composite view) {
		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

}
