package com.condox.vrestate.client;

import java.util.ArrayList;
import java.util.Map;

//import com.google.gwt.core.client.GWT;
import com.condox.vrestate.client.view._AbstractView;
import com.condox.vrestate.shared.IDocument;
import com.condox.vrestate.shared.Log;
import com.condox.vrestate.shared.Options;
import com.condox.vrestate.shared.Suite;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONValue;

public class GETEV implements RequestCallback{
	
	private static Request currRequest = null;
	private static ArrayList<RequestBuilder> requests = new ArrayList<RequestBuilder>();

	static IDocument document = null;
	private GETEV()	{}

	private static int counter = 0;

	public static void RegisterDocument(IDocument doc) {
		document = doc;
	}

	public static void RenewCheckChangesThread() {
		counter++;
		String url = Options.HOME_URL + "ev?sid=" + User.SID
				+ "&generation=" + counter;
		if (Options.context != "") {
			if (url.endsWith("?"))
				url += Options.context;
			else if (url.contains("?"))
				url += "&" + Options.context;
		}
		
		//url += "&track=true";
		
		RequestBuilder request = new RequestBuilder(RequestBuilder.GET, URL.encode(url));
		theGET = new GETEV();
		request.setCallback(theGET);

		if (request != null)
			requests.add(request);
		Update();
	}
	
	private static GETEV theGET = null;
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
		if (received == null || received.length() == 0)
			Log.write("CheckChanges RespondStatus="+response.getStatusCode()+"; Received: <empty>!");
		else {
			Log.write("CheckChanges RespondStatus="+response.getStatusCode()+"; Received: " + received);
			if (document != null) {
				JSONValue JSONreceived = JSONParser.parseLenient(received);
				if (JSONreceived != null)
				{
					Map<Integer, Suite> changedSuites = document.onCheckChanges(response);
					_AbstractView.UpdateSuiteGeoItems(changedSuites);
				}
			}
		}

		Update();
		RenewCheckChangesThread();
	}

	@Override
	public void onError(Request request, Throwable exception) {
		Log.write("Request Failed: " + exception.getMessage());
	}
	
};