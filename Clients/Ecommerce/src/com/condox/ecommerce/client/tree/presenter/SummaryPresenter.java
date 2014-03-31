package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.document.BuildingInfo;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
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
		
		void setUserRole(UserRole role);
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(HasWidgets container) {
		// TODO update data
		loadData();
		container.clear();
		container.add(display.asWidget());
		Data data = tree.getData(Field.UserInfo);
		UserInfo info = new UserInfo();
		info.fromJSONObject(data.asJSONObject());
		display.setUserRole(info.getRole());
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
		String s = (data == null) ? "" : data.asString();
		return s.isEmpty() ? "" : s;
	}

	private int getInteger(Field key) {
		Data data = tree.getData(key);
		int i = (data == null) ? -1 : data.asInteger();
		return Math.max(0, i);
	}

	private void loadData() {
		String listing = "<none>";
		String mls = "<none>";
		int price = 0;
		String virtual_tour = "<none>";
		String more_info = "<none>";
		String status = "<none>";

		String suiteName = "";
		String buildingStreet = "";
		String city = "";
		String postal = "";
		String country = "";
		String province = "";
		// -----------------
		Data data = tree.getData(Field.BuildingInfo);
		if (data != null) {
			BuildingInfo info = new BuildingInfo();
			info.fromJSONObject(data.asJSONObject());
			listing = info.getName();
			buildingStreet = info.getStreet();
			city = info.getCity();
			country = info.getCountry();
			postal = info.getPostal();
			province = info.getProvince();
			Log.write(data.asJSONObject().toString());
		}
		data = tree.getData(Field.SuiteInfo);
		if (data != null) {
			SuiteInfo info = new SuiteInfo();
			info.fromJSONObject(data.asJSONObject());
			mls = info.getMLS();
			price = info.getPrice();
			virtual_tour = info.getVirtualTourURL();
			more_info = info.getMoreInfoURL();
			suiteName = info.getName();
			Log.write(data.asJSONObject().toString());
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
		// -----------------
		String html = "";
		html += "<table>";
		
		EcommerceTree.ListingType viewOrderType = EcommerceTree.ListingType.values()[tree.getData(Field.ListingType).asInteger()];
		switch (viewOrderType) {
		case PRIVATE:
			html += row("Listing type:", "PRIVATE");
			break;
		case PUBLIC:
			html += row("Listing type:", "PUBLIC");
			break;
		}
		
		switch (User.role) {
		case Visitor:
			html += row("User:", "GUEST");
			break;
		case SellingAgent:
			data = tree.getData(Field.UserInfo);
			UserInfo info = new UserInfo();
			info.fromJSONObject(data.asJSONObject());
			html += row("User:", info.getPersonalInfo().getLastName() + ", "
					+ info.getPersonalInfo().getFirstName());
			break;
		default:
			break;
		}
		html += row("Order:", "Private Interactive 3D Listing");
		String address = suiteName;
		if (!buildingStreet.isEmpty())
			address += " - " + buildingStreet;
		if (!city.isEmpty())
			address += ", " + city;

		if (!province.isEmpty())
			address += ", " + province;

		if (!postal.isEmpty())
			address += ", " + postal;

		if (!country.isEmpty())
			address += ", " + country;

		html += row("Address:", address);
		html += row("MLS#:", (mls.isEmpty() ? "&lt;none&gt;" : mls));
		html += "</table>";
		html += "Options:";

		// html += "Listing: " + (listing.isEmpty()? "&lt;empty&gt;" : listing)
		// + "<br>";
		// html += "MLS# " + (mls.isEmpty()? "&lt;none&gt;" : mls) + "<br>";
		// html += status.isEmpty()? "" : status + "<br>";

		html += "<div style=\"margin-left:20px\">";
		html += "Third Party Virtual Tour: ";
		if (virtual_tour.isEmpty())
			html += "&lt;none&gt;";
		else
			html += "<a href=\"" + virtual_tour + "\" target = \"_blank\">"
					+ virtual_tour + "</a>";
		html += "</div>";

		html += "<div style=\"margin-left:20px\">";
		html += "More Info Link: ";
		if (more_info.isEmpty())
			html += "&lt;none&gt;";
		else
			html += "<a href=\"" + more_info + "\" target = \"_blank\">"
					+ more_info + "</a>";
		html += "</div>";

		display.setData(html);
	}

	private String row(String key, String value) {
		return "<tr><td width=\"100px\">" + key + "</td><td>" + value
				+ "</td></tr>";
	}

	private void saveData() {
		// node.setData(Field.SuiteId, new
		// Data(display.getSelectedSuite().getId()));
		// node.setData(Field.SuiteName, new
		// Data(display.getSelectedSuite().getName()));
		// node.setData(Field.Address, new
		// Data(display.getSelectedSuite().getAddress()));
		// node.setData(Field.MLS, new
		// Data(display.getSelectedSuite().getMLS()));
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
