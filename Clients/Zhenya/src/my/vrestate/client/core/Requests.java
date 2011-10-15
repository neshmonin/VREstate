package my.vrestate.client.core;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import my.vrestate.client.Options;

import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestBuilder;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.http.client.Response;
import com.google.gwt.http.client.URL;



public class Requests {
	public static String SERVER_URL = null;
	public static String FUNC = null;
	public static Map<String, String> PARAMS = new HashMap<String, String>();
	
	public static ArrayList<MyRequestBuilder> Query = new ArrayList<MyRequestBuilder>();

	public static void Add(String request, RequestCallback onResponse) {
//		String Request = Options.SERVER_URL + FUNC + "?";
		
//		for (Iterator<Map.Entry<String, String>> iter = PARAMS.entrySet().iterator(); iter.hasNext(); ) {
//			Map.Entry<String, String> param = iter.next();
//            Request += (URL.encode(param.getKey()) + "=" + URL.encode(param.getValue()));
//            if (iter.hasNext()) Request += "&";
//        }
		String Request = Options.SERVER_URL + request;
		Request = URL.encode(Request);
		
		MyRequestBuilder builder = new MyRequestBuilder(RequestBuilder.GET, Request);
		builder.OriginalCallback = onResponse;
		builder.setCallback(MyCallback);
		builder.setRequestData(null);
		GWT.log(builder.getUrl());
		if (Query.isEmpty()) {
			Query.add(builder);
			try {
				Query.get(0).send();
			} catch (RequestException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}else{
			Query.add(builder);
		}
	};

	private static RequestCallback MyCallback = new RequestCallback(){
		@Override
		public void onResponseReceived(Request request, Response response) {
			Query.get(0).OriginalCallback.onResponseReceived(request, response);
			Query.remove(0);
			if (!Query.isEmpty())
				try {
					Query.get(0).send();
				} catch (RequestException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			// TODO Auto-generated method stub
		}

		@Override
		public void onError(Request request, Throwable exception) {
			// TODO Auto-generated method stub
			
		}


	};
}
