package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.document.BuildingInfo;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.node.BuildingsNode;
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
		
		String getFilterCity();
		void setFilterCity(String city);
		

		Widget asWidget();
	}

	private IDisplay display = null;
	private List<BuildingInfo> data = new ArrayList<BuildingInfo>();
	private BuildingInfo selected;
	private BuildingsNode node = null;
	private EcommerceTree tree = null;

	public BuildingsPresenter(IDisplay display, BuildingsNode node) {
		this.display = display;
		this.display.setPresenter(this);
		this.node = node;
		tree = node.getTree();
	}

	public void updateData() {
		tree.setData(Field.FILTERING_BY_CITY, new Data(display.getFilterCity()));
		display.setData(null, null);

		String url = Options.URL_VRT;
		url += "data/building?scopeType=address&ad_mu=" + display.getFilterCity() + "&ed=Resale";
		url += "&sid=" + User.SID;

		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("buildings").isArray();

				Integer id = null;
				Data buildingIdData = tree.getData(Field.BuildingId);
				if (buildingIdData != null)
					id = buildingIdData.asInteger();
//				Log.write("" + id);
				data.clear();
				for (int index = 0; index < arr.size(); index++) {
					BuildingInfo info = new BuildingInfo();
					info.fromJSONObject(arr.get(index).isObject());
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
		Data data = tree.getData(Field.FILTERING_BY_CITY);
		if (data != null) {
			String filterCity = data.Value;
			display.setFilterCity(filterCity);
		}
		updateData();
	}
//
//	public void onNext() {
//		
//		
//		EcommerceTree.set(Field.BuildingID, new Data(selected.getId()));
//		EcommerceTree.set(Field.Address, new Data(selected.getAddress()));
//		
//		EcommerceTree.transitState(State.BuildingReady);
//
//		model.next();
//	}
//
//	public void setSelectedBuilding(BuildingInfo selectedBuilding) {
////		Window.alert("setSelectedBuildingId = " + selectedBuilding.getId());
//		selected = selectedBuilding;
//		EcommerceTree.set(Field.BuildingID, new Data(selected.getId()));
//	}

	public void onPrev() {
		node.setState(Actions.Prev);
		node.next();
	}

	public void onCancel() {
		node.setState(Actions.Cancel);
		node.next();
	}

	public void selectSuite(BuildingInfo object) {
		if (object != null) {
			// TODO add validation
			tree.setData(Field.BuildingId, new Data(object.getId()));
			tree.setData(Field.BuildingName, new Data(object.getName()));
//			node.setData(Field.Address, new Data(object.getAddress()));
			node.setState(Actions.Next);
			node.next();
		}
	}

}
