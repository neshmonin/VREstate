package com.condox.vrestate.maps.server;

import com.condox.clientshared.communication.Options;
import com.condox.clientshared.utils.StringFormatter;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.json.client.JSONObject;

public class ServerAPI {
	public static enum RequestType {
		USER_LOGIN, LOAD_DOCUMENT, GET_SUITES
	};

	public static Request execute(RequestType request, JSONObject data,
			RequestCallback callback) {
		RequestBuilder builder = null;
		switch (request) {
		case USER_LOGIN: {
			String uid = "web";
			String pwd = "web";
			String role = "visitor";
			String url = StringFormatter.format(Options.URL_VRT
					+ "program?q=login&uid={0}&pwd={1}&role={2}", uid, pwd,
					role);
			builder = new RequestBuilder(RequestBuilder.GET, url);
			builder.setCallback(callback);
		}
			break;
		case LOAD_DOCUMENT: {
			String type = data.get("type").isString().stringValue();
			String id = data.get("id").isString().stringValue();
			String sid = data.get("sid").isString().stringValue();
			String url = StringFormatter.format(Options.URL_VRT
					+ "data/view?type={0}&id={1}&sid={2}", type, id, sid);
			builder = new RequestBuilder(RequestBuilder.GET, url);
			builder.setCallback(callback);
		}
			break;
		}

		try {
			return builder.send();
		} catch (RequestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}
}
