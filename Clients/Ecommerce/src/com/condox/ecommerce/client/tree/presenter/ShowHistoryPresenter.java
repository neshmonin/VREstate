package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.ShowHistoryNode;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONValue;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Widget;

public class ShowHistoryPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(ShowHistoryPresenter presenter);
		
		void setHistoryData(String html);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private ShowHistoryNode node = null;

	public ShowHistoryPresenter(I_Display newDisplay, ShowHistoryNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
		String url = Options.URL_VRT + "data/ft?" +
				"&userId=" + User.id + 
				"&sid=" + User.SID;
		
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				String json = response.getText();
				JSONObject obj = JSONParser.parseLenient(json).isObject();
				if (obj.containsKey("transactions"))
					if (obj.get("transactions").isArray() != null) {
						JSONArray items = obj.get("transactions").isArray();
						String html = "";
						for (int i = 0; i < items.size(); i++)
							html += items.get(i).toString() + "<br />";
						display.setHistoryData(html);
					}
			}

			@Override
			public void onError(Request request, Throwable exception) {
				
			}});
	}

//  Events
	public void onClose() {
		node.setState(NodeStates.Close);
		node.next();
	}
}
