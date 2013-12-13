package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.PUT;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.HelloAgentNode;
import com.condox.ecommerce.client.tree.view.ViewOrderInfo;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.Widget;

public class HelloAgentPresenter implements I_Presenter/*, I_HelloAgent*/ {

	public static interface I_Display extends I_Contained {
		void setPresenter(HelloAgentPresenter presenter);
		
		void setNickName(String value);
		
		void setData(List<ViewOrderInfo> data);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private HelloAgentNode node = null;

	public HelloAgentPresenter(I_Display newDisplay, HelloAgentNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
		loadPersonalInfo();
		
	}
	
	private void loadPersonalInfo() {
		String url = Options.URL_VRT + "data/user/" + User.id + "?&sid=" + User.SID;
		GET.send(url, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				JSONObject obj = JSONParser.parseLenient(response.getText()).isObject();
				String nickName = obj.get("nickName").isString().stringValue();
				if (nickName.contains(","))
					nickName = nickName.substring(0, nickName.indexOf(","));
				display.setNickName(nickName);
				loadOrdersList();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});	
	}
	
	private void loadOrdersList() {
		String url = Options.URL_VRT + "data/viewOrder?userId=" + User.id + 
				"&ed=Resale" + 
				"&verbose=true" +
				"&sid=" + User.SID;
		GET.send(url, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				List<ViewOrderInfo> data = new ArrayList<ViewOrderInfo>();

				// TODO parse response json
				JSONObject obj = JSONParser.parseLenient(response.getText())
						.isObject();
				JSONArray arr = obj.get("viewOrders").isArray();
				for (int i = 0; i < arr.size(); i++) {
					ViewOrderInfo info = ViewOrderInfo.fromJSON(arr.get(i)
							.toString());
					if (info != null)
						data.add(info);
				}
				// -------------------------
				// for (int i = 0; i < 20; i++)
				// data.add(new ViewOrderInfo());
				display.setData(data);
//				container.clear();
//				container.add((I_Contained) display);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});	
	}
	
//  Events
	public void onLogout() {
		node.setState(NodeStates.Logout);
		node.next();
	}

	public void onShowSettings() {
		node.setState(NodeStates.Settings);
		node.next();
	}

	public void onNewOrder() {
		node.setState(NodeStates.NewOrder);
		node.next();
	}

	public void onShowHistory() {
		node.setState(NodeStates.ShowHistory);
		node.next();
	}

	public void setEnabled(ViewOrderInfo object, boolean enabled) {
		object.setEnabled(enabled);
		String url = Options.URL_VRT + "data/viewOrder/" + object.getId() + 
				"?sid=" + User.SID;
		PUT.send(url, object.getJSON(), new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				loadOrdersList();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}

	public void delete(ViewOrderInfo object) {
//		object.setEnabled(enabled);
//		String url = Options.URL_VRT + "data/viewOrder/" + object.getId() + 
//				"?sid=" + User.SID;
//		DELETE.send(url, object.getJSON(), new RequestCallback(){
//
//			@Override
//			public void onResponseReceived(Request request, Response response) {
//				loadOrdersList();
//			}
//
//			@Override
//			public void onError(Request request, Throwable exception) {
//				// TODO Auto-generated method stub
//				
//			}});
	}

	public void onUpdateProfile() {
		node.next(NodeStates.UpdateProfile);
	}
}
