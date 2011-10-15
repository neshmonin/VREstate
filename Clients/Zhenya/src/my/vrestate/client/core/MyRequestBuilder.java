package my.vrestate.client.core;

import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;

public class MyRequestBuilder extends RequestBuilder {

	public MyRequestBuilder(Method httpMethod, String url) {
		super(httpMethod, url);
		// TODO Auto-generated constructor stub
	}
	public RequestCallback OriginalCallback = null;
}
