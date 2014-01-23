package com.condox.ecommerce.client.tree.presenter;

import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;

public class GetHistoryHandler implements RequestCallback {
	
	private I_GetHistoryHandler callback = null;
	
	public GetHistoryHandler(I_GetHistoryHandler callback) {
		this.callback = callback;
	}

	@Override
	public void onResponseReceived(Request request, Response response) {
		callback.onGetHistoryOk(response.getText());
	}

	@Override
	public void onError(Request request, Throwable exception) {
		callback.onGetHistoryError();
	}

}
