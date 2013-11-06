package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.I_Contained;
import com.condox.ecommerce.client.tree.I_Container;
import com.condox.ecommerce.client.tree.model.EmailModel;
import com.condox.ecommerce.client.tree.model.ModalMessage;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.user.client.ui.Widget;

public class EmailPresenter implements I_Presenter {

	public String payment = Options.isTestPay()? "CAD1.00" : "CAD49.99";
	
	public static interface I_Display extends I_Contained {
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
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
		Data emailData = EcommerceTree.get(Field.Email);
		if (emailData != null)
			display.setOwnerEmail(emailData.asString());
	}

	public void onPrev() {
		model.prev();
	}

	public void onNext() {
		// **********************************
		String ownerEmail = "";
		String product = "prl";
		String type = "suite";
		int suiteId = 0;
		String moreInfoURL = "";
		String virtualTourURL = "";
		String mlsNum = "";

		String productType = EcommerceTree.get(Field.ProductType).asString();
		if (productType == "Layout") {
			product = "3dl";
			type = "building";
		}
		else if (productType == "ListingPrivate") {
			product = "prl";
			type = "suite";
		}
		else if (productType == "ListingPublic") {
			product = "pul";
			type = "suite";
		}
		
		suiteId = EcommerceTree.get(Field.SuiteId).asInteger();
	
		mlsNum = EcommerceTree.get(Field.MLS).asString();
		virtualTourURL = EcommerceTree.get(Field.VirtualTourURL).asString();
		moreInfoURL = EcommerceTree.get(Field.MoreInfoURL).asString();
		ownerEmail = EcommerceTree.get(Field.Email).asString();

		//**********************************************
		final String mail = ownerEmail;
		String url = Options.URL_VRT
				+ "program?q=register&entity=viewOrder&ownerEmail="
				+ ownerEmail + "&paymentPending=" + payment + "&product="
				+ product + "&propertyType=" + type + "&propertyId=" + suiteId
				+ "&mls_id=" + mlsNum + "&mls_url=" + moreInfoURL + "&evt_url=" + virtualTourURL 
				+ "&daysValid=1&sid=" + User.SID;
		url = URL.encode(url);
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
//				Log.write(response.getText());
				// TODO Auto-generated method stub
				if (response.getStatusCode() == 200) {
					// model.next();
					String msg = 	"The order has been submitted successfuly." +
									"You have 10 minutes to complete it" +
									"<br/>Please check you email account " +
									"<center><b>" + mail + "</b></center> " +
									"and follow the instructions in the message " +
									"<b>Order of interactive 3D product - 3D Condo Explorer</b>" +
									"<br/>" +
									"<i>Make sure you checked the spam folder too.</i>";
					new ModalMessage(msg, "10Minutes.jpg").center();
					//HTML html = new HTML();
					//html.setHTML(msg);
					//PopupPanel popup = new PopupPanel();
					//popup.setWidget(html);
					//popup.setSize("300px", "140px");
					//popup.setModal(true);
					//popup.setGlassEnabled(true);
					//popup.setAutoHideEnabled(true);
					//popup.center();
					EcommerceTree.cancel();
					
				} else {
					new ModalMessage("Sorry, we are currently experiencing some server-side problems. Please try to re-submit your order again later","error-icon.jpg").center();
				}
				
			}
			@Override
			public void onError(Request request, Throwable exception) {

			}

		});
	}

	public void setOwnerEmail(String ownerEmail) {
		EcommerceTree.set(Field.Email, new Data(ownerEmail));
	}
}
