package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.State;
import com.condox.ecommerce.client.tree.model.HelloNode;
import com.condox.ecommerce.client.tree.view.ViewOrderInfo;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.Widget;

public class HelloPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(HelloPresenter presenter);

//		String getUserLogin();

//		String getUserPassword();

		Widget asWidget();

		void setData(List<ViewOrderInfo> data);
	}

	private I_Display display = null;
	private HelloNode node = null;
	private I_Container container;

	public HelloPresenter(I_Display display, HelloNode node) {
		this.display = display;
		this.display.setPresenter(this);
		this.node = node;
	}

	@Override
	public void go(final I_Container container) {
		this.container = container;
		
		String request = Options.URL_VRT + "data/user/" + EcommerceTree.get(Field.UserId).asString() + "?"
				+ "sid=" + User.SID;
		GET.send(request, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				loadOrders();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}
	
	private void loadOrders() {
		String request = Options.URL_VRT + "data/viewOrder?"
				+ "userId=" + EcommerceTree.get(Field.UserId).asString()
				+ "&ed=Resale"
				+ "&verbose=true"
				+ "&sid=" + User.SID;
		GET.send(request, new RequestCallback() {
			
			@Override
			public void onResponseReceived(Request request, Response response) {
				List<ViewOrderInfo> data = new ArrayList<ViewOrderInfo>();
				
				// TODO parse response json
				JSONObject obj = JSONParser.parseLenient(response.getText()).isObject();
				JSONArray arr = obj.get("viewOrders").isArray();
				for (int i = 0; i < arr.size(); i++) {
					ViewOrderInfo info = ViewOrderInfo.fromJSON(arr.get(i).toString());
					if (info != null)
						data.add(info);
				}
				//-------------------------
//				for (int i = 0; i < 20; i++)
//					data.add(new ViewOrderInfo());
				display.setData(data);
				container.clear();
				container.add((I_Contained)display);
			}
			
			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}

	public void onShowHistory() {
		// TODO Auto-generated method stub
		EcommerceTree.transitState(State.History);
		node.next();
	}

//	public void onLogin() {
//		final String uid = display.getUserLogin();
//		final String pwd = display.getUserPassword();
//		EcommerceTree.set(Field.UserLogin, new Data(uid));
//		EcommerceTree.set(Field.UserPassword, new Data(pwd));
//		
////		if (!model.isValid()) {
////			Window.alert("Not valid! Please, check and try again!");
////			return;
////		}
//		
//		String role = "visitor";
////		String url = Options.getUserLogin(uid, pwd, role);
////		EcommerceTree.transitState(State.Guest); // for role == "visitor"
//
////		url = URL.encode(url);
//		
//		String request = Options.URL_VRT + "program?q=login&role=visitor"
//				+ "&uid=" + uid
//				+ "&pwd=" + pwd;
//
//		// GET.send(url);
//		User.Login(this, request);
//	}
//
//	@Override
//	public void onLoginSucceed() {
//		EcommerceTree.transitState(State.Agent);
//		if (display.getUserLogin().equalsIgnoreCase("web"))
//			if (display.getUserPassword().equalsIgnoreCase("web"))
//				EcommerceTree.transitState(State.Guest);
//		model.next();
//	}
//
//	@Override
//	public void onLoginFailed(Throwable exception) {}
}
