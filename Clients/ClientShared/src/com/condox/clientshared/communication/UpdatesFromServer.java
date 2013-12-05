package com.condox.clientshared.communication;

import java.util.ArrayList;
import java.util.Collection;
import com.condox.clientshared.abstractview.Log;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONValue;

public class UpdatesFromServer implements RequestCallback{
	
	private static Request currRequest = null;
	private static ArrayList<RequestBuilder> requests = new ArrayList<RequestBuilder>();

	static Collection<I_CheckChanges> changeHandlers = new ArrayList<I_CheckChanges>();
	private UpdatesFromServer()	{}

	private static int counter = 0;

	public static void RegisterHandler(I_CheckChanges handler) {
		changeHandlers.add(handler);
	}

	public static void RenewCheckChangesThread() {
		String url = Options.URL_VRT + "ev?sid=" + User.SID
				+ "&generation=" + counter++;
		if (Options.context != "") {
			if (url.endsWith("?"))
				url += Options.context;
			else if (url.contains("?"))
				url += "&" + Options.context;
		}
		
		//url += "&track=true";
		
		RequestBuilder request = new RequestBuilder(RequestBuilder.GET, URL.encode(url));
		theGET = new UpdatesFromServer();
		request.setCallback(theGET);

		if (request != null)
			requests.add(request);
		Update();
	}
	
	private static UpdatesFromServer theGET = null;
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

	}	
	
	@Override
	public void onResponseReceived(Request request,
			Response response) {
		String received = response.getText();
		int respondStatus = response.getStatusCode();
		if (received == null || received.length() == 0)
			Log.write("CheckChanges RespondStatus="+respondStatus+"; Received: <empty>!");
		else
			Log.write("CheckChanges RespondStatus="+respondStatus+"; Received: " + received);
		
		if(respondStatus != 200)
			User.Reconnect();
		else
		{
			if (received != null && received.length() != 0) {
				JSONValue JSONreceived = JSONParser.parseLenient(received);
				if (JSONreceived != null) {
					for (I_CheckChanges handler : changeHandlers) {
						handler.onCheckChanges(response);
					}
				}
			}
	
			Update();
			RenewCheckChangesThread();
		}
	}

	@Override
	public void onError(Request request, Throwable exception) {
		Log.write("Request Failed: " + exception.getMessage());
	}
};