package com.condox.clientshared.communication;

import java.util.ArrayList;

import com.condox.clientshared.abstractview.Log;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;

public class PUT implements RequestCallback {

	private static Request currRequest = null;
	private static ArrayList<RequestBuilder> requests = new ArrayList<RequestBuilder>();

	RequestCallback original = null;

	private PUT(RequestCallback original) {
		this.original = original;
	}

	public static void send(String url, String requestData, RequestCallback cb) {
		RequestBuilder request = new RequestBuilder(RequestBuilder.PUT,
				URL.encode(url));
		thePUT = new PUT(cb);
		request.setRequestData(requestData);
		request.setCallback(thePUT);

		if (request != null)
			requests.add(request);
		Update();
	};

	private static PUT thePUT = null;

	private static void Update() {
		if (requests.isEmpty())
			return;

		if (currRequest == null || !currRequest.isPending()) {
			RequestBuilder requestBuilder = requests.get(0);
			RequestCallback original = requestBuilder.getCallback();
			requestBuilder.setHeader("Content-type", "application/json;");
			if (original != null)
				requestBuilder.setCallback(original);
			else
				requestBuilder.setCallback(thePUT);
			try {
				Log.write("Send: " + requestBuilder.getUrl());
				currRequest = requestBuilder.send();
				requests.remove(requestBuilder);
			} catch (RequestException e) {
				e.printStackTrace();
			}

		}
	};

	@Override
	public void onResponseReceived(Request request, Response response) {
		String received = response.getText();
		if (received.isEmpty())
			received = "<empty>";
		Log.write("RespondStatus=" + response.getStatusCode() + "; Received: "
				+ received);
		Log.write("" + response.getHeadersAsString());
		if (original != null)
			original.onResponseReceived(request, response);
		Update();
	}

	@Override
	public void onError(Request request, Throwable exception) {
		Log.write("Request Failed: " + exception.getMessage());
		Update();
	}

};