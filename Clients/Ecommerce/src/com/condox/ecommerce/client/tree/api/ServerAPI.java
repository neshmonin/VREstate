package com.condox.ecommerce.client.tree.api;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.utils.StringFormatter;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Timer;

public class ServerAPI implements RequestCallback {

	public static void login(String role, String uid, String pwd,
			RequestCallback callback) {
		// https://www.3dcondox.com/vre/program?q=login&role=visitor&uid=web&pwd=web
		String url = StringFormatter.format(Options.URL_VRT
				+ "program?q=login&role={0}&uid={1}&pwd={2}", role, uid, pwd);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(callback);
		send(builder);
	}

	private static void send(final RequestBuilder builder) {
		builder.setCallback(new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				log(response);
				builder.getCallback().onResponseReceived(request, response);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				builder.getCallback().onError(request, exception);
			}
		});

		try {
			builder.send();
		} catch (RequestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	private RequestType type = null;
	private I_RequestCallback callback = null;
	private static boolean logging = true;

	public RequestType getType() {
		return type;
	}

	// public void setType(RequestType type) {
	// this.type = type;
	// }

	private boolean busy = false;

	public void execute(final RequestType type, final JSONObject data,
			final I_RequestCallback callback) {
		if (busy)
			new Timer() {

				@Override
				public void run() {
					execute(type, data, callback);

				}
			}.schedule(1000);

		busy = true;
		this.type = type;
		this.callback = callback;
		switch (this.type) {
		case Login:
			login(data);
			break;
		case KeepSessionAlive:
			keepAlive(data);
			break;
		case GetUserInfo:
			getUserInfo(data);
			break;
		case SetUserInfo:
			setUserInfo(data);
			break;
		case GetViewOrders:
			getViewOrders(data);
			break;
		case GetSuiteInfo:
			getSuiteInfo(data);
			break;
		default:
			break;
		}
	}

	private void login(JSONObject data) {
		// https://www.3dcondox.com/vre/program?q=login&role=visitor&uid=web&pwd=web
		String role = data.get("role").isString().stringValue();
		String uid = data.get("uid").isString().stringValue();
		String pwd = data.get("pwd").isString().stringValue();
		String url = Options.URL_VRT + "program?q=login&role=" + role + "&uid="
				+ uid + "&pwd=" + pwd;
		log("Request: " + url);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	private void keepAlive(final JSONObject data) {
		busy = true;
		String sid = data.get("sid").isString().stringValue();
		String url = Options.URL_VRT + "program?q=sessionrenew&sid=" + sid;
		log("Request: " + url);
		final RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				new Timer() {

					@Override
					public void run() {
						keepAlive(data);
					}
				}.schedule(20000);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub

			}
		});
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	private void setUserInfo(JSONObject data) {
		String userId = data.get("userId").isString().stringValue();
		String userSID = data.get("userSID").isString().stringValue();
		String putData = data.get("putData").isString().stringValue();
		String url = Options.URL_VRT + "data/user/" + userId + "?sid="
				+ userSID;
		log("Request url: " + url);
		log("Request data: " + putData);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.PUT,
				URL.encode(url));
		builder.setRequestData(putData);
		builder.setHeader("Content-type", "application/json;");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	private void getUserInfo(JSONObject data) {
		String userId = data.get("userId").isString().stringValue();
		String userSID = data.get("userSID").isString().stringValue();
		String url = Options.URL_VRT + "data/user/" + userId + "?sid="
				+ userSID;
		log("Request: " + url);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	private void getViewOrders(JSONObject data) {
		String userId = data.get("userId").isString().stringValue();
		String userSID = data.get("userSID").isString().stringValue();
		String url = Options.URL_VRT + "data/viewOrder?userId=" + userId
				+ "&ed=Resale" + "&verbose=true" + "&sid=" + userSID;
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	private void getSuiteInfo(JSONObject data) {
		int suiteId = (int) data.get("suiteId").isNumber().doubleValue();
		String userSID = data.get("userSID").isString().stringValue();
		String url = Options.URL_VRT + "data/suite/" + suiteId + "?sid="
				+ userSID;
		log("Request: " + url);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(this);
		try {
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}
	}

	@Override
	public void onResponseReceived(Request request, Response response) {
		busy = false;
		log(response);
		if (response.getStatusCode() == 200)
			this.callback.onOK(JSONParser.parseStrict(response.getText())
					.isObject());
		else if (response.getStatusCode() == 304)
			this.callback.onOK(new JSONObject());
		else
			this.callback.onError();
		// switch (this.type) {
		// case GetUserInfo:
		// this.callback.onOK(JSONParser.parseStrict(response.getText()).isObject());
		// break;
		// default:
		// break;
		//
		// }
	}

	@Override
	public void onError(Request request, Throwable exception) {
		// TODO Auto-generated method stub
		busy = false;
	}

	private static void log(RequestBuilder builder) {
		log("Request: " + URL.decode(builder.getUrl()));
	}

	private static void log(Response response) {
		log("Response status code: " + response.getStatusCode());
		log("Response status text: " + response.getStatusText());
		log("Response text: " + response.getText());
	}

	private static void log(String message) {
		if (logging)
			Log.write(message);
	}

}
