package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.document.BuildingInfo;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.Data;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.condox.ecommerce.client.tree.I_Contained;
import com.condox.ecommerce.client.tree.I_Container;
import com.condox.ecommerce.client.tree.model.BuildingsModel;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.Widget;

public class BuildingsPresenter implements I_Presenter {

	public interface IDisplay extends I_Contained {
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

		String url = Options.URL_VRT;
		url += "data/building?scopeType=address&ad_mu=Toronto&ed=Resale";
		url += "&sid=" + User.SID;

		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("buildings").isArray();

				Integer id = null;
				Data buildingIdData = EcommerceTree.get(Field.BuildingID);
				if (buildingIdData != null)
					id = buildingIdData.asInteger();
//				Log.write("" + id);
				data.clear();
				for (int index = 0; index < arr.size(); index++) {
					BuildingInfo info = new BuildingInfo();
					info.fromJSONValue(arr.get(index));
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
	public void go(I_Container container) {
		container.clear();
		container.add(display);
		updateData();
	}

	public void onPrev() {
		model.prev();
	}

	public void onNext() {
		EcommerceTree.set(Field.BuildingID, new Data(selected.getId()));
		EcommerceTree.set(Field.Address, new Data(selected.getAddress()));
		
		EcommerceTree.transitState(State.BuildingReady);

		model.next();
	}

	public void setSelectedBuilding(BuildingInfo selectedBuilding) {
//		Window.alert("setSelectedBuildingId = " + selectedBuilding.getId());
		selected = selectedBuilding;
		EcommerceTree.set(Field.BuildingID, new Data(selected.getId()));
	}

}
