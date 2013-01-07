package com.condox.vrestate.client;

import com.condox.vrestate.client.Event;
import com.condox.vrestate.client.GET;
import com.condox.vrestate.client.JSONParams;
import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONValue;
import com.google.gwt.user.client.Timer;
import com.pathf.gwt.util.json.client.JSONWrapper;

public class User {

	public static String SID;
	public static int keepAlivePeriodSec;
	private static boolean isReady = false;

	public static void Login() {
//		String request = Options.HOME_URL + "program?q=login&uid="
//		+ Options.USER_NAME + "&pwd=" + Options.USER_PASS;
		String request = Options.HOME_URL + "program?q=login&role=visitor&uid=web&pwd=web";
//		request = request.replace("listing/", "");
		Log.write(request);
		GET.send(request, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
//				Log.write("" + response.getStatusCode());
				String json = response.getText();
//				Log.write(json);
//				Log.write(response.getStatusText());
//				Log.write(response.getStatusText());
				JSONParams params = JSONParams.parse(json);
				SID = params.getString("sid");
				keepAlivePeriodSec = params.getInteger("keepalivePeriodSec");
//				Log.write(SID);
//				Log.write("" + keepAlivePeriodSec);
				isReady = true;
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
			}
		});
	}
	
	public static boolean isReady() {
		return isReady;
	};
	
	
}
