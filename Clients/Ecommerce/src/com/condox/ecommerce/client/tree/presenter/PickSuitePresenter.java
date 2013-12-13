package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.PickSuiteNode;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.Widget;

public class PickSuitePresenter implements I_Presenter {

	public interface IDisplay extends I_Contained {
		void setPresenter(PickSuitePresenter presenter);

		void setData(List<SuiteInfo> data);

		Widget asWidget();

		String getSuiteName();

		String getSuiteFloorplan();

		SuiteInfo getSelectedSuite();
	}

	private IDisplay display = null;
	private List<SuiteInfo> data = new ArrayList<SuiteInfo>();
	private PickSuiteNode node = null;

	public PickSuitePresenter(IDisplay display, PickSuiteNode node) {
		this.display = display;
		this.display.setPresenter(this);
		this.node = node;
	}

	private void updateData() {
		display.setData(null);
		// TODO add validation
		int buildingId = node.getTree().getData(Field.BuildingId).asInteger();
		
		//	Sample: "https://vrt.3dcondox.com/data/inventory?"
		String url = Options.URL_VRT;
		url += "data/inventory?";
		url += "building=" + buildingId;
		url += "&sid=" + User.SID;
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				// Log.write("answer:" + response.getText());
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("inventory").isArray();
				for (int index = 0; index < arr.size(); index++) {
					SuiteInfo info = new SuiteInfo();
					info.fromJSONObject(arr.get(index).isObject());
					data.add(info);
				}
				display.setData(data);
				// CreateTable();
			}

			@Override
			public void onError(Request request, Throwable exception) {
			}
		});
	}


	public String getSelectedBuildingStreet() {
		// TODO ??
		return "Floor:";//tree.getString("buildings.selected.street");
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add(display);
		updateData();
	}

	// Navigation events
	public void onCancel() {
		node.setState(NodeStates.Cancel);
		node.next();
	}
	
	public void onPrev() {
		node.setState(NodeStates.Prev);
		node.next();
	}

	public void onNext() {
		saveData();
		node.setState(NodeStates.Next);
		node.next();
	}
	
	// Data utils
	private void saveData() {
		node.setData(Field.SuiteId, new Data(display.getSelectedSuite().getId()));
		node.setData(Field.SuiteName, new Data(display.getSelectedSuite().getName()));
		node.setData(Field.SuiteAddress, new Data(display.getSelectedSuite().getAddress()));
		node.setData(Field.SuiteMLS, new Data(display.getSelectedSuite().getMLS()));
		node.setData(Field.SuitePrice, new Data(display.getSelectedSuite().getPrice()));
	}


}
