package com.condox.order.client.wizard.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.document.BuildingInfo;
import com.condox.order.client.Globals;
import com.condox.order.client.I_Presenter;
import com.condox.order.client.wizard.I_WizardStep;
import com.condox.order.client.wizard.model.BuildingsModel;
import com.condox.order.client.wizard.model.LoginModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class BuildingsPresenter implements I_Presenter {

	public interface IDisplay {
		void setPresenter(BuildingsPresenter presenter);

		void setData(List<BuildingInfo> data, BuildingInfo selected);

		Widget asWidget();
	}

	private IDisplay display = null;
	private List<BuildingInfo> data = new ArrayList<BuildingInfo>();
	private BuildingInfo selected;
	private BuildingsModel model = null;

	public BuildingsPresenter(IDisplay display, BuildingsModel model) {
		this.display = display;
		this.display.setPresenter(this);
		this.model = model;
	}

	private void updateData() {
		display.setData(null, null);

		String sid = "";
		I_WizardStep step = model;
		while (step != null) {
			try {
				sid = ((LoginModel) step).getUserSid();
			} catch (Exception e) {
				e.printStackTrace();
			}
			step = step.getPrevStep();
		}

		String url = Globals.urlBase;
		url += "data/building?scopeType=address&ad_mu=Toronto&ed=Resale";
		url += "&sid=" + sid;

		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("buildings").isArray();

				Integer id = model.getSelectedId();
//				Log.write("" + id);
				data.clear();
				for (int index = 0; index < arr.size(); index++) {
					BuildingInfo info = new BuildingInfo();
					info.Parse(arr.get(index));
					data.add(info);
					if (id != null)
						if (id.equals(info.getId()))
							selected = info;
				}
				display.setData(data, selected);
			}

			@Override
			public void onError(Request request, Throwable exception) {

			}
		});
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(display.asWidget());
		updateData();
	}

	public void onPrev() {
		model.prev();
	}

	public void onNext() {
		model.setSelected(selected);
		model.setSelectedId(selected.getId());
		// TODO ƒобавить проверку на наличие выделенного дома
		// и при неполадках выводить соотв. сообщение
		model.next();
	}

	public void setSelectedBuilding(BuildingInfo selectedBuilding) {
//		Window.alert("setSelectedBuildingId = " + selectedBuilding.getId());
		selected = selectedBuilding;
		model.setSelected(selected);
		model.setSelectedId(selected.getId());
	}

}
