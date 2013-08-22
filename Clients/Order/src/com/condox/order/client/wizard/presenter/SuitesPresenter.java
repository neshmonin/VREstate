package com.condox.order.client.wizard.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.model.LoginModel;
import com.condox.order.client.wizard.model.BuildingsModel;
import com.condox.order.client.wizard.model.SuitesModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class SuitesPresenter implements I_Presenter {

	public interface IDisplay {
		void setPresenter(SuitesPresenter presenter);

		void setData(List<SuiteInfo> data);

		Widget asWidget();

		String getSuiteName();

		String getSuiteFloorplan();

		SuiteInfo getSelectedSuite();
	}

	private IDisplay display = null;
	private List<SuiteInfo> data = new ArrayList<SuiteInfo>();
	private SuitesModel model = null;

	public SuitesPresenter(IDisplay display, SuitesModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	private void updateData() {
		String sid = "";
		String buildingId = "";
		I_WizardStep step = model;
		while (step != null) {
			try {
				sid = ((LoginModel) step).getUserSid();
			} catch (Exception e) {
				e.printStackTrace();
			}
			try {
				buildingId = String.valueOf(((BuildingsModel)step).getSelectedId());
			} catch (Exception e) {
				e.printStackTrace();
			}
			step = step.getPrevStep();
		}
		
//		String url = "https://vrt.3dcondox.com/data/inventory?";
		String url = "https://vrt.3dcondox.com/vre/data/inventory?";
		url += "building=" + buildingId;
		url += "&sid=" + sid;
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				// Log.write("answer:" + response.getText());
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("inventory").isArray();
				for (int index = 0; index < arr.size(); index++) {
					SuiteInfo info = new SuiteInfo();
					info.Parse(arr.get(index));
					data.add(info);
				}
				display.setData(data);
				// CreateTable();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub

			}
		});
	}


	public String getSelectedBuildingStreet() {
		return "Floor:";//tree.getString("buildings.selected.street");
	}

	@Override
	public void go(HasWidgets container) {
		// TODO Auto-generated method stub
		container.clear();
		container.add(display.asWidget());
		updateData();
	}

	public void onPrev() {
		model.prev();
	}

}
