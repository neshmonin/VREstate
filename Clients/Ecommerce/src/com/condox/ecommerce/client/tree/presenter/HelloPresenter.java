package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.model.LoginModel;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.api.I_RequestCallback;
import com.condox.ecommerce.client.tree.api.RequestType;
import com.condox.ecommerce.client.tree.api.ServerAPI;
import com.condox.ecommerce.client.tree.view.OrderDetailsView;
import com.condox.ecommerce.client.tree.view.PasswordChangeRequired;
import com.condox.ecommerce.client.tree.view.ViewOrderInfo;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.ClickHandler;
import com.google.gwt.event.dom.client.HasClickHandlers;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.SelectionChangeEvent;
import com.google.gwt.view.client.SingleSelectionModel;

public class HelloPresenter implements I_Presenter, I_HelloPresenter,
		I_RequestCallback {

	public interface I_Display {
		HasClickHandlers getEditProfile();

		HasClickHandlers getEditSettings();

		HasClickHandlers getCreateOrder();

		HasClickHandlers getShowHistory();

		HasClickHandlers getLogout();

		HasClickHandlers getReloadPage();

		void setMainView();

		void setProgressView();

		void setErrorView();

		SingleSelectionModel<ViewOrderInfo> getSelection();

		HasWidgets getDetailsArea();

		void setData(List<ViewOrderInfo> data);

		void setUserName(String name);

		Widget asWidget();
	}

	private I_Display display;
	private EcommerceTree tree;
	private ServerAPI api = new ServerAPI();
	List<ViewOrderInfo> orders = new ArrayList<ViewOrderInfo>();
	public static String selected;// remove
	private UserInfo userInfo = new UserInfo();

	private void bind() {
		display.getEditProfile().addClickHandler(new ClickHandler() {

			@Override
			public void onClick(ClickEvent event) {
				showEditProfile();
			}
		});
		display.getEditSettings().addClickHandler(new ClickHandler() {

			@Override
			public void onClick(ClickEvent event) {
				showEditSettings();
			}
		});
		display.getCreateOrder().addClickHandler(new ClickHandler() {

			@Override
			public void onClick(ClickEvent event) {
				showCreateOrder();
			}
		});
		display.getShowHistory().addClickHandler(new ClickHandler() {

			@Override
			public void onClick(ClickEvent event) {
				showHistory();
			}
		});
		display.getSelection().addSelectionChangeHandler(
				new SelectionChangeEvent.Handler() {

					@Override
					public void onSelectionChange(SelectionChangeEvent event) {
						selected = display.getSelection().getSelectedObject()
								.getId();
						showDetails(display.getSelection().getSelectedObject());
					}
				});
		display.getLogout().addClickHandler(new ClickHandler() {

			@Override
			public void onClick(ClickEvent event) {
				// TODO Auto-generated method stub
				tree.next(Actions.Logout);
			}
		});
		// display.getList().addChangeHandler(new ChangeHandler() {
		//
		// @Override
		// public void onChange(ChangeEvent event) {
		// showDetails(orders.get(display.getSelected()));
		// }
		// });

	}

	private void showEditProfile() {
		tree.next(Actions.ProfileStep1);
	}

	private void showEditSettings() {
		tree.next(Actions.Settings);
	}

	private void showCreateOrder() {
		tree.next(Actions.NewOrder);
	}

	private void showHistory() {
		tree.next(Actions.History);
	}

	void showDetails(ViewOrderInfo value) {
		OrderDetailsPresenter detailsPresenter = new OrderDetailsPresenter();
		OrderDetailsView detailsView = new OrderDetailsView();
		detailsPresenter.setTree(tree);
		detailsPresenter.setView(detailsView);
		detailsPresenter.setViewOrderInfo(value);
		detailsPresenter.setParent(this);
		detailsPresenter.go(display.getDetailsArea());
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
//		Data data = tree.getData(Field.UserInfo);
//		userInfo.fromJSONObject(data.asJSONObject());
		
		
		
		
//		if (!info.isPasswordChangeRequired())
//			tree.next(Actions.Settings);
//		else {
			// ---------------------------------------
			container.clear();
			container.add(display.asWidget());
			bind();
			// display.setProgressView();
			getUserInfo();
//		}
	}

	private void getUserInfo() {
		// display.setProgressView();
//		tree.recenter();
		Data data = tree.getData(Field.LoginModel);
		LoginModel loginModel = new LoginModel();
		loginModel.fromJSONObject(data.asJSONObject());
		
		JSONObject obj = new JSONObject();
		obj.put("userId", new JSONString(User.id));
		obj.put("userSID", new JSONString(User.SID));
		api.execute(RequestType.GetUserInfo, obj, this);
	}

	private PopupPanel loading = new PopupPanel();

	public void getViewOrders() {
		loading.clear();
		loading.setModal(true);
		loading.setGlassEnabled(true);
		loading.add(new Label("Loading, please wait..."));
		loading.center();

		JSONObject obj = new JSONObject();
		obj.put("userId", new JSONString(String.valueOf(userInfo.getId())));
		obj.put("userSID", new JSONString(User.SID));
		api.execute(RequestType.GetViewOrders, obj, this);
	}

	@Override
	public void onItemSelected(Object selectedItem) {
		// TODO Auto-generated method stub

	}

	@Override
	public void onOK(JSONObject result) {
		switch (api.getType()) {
		case GetUserInfo:
			// Log.write(result.toString());

//			UserInfo user = new UserInfo();
			userInfo.fromJSONObject(result);
			display.setUserName(userInfo.getPersonalInfo().getLastName() + ", "
					+ userInfo.getPersonalInfo().getFirstName());
			tree.setData(Field.UserInfo, new Data(userInfo));
			//-----------------
			if (userInfo.isPasswordChangeRequired()) {
				final DialogBox passwordChangeRequired = new DialogBox();
				PasswordChangeRequired widget = new PasswordChangeRequired();

				widget.getNow().addClickHandler(new ClickHandler() {

					@Override
					public void onClick(ClickEvent event) {
//						userInfo.setPasswordChangeRequired(true);
						passwordChangeRequired.hide();
						tree.next(Actions.Settings);
					}
				});
				widget.getLater().addClickHandler(new ClickHandler() {

					@Override
					public void onClick(ClickEvent event) {
//						userInfo.setPasswordChangeRequired(false);
						passwordChangeRequired.hide();
//						tree.next(Actions.Agent);
						getViewOrders();
					}
				});
				passwordChangeRequired.add(widget.asWidget());
				passwordChangeRequired.setGlassEnabled(true);
				passwordChangeRequired.center();
			} else 
			//-----------------
			getViewOrders();
			break;
		case GetViewOrders:
			loading.hide();
			Log.write(result.toString());
			JSONArray arr = result.get("viewOrders").isArray();
			orders.clear();
			for (int i = 0; i < arr.size(); i++) {
				ViewOrderInfo info = ViewOrderInfo.fromJSON(arr.get(i)
						.toString());
				info.fromJSONObject(JSONParser.parseStrict(
						arr.get(i).toString()).isObject());
				orders.add(info);
			}
			display.setData(orders);
			if (!orders.isEmpty())
				display.getSelection().setSelected(orders.get(0), true);
			for (ViewOrderInfo item : orders)
				if (item.getId().equals(selected))
					display.getSelection().setSelected(item, true);
			// display.setMainView();
			// tree.recenter();
			break;
		default:
			break;
		}
	}

	@Override
	public void onError() {
		// TODO Auto-generated method stub

	}

}
