package com.condox.vrestate.client;

import java.util.ArrayList;

import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;

public class GET {
	
	private static Request currRequest = null;
	
	private static ArrayList<RequestBuilder> requests = new ArrayList<RequestBuilder>();
	
	public static void send(String url, RequestCallback cb) {
		if ((User.isReady())&&(Options.SHOW_SOLD))
			if (url.endsWith("?"))
				url += "ShowSold=true";
			else if (url.contains("?"))
				url += "&ShowSold=true";
				
		
		
		RequestBuilder request = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		request.setCallback(cb);
		if (request != null)
			requests.add(request);
		Update();
	};
	
	private static void Update() {
		if (requests.isEmpty())
			return;
		else
		if ((currRequest == null)||(!currRequest.isPending())) 
		{
			RequestBuilder requestBuilder = requests.get(0);
			final RequestCallback original = requestBuilder.getCallback();
			requestBuilder.setRequestData(null);
			requestBuilder.setCallback(new RequestCallback(){
	
				@Override
				public void onResponseReceived(Request request,
						Response response) {
					Log.write("Received: " + response.getText());
					original.onResponseReceived(request, response);
					Update();
				}
	
				@Override
				public void onError(Request request, Throwable exception) {
					// TODO Обработка ошибок
				}
			});
			
			try {
				Log.write("Send: " + requestBuilder.getUrl());
				currRequest = requestBuilder.send();
				requests.remove(requestBuilder);
			} catch (RequestException e) {
				// TODO Обработка ошибок
				e.printStackTrace();
			}
	
		};

	};
};