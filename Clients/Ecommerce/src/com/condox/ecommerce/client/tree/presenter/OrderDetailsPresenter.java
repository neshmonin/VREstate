package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.PUT;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.document.BuildingInfo;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.ServerProxy;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.api.I_RequestCallback;
import com.condox.ecommerce.client.tree.api.RequestType;
import com.condox.ecommerce.client.tree.api.ServerAPI;
import com.condox.ecommerce.client.tree.view.ViewOrderInfo;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.event.logical.shared.HasValueChangeHandlers;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasHorizontalAlignment;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.HorizontalPanel;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.ui.Widget;

public class OrderDetailsPresenter implements I_Presenter, I_OrderDetailsPresenter,
		I_RequestCallback {

	public interface I_Display {
//		void setData(List<ViewOrderInfo> data);
		HasClickHandlers getDelete();
		HasValueChangeHandlers<Boolean> getEnabled();
		void setData(ViewOrderInfo info);
		void setData(String html);
		Widget asWidget();
	}

	private I_Display display;
	private EcommerceTree tree;
	private ServerAPI api = new ServerAPI();
	private ViewOrderInfo details;
	private HelloPresenter parent;

	public void setViewOrderInfo(ViewOrderInfo value) {
		details = value;
	}
	
	public void setParent(HelloPresenter parent) {
		this.parent = parent;
	}
	
	private void bind() {
		display.getDelete().addClickHandler(new ClickHandler() {
			
			@Override
			public void onClick(ClickEvent event) {
				deleteOrder();
			}
		});
		display.getEnabled().addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			
			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				// TODO Auto-generated method stub
				enableOrder(event.getValue());
			}
		});
	}
	
	void deleteOrder() {
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
				ServerProxy.deleteOrder(details.getId(), User.SID, new RequestCallback(){
					
					@Override
					public void onResponseReceived(Request request, Response response) {
//						Log.popup();
//						loadOrdersList();
						if (response.getStatusCode() != 200)
							Window.alert("Error while deleting vieworder.");
						parent.getViewOrders();
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
	
	private void enableOrder(boolean value) {
		details.setEnabled(value);
		String url = Options.URL_VRT + "data/viewOrder/" + details.getId() + 
				"?sid=" + User.SID;
		PUT.send(url, details.getJSON(), new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				parent.getViewOrders();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
	}

	@Override
	public void setView(Composite view) {
		display = (I_Display) view;
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
//		container.add((I_Contained) display);
		container.add(display.asWidget());
		bind();
		vieworder = details;
		display.setData(details);
		startLoadingData();
//		getUserInfo();
	}

//	private void getUserInfo() {
//		JSONObject obj = new JSONObject();
//		obj.put("userId", new JSONString(User.id));
//		obj.put("userSID", new JSONString(User.SID));
//		api.execute(RequestType.GetUserInfo, obj, this);
//	}
//
//	private void getViewOrders() {
//		JSONObject obj = new JSONObject();
//		obj.put("userId", new JSONString(User.id));
//		obj.put("userSID", new JSONString(User.SID));
//		api.execute(RequestType.GetViewOrders, obj, this);
//	}
//
	@Override
	public void onOK(JSONObject result) {
		switch (api.getType()) {
		case GetSuiteInfo:
			suite.fromJSONObject(result);
//			JSONObject data = new JSONObject();
			finishLoadingData();
			break;
//		case GetUserInfo:
//			Log.write(result.toString());
//			getViewOrders();
//			break;
//		case GetViewOrders:
//			Log.write(result.toString());
//			List<ViewOrderInfo> orders = new ArrayList<ViewOrderInfo>();
//			JSONArray arr = result.get("viewOrders").isArray();
//			for (int i = 0; i < arr.size(); i++) {
//				ViewOrderInfo info = ViewOrderInfo.fromJSON(arr.get(i)
//						.toString());
//				orders.add(info);
//			}
////			display.setData(orders);
//			break;
		default:
			break;
		}
	}

	@Override
	public void onError() {
		// TODO Auto-generated method stub

	}

	@Override
	public void onDeleteItem(Object item) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onEnableItem(Object item, boolean value) {
		// TODO Auto-generated method stub
		
	}

	private ViewOrderInfo vieworder = new ViewOrderInfo();
	private SuiteInfo suite = new SuiteInfo();
	private BuildingInfo building = new BuildingInfo();
	
	private void startLoadingData() {
		int suiteId = details.getTargetObjectId();
//		Log.write("" + suiteId);
		JSONObject data = new JSONObject();
		data.put("suiteId", new JSONNumber(suiteId));
		data.put("userSID", new JSONString(User.SID));
		api.execute(RequestType.GetSuiteInfo, data, this);
	}
	
	private void finishLoadingData() {
		String html = ""; 
		html += "Listing: " + (vieworder.getLabel().isEmpty()? "&lt;none&gt;" : vieworder.getLabel()) + "<br>";
		html +=	"MLS# " + (vieworder.getMLS().isEmpty()? "&lt;none&gt;" : vieworder.getMLS())  + "<br>";
		html += suite.getStatus().name().isEmpty()? "" : "Status: " + suite.getStatus() + "<br>";
		html +=	"Third Party Virtual Tour<br>";
		html += "<div style=\"margin-left:20px\">";
		if (vieworder.getVirtualTourUrl().isEmpty())
			html += "&lt;none&gt;";
		else
			html += "<a href=\"" + vieworder.getVirtualTourUrl() + "\" target = \"_blank\">" + vieworder.getVirtualTourUrl() + "</a>";
		html += "</div>";

		html +=	"More Info Link<br>";
		html += "<div style=\"margin-left:20px\">";
		if (vieworder.getMoreInfoUrl().isEmpty())
			html += "&lt;none&gt;";
		else
			html += "<a href=\"" + vieworder.getMoreInfoUrl() + "\" target = \"_blank\">" + vieworder.getMoreInfoUrl() + "</a>";
		html += "</div>";
		
		display.setData(html);
		
	}
	
	private void loadData() {
//			switch (info.getStatus()) {
//			case AvailableRent:
//				status = "For rent - price $" + price + "/m";
//				break;
//			case AvailableResale:
//				status = "For sale - price $" + price;
//				break;
//			default:
//				break;
//	}
//		}
		
		//-----------------
		
		
	}

}
