package com.condox.clientshared.communication;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.utils.JSONParams;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONException;
import com.google.gwt.user.client.Timer;

public class User implements RequestCallback, I_Login {

	public static String SID;
	public static int keepAlivePeriodSec;
	public static User theUser = null;

	I_Login externalInterface = null;

	public static User Login(I_Login loginInterface) {
		String request = Options.HOME_URL + "program?q=login&role=visitor&uid=web&pwd=web"
											+ "&generation=" + counter++;

		theUser = new User(loginInterface);
		GET.send(request, theUser);
		return theUser;
	}

	public static User ReLogin() {
		return Login(null);
	}

	protected User(I_Login vrEstate) {
		if (vrEstate != null)
			this.externalInterface = vrEstate;
		else
			this.externalInterface = this;
	}
	
	private Timer keepAliveThread = null;
	private static int counter = 0;
	
	@Override
	public void onResponseReceived(Request request, Response response) {
		String json = response.getText();
		JSONParams params = JSONParams.parse(json);
		try {
			SID = params.getString("sid");
			keepAlivePeriodSec = params.getInteger("keepalivePeriodSec");
	
			keepAliveThread = new Timer() {
				@Override
				public void run() {
					String request = Options.HOME_URL + "program?q=sessionrenew&sid=" + SID
										+ "&generation=" + counter++;
					GET.send(request);
				}
			};
			keepAliveThread.scheduleRepeating(keepAlivePeriodSec*1000);
			
			externalInterface.onLoginSucceed();
		}
		catch(JSONException e){
			externalInterface.onLoginFailed(e);
		}
	
	}

	@Override
	public void onError(Request request, Throwable exception) {
		externalInterface.onLoginFailed(exception);
	}

	private static int reloginTimeSecs = 10;
	private static Timer reloginTimer = null;
	public static void Reconnect() {
		if (reloginTimer == null) {
			reloginTimer = new Timer() {
				@Override
				public void run() {
					User.ReLogin();
				}
			};
		}
		reloginTimer.schedule(reloginTimeSecs*1000);
	}

	@Override
	public void onLoginSucceed() {
		Log.write("ReLogin Secceed: SID=" + SID);
		reloginTimer.cancel();
		reloginTimer = null;
		UpdatesFromServer.RenewCheckChangesThread();
	}

	@Override
	public void onLoginFailed(Throwable exception) {
		Log.write("Failed to ReLogin: " + exception.toString());
		reloginTimer.schedule(reloginTimeSecs*1000);
	}
	
}
