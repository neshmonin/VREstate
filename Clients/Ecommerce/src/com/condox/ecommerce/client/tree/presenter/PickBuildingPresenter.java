package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.document.BuildingInfo;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Cookies;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class PickBuildingPresenter implements I_Presenter {

	public interface IDisplay {
		
		void setPresenter(PickBuildingPresenter presenter);

		void setData(List<BuildingInfo> data, BuildingInfo selected);
		
		String getFilterCity();
		void setFilterCity(String city);
		

		Widget asWidget();
	}

	private IDisplay display = null;
	private List<BuildingInfo> data = new ArrayList<BuildingInfo>();
	private BuildingInfo selected;
	private EcommerceTree tree = null;

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
				Log.write(json);
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("buildings").isArray();

				Integer id = null;
//				Data buildingIdData = tree.getData(Field.BuildingId);
//				if (buildingIdData != null)
//					id = buildingIdData.asInteger();
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
	public void go(HasWidgets container) {
		container.clear();
		container.add(display.asWidget());
		String city = Cookies.getCookie("city");
		if (city != null)
			display.setFilterCity(city);
//		Data data = tree.getData(Field.FILTERING_BY_CITY);
//		if (data != null) {
//			String filterCity = data.Value;
//			display.setFilterCity(filterCity);
//		}
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
		tree.next(Actions.Prev);
	}

	public void onCancel() {
		tree.next(Actions.Cancel);
	}

	public void selectSuite(BuildingInfo object) {
		if (object != null) {
			Cookies.setCookie("city", display.getFilterCity(),
					new Date(new Date().getTime() + 1000 * 60 * 60 * 24));
			// TODO add validation
//			tree.setData(Field.BuildingId, new Data(object.getId()));
//			tree.setData(Field.BuildingName, new Data(object.getName()));
			tree.setData(Field.BuildingInfo, new Data(object));
			tree.next(Actions.Next);
		}
	}

	@Override
	public void setView(Composite view) {
		display = (IDisplay) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

}
