package com.condox.clientshared.communication;

import com.condox.clientshared.utils.JSONParams;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.Timer;

public class User implements RequestCallback {

	public static String SID;
	public static int keepAlivePeriodSec;
	public static User theUser = null;

	I_Login vrEstate = null;

	public static User Login(I_Login vrEstate) {
//		String request = Options.HOME_URL + "program?q=login&uid="
//		+ Options.USER_NAME + "&pwd=" + Options.USER_PASS;
		String request = Options.HOME_URL + "program?q=login&role=visitor&uid=web&pwd=web";
//		request = request.replace("listing/", "");

		theUser = new User(vrEstate);
		GET.send(request, theUser);
		return theUser;
	}

	protected User(I_Login vrEstate)
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
		
		vrEstate.onLoginSucceed();
	
	}

	@Override
	public void onError(Request request, Throwable exception) {
		vrEstate.onLoginFailed();
	}
	
}
