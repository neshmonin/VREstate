package com.condox.vrestate.client;

import java.util.ArrayList;

//import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;

public class GET implements RequestCallback{
	
	private static Request currRequest = null;
	private static ArrayList<RequestBuilder> requests = new ArrayList<RequestBuilder>();

	RequestCallback original = null;
	private GET(RequestCallback original)
	{
		this.original = original;
	}

	public static void send(String url) {
		send(url, null);
	}
	
	public static void send(String url, RequestCallback cb) {
		if (Options.SHOW_SOLD)
			if (url.endsWith("?"))
				url += "ShowSold=true";
			else if (url.contains("?"))
				url += "&ShowSold=true";
		
		//url += "&track=true";
		
		RequestBuilder request = new RequestBuilder(RequestBuilder.GET, URL.encode(url));
		theGET = new GET(cb);
		request.setCallback(theGET);

		if (request != null)
			requests.add(request);
		Update();
	};
	
	private static GET theGET = null;
	private static void Update() {
		if (requests.isEmpty())
			return;

		if (currRequest == null || !currRequest.isPending()) 
		{
			RequestBuilder requestBuilder = requests.get(0);
			RequestCallback original = requestBuilder.getCallback();
			requestBuilder.setRequestData(null);
			if (original != null)
				requestBuilder.setCallback(original);
			else
				requestBuilder.setCallback(theGET);
			
			try {
				Log.write("Send: " + requestBuilder.getUrl());
				currRequest = requestBuilder.send();
				requests.remove(requestBuilder);
			} catch (RequestException e) {
				e.printStackTrace();
			}
	
		};

	};
	
	
	@Override
	public void onResponseReceived(Request request,
			Response response) {
		String received = response.getText();
		if (received.isEmpty())
			received = "<empty>";
		Log.write("RespondStatus="+response.getStatusCode()+"; Received: " + received);
		if (original != null)
			original.onResponseReceived(request, response);
		Update();
	}

	@Override
	public void onError(Request request, Throwable exception) {
		Log.write("Request Failed: " + exception.getMessage());
	}
	
};