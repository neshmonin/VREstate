package com.condox.order.client.wizard.model;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.order.client.I_Model;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.WizardStep;
import com.condox.order.client.wizard.presenter.ListingOptionsPresenter;
import com.condox.order.client.wizard.presenter.LoginPresenter;
import com.condox.order.client.wizard.view.ListingOptionsView;
import com.condox.order.client.wizard.view.LoginView;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.HasWidgets;

public class ListingOptionsModel extends WizardStep {

	public ListingOptionsModel(I_WizardStep parent) {
		super(parent);
		// TODO Auto-generated constructor stub
	}

	/*
	 * private String role = ""; private String uid = ""; private String pwd =
	 * ""; private String sid = "";
	 */

	private String suiteId = "";
	private String mls = "";
	private String urlVirtualTour = "";
	private String urlMoreInfo = "";

	public void setMls(String mls) {
		this.mls = mls;
	}

	public String getMls() {
		return mls;
	}

	public void setSuiteId(String value) {
		suiteId = value;
	}

	public String getSuiteId() {
		return suiteId;
	}

	public void setUrlVirtualTour(String urlVirtualTour) {
		this.urlVirtualTour = urlVirtualTour;
	}

	public String getUrlVirtualTour() {
		return urlVirtualTour;
	}

	public void setUrlMoreInfo(String urlMoreInfo) {
		this.urlMoreInfo = urlMoreInfo;
	}

	public String getUrlMoreInfo() {
		return urlMoreInfo;
	}

	private HasWidgets container = null;
	ListingOptionsPresenter presenter = new ListingOptionsPresenter(
			new ListingOptionsView(), this);

	@Override
	public void go(HasWidgets container) {
		this.container = container;
		updateData();
		super.go(container);
	}

	public void next() {
		getNextStep().go(container);
	}

	@Override
	protected I_WizardStep createNextStep() {
		children.put(this, new SummaryModel(this));
		return children.get(this);
	}

	public void prev() {
		getPrevStep().go(container);
	}

	private SuiteInfo selectedSuite = null;
	private void updateData() {
		String sid = "";
		I_WizardStep step = this;
		while (step != null) {
			try {
				sid = ((LoginModel) step).getUserSid();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				mls = ((MLSModel) step).getMLS();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				selectedSuite = ((SuitesModel) step).getSelected();
			} catch (Exception e) {
				e.printStackTrace();
			}
			step = step.getPrevStep();
		}
		
		if (selectedSuite != null)
			mls = selectedSuite.getMLS();

		String url = "https://vrt.3dcondox.com/vre/data/inventory?";
		url += "mlsId=" + mls;
		url += "&sid=" + sid;
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				// Log.write("HTTP ok:" + response.getStatusCode());
				if (response.getStatusCode() == 200) {
					String json = response.getText();
					JSONObject obj = JSONParser.parseStrict(json).isObject();
					JSONArray arr = obj.get("inventory").isArray();
					SuiteInfo info = new SuiteInfo();
					info.Parse(arr.get(0));
					// display.setData(info);
					urlVirtualTour = info.getVirtualTourURL();
					urlMoreInfo = info.getMoreInfoURL();
					suiteId = String.valueOf(info.getId());
					presenter.go(container);
				} else
					new ErrorMessage(
							"Sorry, this listing cannot be located in the 3D Condo Explorer's Database."
									+ "Please try another MLS#","warning-icon.png").center();
				// CreateTable();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// Log.write("HTTP errot:" + response.getStatusCode());
				// TODO Auto-generated method stub

			}
		});
	}

}
