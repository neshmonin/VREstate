package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.ServerProxy;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.ChangingPasswordNode;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.ui.Widget;

public class ChangingPasswordPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(ChangingPasswordPresenter presenter);
		void setResult(int result);
		Widget asWidget();
	}

	private I_Display display = null;
	private ChangingPasswordNode node = null;

	public ChangingPasswordPresenter(I_Display newDisplay, ChangingPasswordNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		recoverPassword();
		container.clear();
		container.add((I_Contained)display);
	}
	
	private void recoverPassword() {
		String login = getString(Field.UserEmail);
		ServerProxy.recoverPassword(login, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				if (response.getStatusCode() == 200)
					display.setResult(0);
				else
					display.setResult(1);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				display.setResult(1);
			}});
	}
	
	// Navigation events
	public void onBackToLogin() {
		node.setState(NodeStates.Ok);
		node.next();
	}
	
	// Data utils
	private String getString(Field key) {
		Data data = node.getTree().getData(key);
		String s = (data == null)? "" : data.asString();
		return s.isEmpty()? "" : s;
	}	
	
}
