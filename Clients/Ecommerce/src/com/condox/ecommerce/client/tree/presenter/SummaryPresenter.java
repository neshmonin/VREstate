package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.condox.ecommerce.client.tree.model.SummaryModel;
import com.google.gwt.user.client.ui.Widget;

public class SummaryPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(SummaryPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private SummaryModel model = null;

	public SummaryPresenter(I_Display display, SummaryModel model) {
		this.model = model;
		this.display = display;
		this.display.setPresenter(this);
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

	public void onPrev() {
		model.prev();
	}

	// ******************
	String user = "";
	String order = "Private Interactive 3D Listing";
	String address = "&lt;none&gt;";
	String mls = "";
	String urlVirtualTour = "";
	String urlMoreInfo = "";
	String sid = "";
	String product = "prl";
	String type = "suite";
	String payment = "CAD4.99";

	public String getSummary() {
		// **********************************
		user = EcommerceTree.get(Field.UserLogin).asString();
		user = "web".equals(user)? "GUEST" : user;

		String productType = EcommerceTree.get(Field.ProductType).asString();
		if (productType == "Layout") {
			product = "3dl";
			order = "Interactive 3D Layout";
			type = "building";
		}
		else if (productType == "ListingPrivate") {
			product = "prl";
			order = "Private Interactive 3D Listing";
			type = "suite";
		}
		else if (productType == "ListingPublic") {
			product = "pul";
			order = "Public Interactive 3D Listing";
			type = "suite";
		}
		
		mls = EcommerceTree.get(Field.MLS).asString();
		urlVirtualTour = EcommerceTree.get(Field.VirtualTourURL).asString();
		urlMoreInfo = EcommerceTree.get(Field.MoreInfoURL).asString();
		address = EcommerceTree.get(Field.Address).asString();

		String html = "";
		html = 	"<table>" + 
					"<tr>" + "<td>" + "User:" + "</td>" + "<td>" + user + "</td>" + "</tr>" + 
					"<tr>" + "<td>" + "Order:" + "</td>" + "<td>" + order + "</td>" + "</tr>" + 
					"<tr>" + "<td>" + "Address:" + "</td>" + "<td>" + address + "</td>" + "</tr>" + 
					"<tr>" + "<td>" + "MLS#:" + "</td>" + "<td>" + mls + "</td>" + "</tr>" + 
					"<tr>" + "<td>" + "Options:" + "</td>" + "<td>" + "" + "</td>" + "</tr>" + 
				"</table>";
		html += "<div style=\"position:relative; left:40px\">" +
				"Virtual Tour URL:" + (urlVirtualTour.isEmpty()? "&lt;none&gt;" : urlVirtualTour) + 
				"<br/>More Info URL:" + (urlMoreInfo.isEmpty()? "&lt;none&gt;" : urlMoreInfo) +
				"</div>";
		html += "<br/><br/><br/><br/>You will be able to preview the order and, if you like it, you will be charged " +
				"$49.99 (paid via secure connection with your credit card)";
		return html;
	}

	public void onNext() {
		EcommerceTree.transitState(State.SummaryReady);
		model.next();
	}
}
