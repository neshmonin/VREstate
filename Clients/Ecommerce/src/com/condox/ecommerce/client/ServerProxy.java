package com.condox.ecommerce.client;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.DELETE;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;

public class ServerProxy {

	private static void GET(String url, String data,
			final RequestCallback callback) {
		final RequestBuilder builder = new RequestBuilder(RequestBuilder.GET,
				URL.encode(url));
		builder.setRequestData(data);
		builder.setCallback(new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				ResponseLog(response);
				callback.onResponseReceived(request, response);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				ErrorLog();
				callback.onError(request, exception);
			}
		});
		try {
			RequestLog(builder);
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}

	}

	private static void DELETE(String url, String data,
			final RequestCallback callback) {
		final RequestBuilder builder = new RequestBuilder(RequestBuilder.DELETE,
				URL.encode(url));
		builder.setRequestData(data);
		builder.setHeader("Content-type", "application/json;");
		builder.setCallback(new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				ResponseLog(response);
				callback.onResponseReceived(request, response);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				ErrorLog();
				callback.onError(request, exception);
			}
		});
		try {
			RequestLog(builder);
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}

	}

	private static void PUT(String url, String data,
			final RequestCallback callback) {
		final RequestBuilder builder = new RequestBuilder(RequestBuilder.PUT,
				URL.encode(url));
		builder.setRequestData(data);
		builder.setHeader("Content-type", "application/json;");
		builder.setCallback(new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				ResponseLog(response);
				callback.onResponseReceived(request, response);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				ErrorLog();
				callback.onError(request, exception);
			}
		});
		try {
			RequestLog(builder);
			builder.send();
		} catch (RequestException e) {
			e.printStackTrace();
		}

	}

	// Recover forgotted password
//	public static void recoverPassword(String id, final RequestCallback callback) {
//		Log.write("Recover forgotted password:");
//		String url = Options.URL_VRT + "program?q=recover"
//				+ "&role=sellingagent&uid=" + id;
//		GET(url, null, callback);
//	}

//	// Get user info
//	public static void getUserInfo(String id, String sid,
//			final RequestCallback callback) {
//		Log.write("Get User info:");
//		String url = Options.URL_VRT + "data/user/" + id + "?sid=" + sid;
//		GET(url, null, callback);
//	}

	// Set user info
	public static void setUserInfo(String id, String info, String sid,
			final RequestCallback callback) {
		Log.write("Set User info:");
		String url = Options.URL_VRT + "data/user/" + id + "?sid=" + sid;
		PUT(url, info, callback);
	}

//	// Get orders list
//	public static void getOrdersList(String user_id, String sid,
//			final RequestCallback callback) {
//		Log.write("Get Orders list:");
//		String url = Options.URL_VRT + "data/viewOrder?userId=" + user_id
//				+ "&ed=Resale" + "&verbose=true" + "&sid=" + sid;
//		GET(url, null, callback);
//	}
	
	// Delete order
		public static void deleteOrder(String order_id, String sid,
				final RequestCallback callback) {
			Log.write("Delete order:");
			
			String url = Options.URL_VRT + "data/viewOrder/" + order_id
					 + "?sid=" + sid;
			DELETE(url, null, callback);
		}
	
	// Get suite info from MLS#
	public static void getSuiteInfoFromMLS(String mls, String sid,
			final RequestCallback callback) {
		Log.write("Get suite info from MLS#:");
		String url = Options.URL_VRT + "data/inventory?mlsId=" + mls
				+ "&sid=" + sid;
		GET(url, null, callback);
	}
	

	// Utils
	private static void RequestLog(RequestBuilder builder) {
		Log.write(" Request method: " + builder.getHTTPMethod());
		Log.write(" Request header (Content-type): " + builder.getHeader("Content-type"));
		Log.write(" Request url: " + builder.getUrl());
		Log.write(" Request data: " + builder.getRequestData());
	}

	private static void ResponseLog(Response response) {
		Log.write(" Response code: " + response.getStatusCode());
		Log.write(" Response text: " + response.getText());
	}

	private static void ErrorLog() {
		// Log.write(caption);
	}
}
