package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.order.client.Globals;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.Wizard;
import com.condox.order.client.wizard.model.BuildingsModel;
import com.condox.order.client.wizard.model.EmailModel;
import com.condox.order.client.wizard.model.ListingOptionsModel;
import com.condox.order.client.wizard.model.LoginModel;
import com.condox.order.client.wizard.model.ProductModel;
import com.condox.order.client.wizard.model.SuitesModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.Widget;

public class EmailPresenter implements I_Presenter {

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
		// TODO —генерировать строку дл€ Summary
		I_WizardStep step = model;
		String ownerEmail = "";
		String payment = "$49.99";
		String product = "";
		String type = "";
		String suiteId = "";
		String sid = "";
		while (step != null) {
			try {
				sid = ((LoginModel) step).getUserSid();
//				user = ((LoginModel) step).getUserLogin();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				if (((ProductModel) step).getListingPrivate()) {
					product = "prl";
//					order = "Private Interactive 3D Listing";
					type = "suite";
				} else if (((ProductModel) step).getListingShared()) {
					product = "pul";
//					order = "Public Interactive 3D Listing";
					type = "suite";
				} else if (((ProductModel) step).getLayout()) {
//					order = "Interactive 3D Layout";
					type = "building";
				}
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
//				buildingId = String.valueOf(((BuildingsModel) step)
//						.getSelectedId());
//				address = ((BuildingsModel) step).getSelected().getAddress();
//				address = ((BuildingsModel) step).getSelected().getStreet();
//				address += ", " + ((BuildingsModel) step).getSelected().getCity();
//				address += ", " + ((BuildingsModel) step).getSelected().getPostal();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				suiteId = String.valueOf(((SuitesModel) step)
						.getSelectedIndex());
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
//				mls = String.valueOf(((ListingOptionsModel) step).getMls());
//				urlVirtualTour = ((ListingOptionsModel) step).getUrlVirtualTour();
//				urlMoreInfo = String.valueOf(((ListingOptionsModel) step).getUrlMoreInfo());
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
		//**********************************************
		String url = Globals.urlBase
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
				
			}
			@Override
			public void onError(Request request, Throwable exception) {

			}

		});
		// model.next();
		String msg = "Please check your email account";
		msg += "<br/><center>" + ownerEmail + "</center>";
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
	}

	public void setOwnerEmail(String ownerEmail) {
		model.setOwnerMail(ownerEmail);
	}
}
