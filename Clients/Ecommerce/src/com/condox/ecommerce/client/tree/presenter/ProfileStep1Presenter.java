package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.api.I_RequestCallback;
import com.condox.ecommerce.client.tree.api.RequestType;
import com.condox.ecommerce.client.tree.api.ServerAPI;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class ProfileStep1Presenter implements I_Presenter, I_RequestCallback {

	public static interface I_Display {
		void setPresenter(ProfileStep1Presenter presenter);

		UserInfo getUserInfo();
		void setUserInfo(UserInfo info);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;
	private ServerAPI api = new ServerAPI();
	private boolean wantToFinish = false;
	private HasWidgets container = null;


	@Override
	public void go(HasWidgets container) {
		this.container = container;
		startLoading();
	}
	
	private void startLoading() {
//		Data data = tree.getData(Field.UserInfo);
//		UserInfo info = new UserInfo();
//		info.fromJSONObject(data.asJSONObject());
//		display.setUserInfo(info);
//		finishLoading();
		JSONObject obj = new JSONObject();
		obj.put("userId", new JSONString(User.id));
		obj.put("userSID", new JSONString(User.SID));
		api.execute(RequestType.GetUserInfo, obj, this);
	}
	
	private void finishLoading() {
		container.clear();
		container.add(display.asWidget());
	}

	// Navigation events
	
	public void onClose() {
		tree.next(Actions.Close);
	}

	public void onCancel() {
		tree.next(Actions.Cancel);
	}

	public void onApply() {
		UserInfo info = display.getUserInfo();
		
		JSONObject data = new JSONObject();
		data.put("userId", new JSONString(User.id));
		data.put("userSID", new JSONString(User.SID));
		data.put("putData", new JSONString(info.toJSONObject().toString()));
		api.execute(RequestType.SetUserInfo, data, this);
	}
	
	public void onFinish() {
		wantToFinish = true;
		onApply();
	}

	public void onNext() {
//		tree.next(Actions.Next);
	}

	@Override
	public void setView(Composite view) {
		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

	public void onChange() {
		
	}

	@Override
	public void onSuccess(JSONObject result) {
		// TODO Auto-generated method stub
		switch(api.getType()) {
		case GetUserInfo:
			UserInfo user = new UserInfo();
			user.fromJSONObject(result);
			display.setUserInfo(user);
			finishLoading();
			break;
		case SetUserInfo:
			if (wantToFinish)
				tree.next(Actions.Finish);
			break;
		default:
			break;
		}
	}

	@Override
	public void onError(String message) {
		// TODO Auto-generated method stub
		
	}

	// Events
//	public void onForgotPassword() {
//		String email = display.getUserEmail().trim();
//		String password = display.getUserPassword().trim();
//		node.setData(Field.UserEmail, new Data(email));
//		node.setData(Field.UserPassword, new Data(password));
//		node.setState(NodeStates.ForgotPassword);
//		node.next();
//	}

}
