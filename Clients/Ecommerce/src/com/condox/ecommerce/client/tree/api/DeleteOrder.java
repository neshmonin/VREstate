package com.condox.ecommerce.client.tree.api;

import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.user.client.Window;

public class DeleteOrder extends Request implements RequestCallback {
	private I_DeleteOrder caller = null;
	
	public DeleteOrder(String url, I_DeleteOrder caller) {
		this.caller = caller;
		RequestBuilder builder = new RequestBuilder(RequestBuilder.DELETE, URL.encode(url));
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
			Window.alert("Error while deleting order: " + e.getMessage());
		}
	}
	@Override
	public void onResponseReceived(Request request, Response response) {
		caller.onDeleteOrderOK(response.getText());
	}
	@Override
	public void onError(Request request, Throwable exception) {
		caller.onDeleteOrderError();
	}
}
