package com.condox.order.client.view;

import com.condox.order.client.context.BuildingsContext;
import com.condox.order.client.context.Log;
import com.condox.order.client.context.Tree;
import com.condox.order.client.utils.GET;
import com.condox.order.client.utils.Globals;
import com.condox.order.client.view.factory.IView;
import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class WelcomeView extends Composite implements IView {

	private static WelcomeViewUiBinder uiBinder = GWT
			.create(WelcomeViewUiBinder.class);

	interface WelcomeViewUiBinder extends UiBinder<Widget, WelcomeView> {
	}

	private Tree tree;
	
	public WelcomeView(Tree tree) {
		this();
		this.tree = tree;
		defaultLogin();
	}
	
	private void defaultLogin() {
		GET.send(Globals.getLoginRequest("visitor", "web", "web"), onLoginRequest);
	}
	
	private RequestCallback onLoginRequest = new RequestCallback(){

		@Override
		public void onResponseReceived(Request request, Response response) {
			Log.write(response.getText());
			JSONObject obj = JSONParser.parseStrict(response.getText()).isObject();
			Globals.setSID(obj.get("sid").isString().stringValue());
			
			tree.next(new BuildingsContext());
		}

		@Override
		public void onError(Request request, Throwable exception) {
		}};
	
	public WelcomeView() {
		initWidget(uiBinder.createAndBindUi(this));
	}
}
