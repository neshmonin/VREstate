package com.condox.ecommerce.client;

import com.condox.clientshared.communication.Options;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.URL;

public class ServerProxy {
	
	// Get user info
	public static void getUserInfo(int id, String sid, RequestCallback callback) {
		String url = Options.URL_VRT + "data/user/" + id + "?&sid=" + sid;
		RequestBuilder builder = new RequestBuilder(RequestBuilder.GET, URL.encode(url));
		builder.setRequestData(null);
		builder.setCallback(callback);
		try {
			builder.send();
		} catch (RequestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}
