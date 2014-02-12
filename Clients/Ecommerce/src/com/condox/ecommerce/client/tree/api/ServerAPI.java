package com.condox.ecommerce.client.tree.api;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;

public class ServerAPI implements RequestCallback {
	private RequestType type = null;
	private I_RequestCallback callback = null;

	public RequestType getType() {
		return type;
	}

	// public void setType(RequestType type) {
	// this.type = type;
	// }

	public void execute(RequestType type, JSONObject data,
			I_RequestCallback callback) {
		this.type = type;
		this.callback = callback;
		switch (this.type) {
		case GetUserInfo:
			getUserInfo(data);
			break;
		case SetUserInfo:
			setUserInfo(data);
			break;
		case GetViewOrders:
			getViewOrders(data);
			break;
		case GetSuiteInfo:
			getSuiteInfo(data);
			break;
		default:
			break;
		}
	}

	private void setUserInfo(JSONObject data) {
		String userId = data.get("userId").isString().stringValue();
		String userSID = data.get("userSID").isString().stringValue();
		String putData = data.get("putData").isString().stringValue();
		String url = Options.URL_VRT + "data/user/" + userId + "?sid="
				+ userSID;
		RequestBuilder builder = new RequestBuilder(RequestBuilder.PUT,
				URL.encode(url));
		builder.setRequestData(putData);
		builder.setHeader("Content-type", "application/json;");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	private void getUserInfo(JSONObject data) {
		String userId = data.get("userId").isString().stringValue();
		String userSID = data.get("userSID").isString().stringValue();
		String url = Options.URL_VRT + "data/user/" + userId + "?sid="
				+ userSID;
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	private void getViewOrders(JSONObject data) {
		String userId = data.get("userId").isString().stringValue();
		String userSID = data.get("userSID").isString().stringValue();
		String url = Options.URL_VRT + "data/viewOrder?userId=" + userId
				+ "&ed=Resale" + "&verbose=true" + "&sid=" + userSID;
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	private void getSuiteInfo(JSONObject data) {
		int suiteId = (int) data.get("suiteId").isNumber().doubleValue();
		String userSID = data.get("userSID").isString().stringValue();
		String url = Options.URL_VRT + "data/suite/" + suiteId + "?sid="
				+ userSID;
		Log.write("Request: " + url);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	@Override
	public void onResponseReceived(Request request, Response response) {
		Log.write("Response: " + response.getStatusCode() + ", "
				+ response.getStatusText());
		if (response.getStatusCode() == 200)
			this.callback.onOK(JSONParser.parseStrict(response.getText())
					.isObject());
		if (response.getStatusCode() == 304)
			this.callback.onOK(new JSONObject());
		// switch (this.type) {
		// case GetUserInfo:
		// this.callback.onOK(JSONParser.parseStrict(response.getText()).isObject());
		// break;
		// default:
		// break;
		//
		// }
	}

	@Override
	public void onError(Request request, Throwable exception) {
		// TODO Auto-generated method stub

	}

}
