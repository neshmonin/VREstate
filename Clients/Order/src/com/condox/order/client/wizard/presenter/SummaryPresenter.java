package com.condox.order.client.wizard.presenter;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.order.client.Globals;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.model.BuildingsModel;
import com.condox.order.client.wizard.model.EmailModel;
import com.condox.order.client.wizard.model.LoginModel;
import com.condox.order.client.wizard.model.ProductModel;
import com.condox.order.client.wizard.model.SuitesModel;
import com.condox.order.client.wizard.model.SummaryModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
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
	String sid = "";
	String buildingId = "";
	String suiteId = "25205";
	String product = "pr1";
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
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				if (((ProductModel) step).getListing()) {
					product = "prl";
					type = "suite";
				} else if (((ProductModel) step).getListing()) {
					product = "layout";
					type = "building";
				}
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				buildingId = String.valueOf(((BuildingsModel) step)
						.getSelectedId());
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
				ownerEmail = String.valueOf(((EmailModel) step).getOwnerMail());
			} catch (Exception e) {
				e.printStackTrace();
			}
			step = step.getPrevStep();
		}
		String html = "Summary: <br /> Session id: " + sid + "<br />"
				+ "Selected building: " + buildingId + "<br />"
				+ "Owner mail: " + ownerEmail + "<br />" + "Selected suite: "
				+ suiteId;
		// **********************************
		return html;
	}

	public void onNext() {
		String url = Globals.urlBase
				+ "program?q=register&entity=viewOrder&ownerEmail="
				+ ownerEmail + "&paymentPending=" + payment + "&product="
				+ product + "&propertyType=" + type + "&propertyId=" + suiteId
				+ "&daysValid=20&sid=" + sid;
		url = URL.encode(url);
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				Log.write(response.getText());
//				RootPanel.get("log").clear();
//				RootPanel.get("log").add(new HTML(response.getText()));
			}

			@Override
			public void onError(Request request, Throwable exception) {
				
			}});
	}
}
