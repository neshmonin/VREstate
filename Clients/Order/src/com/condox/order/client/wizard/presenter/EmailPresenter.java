package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.Wizard;
import com.condox.order.client.wizard.model.EmailModel;
import com.condox.order.client.wizard.model.ErrorMessage;
import com.condox.order.client.wizard.model.ListingOptionsModel;
import com.condox.order.client.wizard.model.LoginModel;
import com.condox.order.client.wizard.model.ProductModel;
import com.condox.order.client.wizard.model.SuitesModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.Widget;

public class EmailPresenter implements I_Presenter {

	public String payment = Options.isTestPay()? "CAD1.00" : "CAD49.99";
	
	public static interface I_Display {
		void setPresenter(EmailPresenter presenter);

		void setOwnerEmail(String ownerEmail);

		Widget asWidget();
	}

	private I_Display display = null;
	private EmailModel model = null;

	public EmailPresenter(I_Display display, EmailModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(this.display.asWidget());
		display.setOwnerEmail(model.getOwnerMail());
	}

	public void onPrev() {
		model.prev();
	}

	public void onNext() {
		// **********************************
		I_WizardStep step = model;
		String ownerEmail = "";
		String product = "prl";
		String type = "suite";
		String suiteId = "";
		String sid = "";
		while (step != null) {
			switch (step.getStepType()) {
			case LoginModel:
				sid = ((LoginModel) step).getUserSid();
				break;
			case ProductModel:
				ProductModel productModelStep = (ProductModel) step;
				if (productModelStep.getListingPrivate()) {
					product = "prl";
//					order = "Private Interactive 3D Listing";
					type = "suite";
				} else if (productModelStep.getListingShared()) {
					product = "pul";
//					order = "Public Interactive 3D Listing";
					type = "suite";
				} else if (productModelStep.getLayout()) {
//					order = "Interactive 3D Layout";
					type = "building";
				}
				break;
		//	case BuildingsModel:
		//		buildingId = String.valueOf(((BuildingsModel) step)
		//				.getSelectedId());
		//		address = ((BuildingsModel) step).getSelected().getAddress();
		//		address = ((BuildingsModel) step).getSelected().getStreet();
		//		address += ", " + ((BuildingsModel) step).getSelected().getCity();
		//		address += ", " + ((BuildingsModel) step).getSelected().getPostal();
		//		break;
			case SuitesModel:
				suiteId = String.valueOf(((SuitesModel) step)
						.getSelected().getId());
				break;
			case ListingOptionsModel:
				suiteId = String.valueOf(((ListingOptionsModel) step).getSuiteId());
				//urlVirtualTour = ((ListingOptionsModel) step).getUrlVirtualTour();
				//urlMoreInfo = String.valueOf(((ListingOptionsModel) step).getUrlMoreInfo());
				break;
			case EmailModel:
				ownerEmail = String.valueOf(((EmailModel) step).getOwnerMail());
				break;
			}
			step = step.getPrevStep();
		}
		//**********************************************
		final String mail = ownerEmail;
		String url = Options.URL_VRT
				+ "program?q=register&entity=viewOrder&ownerEmail="
				+ ownerEmail + "&paymentPending=" + payment + "&product="
				+ product + "&propertyType=" + type + "&propertyId=" + suiteId
				+ "&daysValid=20&sid=" + sid;
		url = URL.encode(url);
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
//				Log.write(response.getText());
				// TODO Auto-generated method stub
				if (response.getStatusCode() == 200) {
					// model.next();
					String msg = "Please check your email account";
					msg += "<br/><center>" + mail + "</center>";
					msg += "(make sure you check the spam folder too).";
					msg += "<br/>";
					msg += "<br/>To complete the order you'll need to follow the" +
					" instructions from a message named" +
					"<br/> \"Order of interactive 3D product - 3D Condo Explorer\"";
					HTML html = new HTML();
					html.setHTML(msg);
					PopupPanel popup = new PopupPanel();
					popup.setWidget(html);
					popup.setSize("300px", "140px");
					popup.setModal(true);
					popup.setGlassEnabled(true);
					popup.setAutoHideEnabled(true);
					popup.center();
					Wizard.cancel();
					
				} else {
					new ErrorMessage("Sorry, we are currently experiencing some server-side problems. Please try to re-submit your order again later","error-icon.jpg").center();
				}
				
			}
			@Override
			public void onError(Request request, Throwable exception) {

			}

		});
	}

	public void setOwnerEmail(String ownerEmail) {
		model.setOwnerMail(ownerEmail);
	}
}
