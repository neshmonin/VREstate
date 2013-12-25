package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.ServerProxy;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.node.UpdateProfile1Node;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.ui.Widget;

public class UpdateProfile1Presenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(UpdateProfile1Presenter presenter);

		UserInfo getUserInfo();
		void setUserInfo(UserInfo info);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private UpdateProfile1Node node = null;

	public UpdateProfile1Presenter(I_Display newDisplay, UpdateProfile1Node newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
		// load info
		Data data = node.getTree().getData(Field.UserInfo);
		UserInfo info = new UserInfo();
		info.fromJSONObject(data.asJSONObject());
		display.setUserInfo(info);
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained)display);
	}

	// Navigation events
	public void onClose() {
		node.next(Actions.Close);
	}

	public void onCancel() {
		node.next(Actions.Cancel);
	}

	public void onApply() {
		UserInfo info = display.getUserInfo();
		ServerProxy.setUserInfo(User.id, info.toJSONObject().toString(), User.SID, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
//		node.next(NodeStates.Apply);
	}

	public void onFinish() {
		node.next(Actions.Finish);
	}

	public void onNext() {
		node.next(Actions.Next);
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
