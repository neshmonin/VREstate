package com.condox.ecommerce.client.tree.api;

import java.util.Date;

import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.utils.StringFormatter;
import com.condox.ecommerce.client.model.LoginModel;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;
import com.google.gwt.i18n.client.DateTimeFormat;
import com.google.gwt.i18n.client.TimeZone;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Timer;

public class ServerAPI implements RequestCallback {

	public static Request login(LoginModel loginModel, RequestCallback callback) {
		// https://www.3dcondox.com/vre/program?q=login&role=visitor&uid=web&pwd=web
		// -----------------------------
		String role = "";
		switch (loginModel.getRole()) {
		case Visitor:
			role = "visitor";
			break;
		case SellingAgent:
			role = "agent";
			break;
		case SuperAdmin:
			role = "superadmin";
			break;
		default:
			break;
		}
		String uid = loginModel.getUid();
		String pwd = loginModel.getPwd();
		// if (uid.isEmpty() && pwd.isEmpty()) {
		// uid = "web";
		// pwd = "web";
		// role = "visitor";
		// }
		// -----------------------------
		String url = StringFormatter.format(Options.URL_VRT
				+ "program?q=login&role={0}&uid={1}&pwd={2}", role, uid, pwd);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData("");
		builder.setCallback(callback);
		return send(builder);
	}

	private static Request send(final RequestBuilder builder) {
		log(builder);
		final RequestCallback original = builder.getCallback();
		builder.setCallback(new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				log(response);
				original.onResponseReceived(request, response);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				original.onError(request, exception);
			}
		});

		try {
			return builder.send();
		} catch (RequestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
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
			this.callback.onSuccess(JSONParser.parseStrict(response.getText())
					.isObject());
		else if (response.getStatusCode() == 304)
			this.callback.onSuccess(new JSONObject());
		else
			this.callback.onError("");
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

	// private static void log(String message) {
	// if (logging)
	// Log.write(message);
	// }

	// ===============================================================
	private final static String BASE_URL = Options.URL_VRT;

	// private final static String BASE_URL = "https://vrt.3dcondox.com/vre/";

	public static void userLogin(UserRole role, String uid, String pwd,
			final I_RequestCallback callback) {
		// Role
		String sRole = "";
		switch (role) {
		case Visitor:
			sRole = "visitor";
			break;
		case Agent:
			sRole = "agent";
			break;
		case SuperAdmin:
			sRole = "superadmin";
			break;
		}
		// https://www.3dcondox.com/vre/program?q=login&uid=web&pwd=web&role=visitor
		// -----------------------------
		String url = "";
		if (uid.isEmpty() && pwd.isEmpty())
			url = StringFormatter.format(BASE_URL
					+ "program?q=login&uid={0}&pwd={1}&role={2}", "web", "web",
					"visitor");
		else
			url = StringFormatter.format(BASE_URL
					+ "program?q=login&uid={0}&pwd={1}&role={2}", uid, pwd,
					sRole);

		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET, url);
		execute(builder, callback);
	}

	public static void registerViewOrder(String ownerId,
			EcommerceTree.ListingType product, String mlsId, int propertyId,
			String sid, final I_RequestCallback callback) {
		
		String sProduct = "";
		switch (product) {
		case PRIVATE:
			sProduct = "prl";
			break;
		case PUBLIC:
			sProduct = "pul";
			break;
		}
		
		String url = StringFormatter.format(BASE_URL + "program?q=register"
				+ "&entity=viewOrder&ownerId={0}" + "&daysValid=1"
				+ "&product={1}" + "&options=fp" + "&mls_id={2}"
				+ "&propertyType=suite" + "&propertyId={3}" + "&sid={4}",
				ownerId, sProduct, mlsId, propertyId, sid);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET, url);
		execute(builder, callback);
	}
	
	public static void requestSuite(int id, String sid, I_RequestCallback callback) {
		String url = StringFormatter.format(BASE_URL + "data/suite/{0}?sid={1}", id, sid);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET, url);
		execute(builder, callback);
	}
	
	public static void updateSuite(int id, String sid, String data, I_RequestCallback callback) {
		String url = StringFormatter.format(BASE_URL + "data/suite/{0}?sid={1}", id, sid);
		RequestBuilder builder = new RequestBuilder(RequestBuilder.PUT, url);
		builder.setHeader("Content-type", "application/json");
		builder.setRequestData(data);
		log("Request data: " + data);
		execute(builder, callback);
	}
	
	public static void removeViewOrder(String id, String sid, I_RequestCallback callback) {
			String url = StringFormatter.format(BASE_URL + "data/viewOrder/{0}?sid={1}",id,sid);
			RequestBuilder builder = new RequestBuilder(RequestBuilder.DELETE, url);
			execute(builder, callback);
	}

	// public static void fetchViewData(ViewType type, String id, String sid,
	// final I_RequestCallback callback) {
	// String sType = "";
	// switch (type) {
	// case VIEW_ORDER:
	// sType = "viewOrder";
	// break;
	// case SITE:
	// sType = "site";
	// break;
	// }
	// String url = "";
	// url = StringFormatter.format(BASE_URL
	// + "/data/view?type={0}&id={1}&track=true&SID={2}", sType, id,
	// sid);
	// RequestBuilder builder = new RequestBuilder(RequestBuilder.GET, url);
	// execute(builder, callback);
	// }

	private static void execute(RequestBuilder builder,
			final I_RequestCallback callback) {
		builder.setCallback(new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				log("Response status: " + response.getStatusCode() + ": "
						+ response.getStatusText());
				log("Response: " + response.getText());
				if (response.getStatusCode() != Response.SC_OK)
					callback.onError(response.getStatusCode() + ":"
							+ response.getStatusText());
				else {
					String json = response.getText();
					// if (json.isEmpty())
					JSONObject obj = JSONParser.parseStrict(json).isObject();
					callback.onSuccess(obj);
				}
			}

			@Override
			public void onError(Request request, Throwable exception) {
				callback.onError("Some unknown error.");
			}
		});

		log("Send: " + builder.getUrl());
		try {
			builder.send();
		} catch (RequestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	private static void log(String message) {
		Date date = new Date();
		DateTimeFormat dtf = DateTimeFormat.getFormat("yyyy/MM/dd HH:mm:ss");
		GWT.log(dtf.format(date, TimeZone.createTimeZone(0)) + " - " + message);
	}

}
