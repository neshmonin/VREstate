package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.PUT;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.Ecommerce;
import com.condox.ecommerce.client.Ecommerce.Modes;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.ServerProxy;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceNode;
import com.condox.ecommerce.client.tree.view.ViewOrderInfo;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasHorizontalAlignment;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.ui.Widget;

public class HelloPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(HelloPresenter presenter);
		
		void setNickName(String value);
		
		void setData(List<ViewOrderInfo> data);
		
		Widget asWidget();

		void setViewOrderUrl(String url);
	}

	private I_Display display = null;
	private EcommerceTree tree = null;
// ServerAPI api = new ServerAPI();
	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
		loadPersonalInfo();
	}
	
	private void loadPersonalInfo() {
		ServerProxy.getUserInfo(User.id, User.SID, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				JSONObject obj = JSONParser.parseLenient(response.getText()).isObject();
				UserInfo info = new UserInfo();
				info.fromJSONObject(obj);
				tree.setData(Field.UserInfo, new Data(info));
				display.setNickName(info.getNickName());
				loadOrdersList();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}
	
	private void loadOrdersList() {
		final PopupPanel loading = new PopupPanel();
		loading.clear();
		loading.setModal(true);
		loading.setGlassEnabled(true);
		loading.add(new Label("Loading listings, please wait..."));
		loading.center();
		
		ServerProxy.getOrdersList(User.id, User.SID, new RequestCallback(){

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
				loading.hide();
				display.setData(data);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}
	
//  Events
	public void onLogout() {
		tree.next(Actions.Logout);
	}

	public void onShowSettings() {
		tree.next(Actions.Settings);
	}

	public void onNewOrder() {
		tree.next(Actions.NewOrder);
	}

	public void onShowHistory() {
		tree.next(Actions.History);
	}

	public void setEnabled(ViewOrderInfo object, boolean enabled) {
		object.setEnabled(enabled);
		String url = Options.URL_VRT + "data/viewOrder/" + object.getId() + 
				"?sid=" + User.SID;
		PUT.send(url, object.getJSON(), new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				if (Modes.testUpdateOrder.equals(Ecommerce.mode))
					Log.popup();
				loadOrdersList();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}

	public void delete(final ViewOrderInfo object) {
		final PopupPanel confirm = new PopupPanel();
		confirm.setModal(true);
		confirm.setGlassEnabled(true);
		VerticalPanel vp = new VerticalPanel();
		vp.setHorizontalAlignment(HasHorizontalAlignment.ALIGN_CENTER);
		vp.setSpacing(10);
		Label labelConfirm1 = new Label("Order will be deleted PERMANENTLY!");
		Label labelConfirm2 = new Label("Are you sure?");
		vp.add(labelConfirm1);
		vp.add(labelConfirm2);
		HorizontalPanel hp = new HorizontalPanel();
		hp.setWidth("100%");
		hp.setHorizontalAlignment(HasHorizontalAlignment.ALIGN_CENTER);
		Button buttonYes = new Button("Yes");
		buttonYes.setWidth("75px");
		buttonYes.addClickHandler(new ClickHandler(){

			@Override
			public void onClick(ClickEvent event) {
				confirm.hide();
				ServerProxy.deleteOrder(object.getId(), User.SID, new RequestCallback(){
					
					@Override
					public void onResponseReceived(Request request, Response response) {
//						Log.popup();
						loadOrdersList();
					}
					
					@Override
					public void onError(Request request, Throwable exception) {
						// TODO Auto-generated method stub
						
					}});
			}});
		Button buttonNo = new Button("No");
		buttonNo.setWidth("75px");
		buttonNo.addClickHandler(new ClickHandler(){

			@Override
			public void onClick(ClickEvent event) {
				confirm.hide();
			}});
		hp.add(buttonYes);
		hp.add(buttonNo);
		vp.add(hp);
		confirm.setWidget(vp);
		confirm.center();
	}

	public void onUpdateProfile() {
		tree.next(Actions.ProfileStep1);
	}

	public void openAddress(ViewOrderInfo object) {
		if (object != null) {
			String url = object.getUrl();
			Window.open(url, "_blank", null);
		}
	}

	@Override
	public void setView(Composite view) {
		// TODO Auto-generated method stub
		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		// TODO Auto-generated method stub
		this.tree = tree;
	}

	public void getUrl(ViewOrderInfo object) {
		display.setViewOrderUrl(object.getUrl());
	}
}
