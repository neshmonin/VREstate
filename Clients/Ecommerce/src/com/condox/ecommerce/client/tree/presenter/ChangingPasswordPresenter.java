package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.UserInfo;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.view.WarningView;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class ChangingPasswordPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(ChangingPasswordPresenter presenter);

		void setResult(int result);

		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;

	@Override
	public void go(final HasWidgets container) {
		Data data = tree.getData(Field.UserInfo);
		if (data != null) {
			UserInfo info = new UserInfo();
			info.fromJSONObject(data.asJSONObject());

			String url = Options.URL_VRT + "program" //
					+ "?q=recover" //
					+ "&role=" + info.getRole().name().toLowerCase() //
//					+ "&ed=<estate developer id>" 
					+ "&uid=" + URL.encodeQueryString(info.getLogin());
			GET.send(url, new RequestCallback(){

				@Override
				public void onResponseReceived(Request request,
						Response response) {
					if (response.getStatusCode() != 200) {
						WarningPresenter warning = new WarningPresenter(new WarningView(), null);
						String message = "Status code: " + response.getStatusCode() + "<br />";
						message += "Status text: " + response.getStatusText() + "<br />";
						message += "Response text: " + response.getText();
						warning.setMessage(message);
						warning.go(null);
					} else {
					Log.write(response.getStatusText());
					 display.setResult(0);
					 container.clear();
					 container.add(display.asWidget());
					// TODO Auto-generated method stub
					}
				}

				@Override
				public void onError(Request request, Throwable exception) {
					new WarningPresenter(new WarningView(), null).go(null);
				}});
		}

	}

	// private void recoverPassword() {
	// String login = getString(Field.Email);
	// ServerProxy.recoverPassword(login, new RequestCallback(){
	//
	// @Override
	// public void onResponseReceived(Request request, Response response) {
	// if (response.getStatusCode() == 200)
	// display.setResult(0);
	// else
	// display.setResult(1);
	// }
	//
	// @Override
	// public void onError(Request request, Throwable exception) {
	// display.setResult(1);
	// }});
	// }

	// Navigation events
	public void onBackToLogin() {
		tree.next(Actions.Close);
	}

	// Data utils
	private String getString(Field key) {
		Data data = tree.getData(key);
		String s = (data == null) ? "" : data.asString();
		return s.isEmpty() ? "" : s;
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

}
