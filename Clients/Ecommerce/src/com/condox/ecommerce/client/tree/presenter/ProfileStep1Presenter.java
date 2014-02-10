package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.ServerProxy;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class ProfileStep1Presenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(ProfileStep1Presenter presenter);

		UserInfo getUserInfo();
		void setUserInfo(UserInfo info);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;



	@Override
	public void go(HasWidgets container) {
		Data data = tree.getData(Field.UserInfo);
		UserInfo info = new UserInfo();
		info.fromJSONObject(data.asJSONObject());
		display.setUserInfo(info);
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
		onApply();
		tree.next(Actions.Finish);
	}

	public void onNext() {
		tree.next(Actions.Next);
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
