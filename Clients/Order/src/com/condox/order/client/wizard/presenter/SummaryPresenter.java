package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.document.SuiteInfo;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.model.BuildingsModel;
import com.condox.order.client.wizard.model.EmailModel;
import com.condox.order.client.wizard.model.ListingOptionsModel;
import com.condox.order.client.wizard.model.LoginModel;
import com.condox.order.client.wizard.model.ProductModel;
import com.condox.order.client.wizard.model.SuitesModel;
import com.condox.order.client.wizard.model.SummaryModel;
import com.google.gwt.user.client.ui.HasWidgets;
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
	String order = "Private Interactive 3D Listing";
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
		step = model;
		while (step != null) {
			switch (step.getStepType()) {
			case LoginModel:
				sid = ((LoginModel) step).getUserSid();
				user = ((LoginModel) step).getUserLogin();
				break;
			case ProductModel:
				ProductModel productModelStep = (ProductModel) step;
				if (productModelStep.getListingPrivate()) {
					product = "prl";
					order = "Private Interactive 3D Listing";
					type = "suite";
				} else if (productModelStep.getListingShared()) {
					product = "pul";
					order = "Public Interactive 3D Listing";
					type = "suite";
				} else if (productModelStep.getLayout()) {
					order = "Interactive 3D Layout";
					type = "building";
				}
				break;
			case BuildingsModel:
				buildingId = String.valueOf(((BuildingsModel) step)
						.getSelectedId());
//				address = ((BuildingsModel) step).getSelected().getAddress();
//				address = ((BuildingsModel) step).getSelected().getStreet();
//				address += ", " + ((BuildingsModel) step).getSelected().getCity();
//				address += ", " + ((BuildingsModel) step).getSelected().getPostal();
				break;
			case SuitesModel:
				if (suite == null)
					suite = ((SuitesModel) step).getSelected();
				break;
			case ListingOptionsModel:
				if (suite == null)
					suite = ((ListingOptionsModel) step).getSelectedSuite();
				mls = String.valueOf(((ListingOptionsModel) step).getMls());
				urlVirtualTour = ((ListingOptionsModel) step).getUrlVirtualTour();
				urlMoreInfo = String.valueOf(((ListingOptionsModel) step).getUrlMoreInfo());
				break;
			case EmailModel:
				ownerEmail = String.valueOf(((EmailModel) step).getOwnerMail());
				break;
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
