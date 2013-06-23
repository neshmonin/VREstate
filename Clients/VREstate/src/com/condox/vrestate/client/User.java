package com.condox.vrestate.client;


import com.condox.vrestate.shared.Options;
import com.condox.vrestate.client.GET;
import com.condox.vrestate.client.JSONParams;
//import com.condox.vrestate.client.Log;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.Timer;

public class User implements RequestCallback {

	public static String SID;
	public static int keepAlivePeriodSec;
	public static User theUser = null;

	public static User Login(VREstate vrEstate) {
//		String request = Options.HOME_URL + "program?q=login&uid="
//		+ Options.USER_NAME + "&pwd=" + Options.USER_PASS;
		String request = Options.HOME_URL + "program?q=login&role=visitor&uid=web&pwd=web";
//		request = request.replace("listing/", "");

		theUser = new User(vrEstate);
		GET.send(request, theUser);
		return theUser;
	}

	VREstate vrEstate = null;
	
	protected User(VREstate vrEstate)
	{
		this.vrEstate = vrEstate;
	}
	
	private Timer keepAliveThread = null;
	
	@Override
	public void onResponseReceived(Request request, Response response) {
		String json = response.getText();
		JSONParams params = JSONParams.parse(json);
		SID = params.getString("sid");
		keepAlivePeriodSec = params.getInteger("keepalivePeriodSec");
		
		keepAliveThread = new Timer() {
			@Override
			public void run() {
				String request = Options.HOME_URL + "program?q=sessionrenew&sid=" + SID;
				GET.send(request);
			}
		};
		keepAliveThread.scheduleRepeating(keepAlivePeriodSec*1000);
		
		vrEstate.StartGE();
	
	}

	@Override
	public void onError(Request request, Throwable exception) {
		Log.write("Failed to Login");
	}
	
}
