package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.order.client.Globals;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.model.BuildingsModel;
import com.condox.order.client.wizard.model.EmailModel;
import com.condox.order.client.wizard.model.ListingOptionsModel;
import com.condox.order.client.wizard.model.LoginModel;
import com.condox.order.client.wizard.model.ProductModel;
import com.condox.order.client.wizard.model.SuitesModel;
import com.condox.order.client.wizard.model.SummaryModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.RootLayoutPanel;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.user.client.ui.Widget;

public class SummaryPresenter implements I_Presenter {

	public static interface I_Display {
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
	public void go(HasWidgets container) {
		container.clear();
		container.add(this.display.asWidget());
	}

	public void onPrev() {
		model.prev();
	}

	// ******************
	String user = "";
	String order = "";
	String address = "&lt;none&gt;";
	String mls = "";
	String urlVirtualTour = "";
	String urlMoreInfo = "";
	String sid = "";
	String buildingId = "";
	SuiteInfo suite = null;
	String product = "prl";
	String type = "suite";
	I_WizardStep step = null;
	String ownerEmail = "andrey.masliuk@3dcondx.com";
	String payment = "CAD4.99";

	public String getSummary() {
		// **********************************
		// TODO —генерировать строку дл€ Summary
		step = model;
		while (step != null) {
			try {
				sid = ((LoginModel) step).getUserSid();
				user = ((LoginModel) step).getUserLogin();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				if (((ProductModel) step).getListingPrivate()) {
					product = "prl";
					order = "Private Interactive 3D Listing";
					type = "suite";
				} else if (((ProductModel) step).getListingShared()) {
					product = "pul";
					order = "Public Interactive 3D Listing";
					type = "suite";
				} else if (((ProductModel) step).getLayout()) {
					order = "Interactive 3D Layout";
					type = "building";
				}
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				buildingId = String.valueOf(((BuildingsModel) step)
						.getSelectedId());
//				address = ((BuildingsModel) step).getSelected().getAddress();
//				address = ((BuildingsModel) step).getSelected().getStreet();
//				address += ", " + ((BuildingsModel) step).getSelected().getCity();
//				address += ", " + ((BuildingsModel) step).getSelected().getPostal();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				if (suite == null)
					suite = ((SuitesModel) step).getSelected();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				if (suite == null)
					suite = ((ListingOptionsModel) step).getSelectedSuite();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				mls = String.valueOf(((ListingOptionsModel) step).getMls());
				urlVirtualTour = ((ListingOptionsModel) step).getUrlVirtualTour();
				urlMoreInfo = String.valueOf(((ListingOptionsModel) step).getUrlMoreInfo());
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				ownerEmail = String.valueOf(((EmailModel) step).getOwnerMail());
			} catch (Exception e) {
				e.printStackTrace();
			}
			step = step.getPrevStep();
		}
		/*String html = "Summary: <br /> Session id: " + sid + "<br />"
				+ "Selected building: " + buildingId + "<br />"
				+ "Owner mail: " + ownerEmail + "<br />" + "Selected suite: "
				+ suiteId;*/
		// **********************************
		user = "web".equals(user)? "GUEST" : user;
//		if (suite != null)
//			address = suite.getName() + " - " + address;
		address = suite.getAddress();
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
		model.next();
		/*String url = Globals.urlBase
				+ "program?q=register&entity=viewOrder&ownerEmail="
				+ ownerEmail + "&paymentPending=" + payment + "&product="
				+ product + "&propertyType=" + type + "&propertyId=" + suiteId
				+ "&daysValid=20&sid=" + sid;
		url = URL.encode(url);
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				Log.write(response.getText());
				// RootPanel.get("log").clear();
				// RootPanel.get("log").add(new HTML(response.getText()));
			}

			@Override
			public void onError(Request request, Throwable exception) {

			}
		});*/
	}
}
