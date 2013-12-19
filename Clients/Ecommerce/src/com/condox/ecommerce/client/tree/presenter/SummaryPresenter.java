package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.document.SuiteInfo.Status;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.node.SummaryNode;
import com.google.gwt.user.client.ui.Widget;

public class SummaryPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(SummaryPresenter presenter);

		void setData(String data);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private SummaryNode node = null;

	public SummaryPresenter(I_Display newDisplay, SummaryNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		// TODO update data
		loadData();
		container.clear();
		container.add(display);
	}
	
	// Navigation events
	public void onCancel() {
		node.next(Actions.Cancel);
	}

	public void onPrev() {
		node.next(Actions.Prev);
	}
	
	public void onNext() {
		saveData();
		node.next(Actions.Next);
	}
	
	// Data utils
	private String getString(Field key) {
		Data data = node.getTree().getData(key);
		String s = (data == null)? "" : data.asString();
		return s.isEmpty()? "" : s;
	}
	
	private int getInteger(Field key) {
		Data data = node.getTree().getData(key);
		int i = (data == null)? -1 : data.asInteger();
		return Math.max(0, i);
	}
	
	private void loadData() {
		String listing = getString(Field.BuildingName);
		String mls = getString(Field.SuiteMLS);
		int price = getInteger(Field.SuitePrice);
		String virtual_tour = getString(Field.VirtualTourUrl);
		String more_info = getString(Field.MoreInfoUrl);
		String status = "";
		//-----------------
		Data data = node.getTree().getData(Field.SuiteSelected);
		if (data != null) {
			SuiteInfo info = new SuiteInfo();
			info.fromJSONObject(data.asJSONObject());
			mls = info.getMLS();
			price = info.getPrice();
			virtual_tour = info.getVirtualTourURL();
			more_info = info.getMoreInfoURL();
			switch (info.getStatus()){
				case AvailableRent:
					status = "Status: Rent";
					break;
				case AvailableResale:
					status = "Status: Sale";
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
		html += "Price  " + (price == 0? "&lt;empty&gt;" : "$" + String.valueOf(price)) + "<br>";

		html +=	"Third Party Virtual Tour<br>";
		html += "<div style=\"margin-left:20px\">";
		if (virtual_tour.isEmpty())
			html += "&lt;none&gt;";
		else
			html += "<a href=\"" + virtual_tour + "\">" + virtual_tour + "</a>";
		html += "</div>";

		html +=	"More Info Link<br>";
		html += "<div style=\"margin-left:20px\">";
		if (more_info.isEmpty())
			html += "&lt;none&gt;";
		else
			html += "<a href=\"" + more_info + "\">" + more_info + "</a>";
		html += "</div>";
		
		display.setData(html);
		
	}
	
	private void saveData() {
//		node.setData(Field.SuiteId, new Data(display.getSelectedSuite().getId()));
//		node.setData(Field.SuiteName, new Data(display.getSelectedSuite().getName()));
//		node.setData(Field.Address, new Data(display.getSelectedSuite().getAddress()));
//		node.setData(Field.MLS, new Data(display.getSelectedSuite().getMLS()));
	}

}
