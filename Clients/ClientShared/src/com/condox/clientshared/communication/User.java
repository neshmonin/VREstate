package com.condox.clientshared.communication;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.utils.JSONParams;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONException;
import com.google.gwt.user.client.Random;
import com.google.gwt.user.client.Timer;

public class User implements RequestCallback, I_Login {

	public enum UserRole {
		Visitor, Agent, SuperAdmin, SellingAgent
	}

	public static String SID;
	public static String id;
	public static UserRole role;
	public static int keepAlivePeriodSec;
	public static User theUser = null;

	private static boolean firstLogin = true;

	private static String request;

	I_Login externalInterface = null;

	public static User Login(I_Login loginInterface, String uid, String pwd,
			UserRole role) {
		User.role = role;

		firstLogin = true;

		User.request = Options.URL_VRT + "program?q=login";
		switch (role) {
		case Visitor:
			User.request += "&role=visitor";
			break;
		case Agent:
			User.request += "&role=agent";
			break;
		case SuperAdmin:
			User.request += "&role=superadmin";
			break;
		case SellingAgent:
			User.request += "&role=sellingagent";
			break;
		default:
		}
		if (uid == null || uid.isEmpty())
			User.request += "&uid=web&pwd=web";
		else {
			User.request += "&uid=" + uid + "&pwd=" + pwd;
		}
		theUser = new User(loginInterface);
		GET.send(User.request + "&generation=" + counter++, theUser);
		return theUser;
	}

	public static void ReLogin() {

		firstLogin = false;

		GET.send(User.request + "&generation=" + counter++, theUser);
	}

	protected User(I_Login vrEstate) {
		if (vrEstate != null)
			this.externalInterface = vrEstate;
		else
			this.externalInterface = this;
	}

	private Timer keepAliveThread = null;
	private static int counter = Random.nextInt(Integer.MAX_VALUE);

	@Override
	public void onResponseReceived(Request request, Response response) {
		String json = response.getText();

		if (json.isEmpty() && firstLogin) {
			externalInterface.onLoginFailed(null);
			return;
		}

		JSONParams params = JSONParams.parse(json);
		try {
			SID = params.getString("sid");
			id = String.valueOf(params.getInteger("userId"));
			keepAlivePeriodSec = params.getInteger("keepalivePeriodSec");

			keepAliveThread = new Timer() {
				@Override
				public void run() {
					String request = Options.URL_VRT
							+ "program?q=sessionrenew&sid=" + SID
							+ "&generation=" + counter++;
					GET.send(request);
				}
			};
			keepAliveThread.scheduleRepeating(keepAlivePeriodSec * 1000);

			if (firstLogin)
				externalInterface.onLoginSucceed();
		} catch (JSONException e) {
			if (firstLogin)
				externalInterface.onLoginFailed(e);
		}

	}

	@Override
	public void onError(Request request, Throwable exception) {
		if (firstLogin)
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
		reloginTimer.schedule(reloginTimeSecs * 1000);
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
		reloginTimer.schedule(reloginTimeSecs * 1000);
	}

}
