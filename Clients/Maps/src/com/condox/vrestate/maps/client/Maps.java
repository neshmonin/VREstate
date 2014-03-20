package com.condox.vrestate.maps.client;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.document.Building;
import com.condox.clientshared.document.IDocument;
import com.condox.clientshared.document.Suite;
import com.condox.clientshared.utils.StringFormatter;
import com.condox.vrestate.maps.server.ServerAPI;
import com.condox.vrestate.maps.server.ServerAPI.RequestType;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.dom.client.Document;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.Window;
import com.google.maps.gwt.client.GoogleMap;
import com.google.maps.gwt.client.InfoWindow;
import com.google.maps.gwt.client.InfoWindowOptions;
import com.google.maps.gwt.client.LatLng;
import com.google.maps.gwt.client.MapOptions;
import com.google.maps.gwt.client.MapTypeId;
import com.google.maps.gwt.client.Marker;
import com.google.maps.gwt.client.Marker.ClickHandler;
import com.google.maps.gwt.client.MarkerOptions;
import com.google.maps.gwt.client.MouseEvent;

/**
 * Entry point classes define <code>onModuleLoad()</code>.
 */
public class Maps implements EntryPoint {
	public static String sid;

	public void onModuleLoad() {
		Options.Init();
		// -------------
		Window.alert("Start user login.");
		ServerAPI.execute(RequestType.USER_LOGIN, null, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				String message = StringFormatter.format("{0}:{1}\n{2}",
						response.getStatusCode(), response.getStatusText(),
						response.getText());
				Window.alert(message);
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				sid = obj.get("sid").isString().stringValue();
				Window.alert("User login OK.");
				onUserLogined();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub

			}
		});

		// -----------------------------------------

	}

	private void onUserLogined() {
		String type = Options.isViewOrder() ? "viewOrder" : "site";
		String id = Options.isViewOrder() ? Options.getViewOrderId() : Options
				.getSiteId();
		Log.write(type);
		Log.write(id);
		// String sid = ""; // TODO
		JSONObject obj = new JSONObject();
		obj.put("type", new JSONString(type));
		obj.put("id", new JSONString(id));
		obj.put("sid", new JSONString(sid));
		Window.alert("Start load document.");
		ServerAPI.execute(RequestType.LOAD_DOCUMENT, obj,
				new RequestCallback() {

					@Override
					public void onResponseReceived(Request request,
							Response response) {
						// TODO Auto-generated method stub
						String message = StringFormatter.format("{0}:{1}\n{2}",
								response.getStatusCode(), response.getStatusText(),
								response.getText());
						Window.alert(message);
						
						String json = response.getText();
						

						com.condox.clientshared.document.Document.progressBar = new MyProgress();
						IDocument doc = com.condox.clientshared.document.Document
								.get();
						doc.Parse(json);
						Window.alert("Document parsed OK.");
						onDocumentParsed();
					}

					@Override
					public void onError(Request request, Throwable exception) {
						// TODO Auto-generated method stub

					}
				});
	}

	private void onDocumentParsed() {
		Window.alert("Start showing maps.");
		// ----------------------
		LatLng myLatLng = LatLng.create(-34.397, 150.644);
		MapOptions myOptions = MapOptions.create();
		myOptions.setZoom(8.0);
		myOptions.setCenter(myLatLng);
		myOptions.setMapTypeId(MapTypeId.ROADMAP);
		final GoogleMap map = GoogleMap.create(
				Document.get().getElementById("map_canvas"), myOptions);

		// MarkerOptions newMarkerOpts = MarkerOptions.create();
		// newMarkerOpts.setPosition(myLatLng);
		// newMarkerOpts.setMap(map);
		// newMarkerOpts.setTitle("Hello World!");
		// Marker.create(newMarkerOpts);
		// ----------------------------------------------
		// DrawingManager manager = DrawingManager.create();
		// manager.setMap(map);
		IDocument doc = com.condox.clientshared.document.Document.get();
		for (Building item : doc.getBuildings().values()) {
			LatLng pos = LatLng.create(item.getCenter().getLatitude(), item
					.getCenter().getLongitude());
			MarkerOptions newMarkerOpts = MarkerOptions.create();
			newMarkerOpts.setPosition(pos);
			newMarkerOpts.setMap(map);
			newMarkerOpts.setTitle(item.getName());
			final Marker marker = Marker.create(newMarkerOpts);
			// -----------------------
			map.setCenter(pos);
			// ------------------------
			marker.addClickListener(new ClickHandler() {
				
				@Override
				public void handle(MouseEvent event) {
					InfoWindowOptions infowindowOpts = InfoWindowOptions.create();
					String content = "<h1><b>This suites are hard-coded, working now</b></h1><br>";
//					for (Suite suite : doc.getSuites().values())
//						content += "<button>" + suite.getName() + "</button>";
					for (int i = 0; i < 100; i++)
						content += "<button>" + i + "</button>";
					infowindowOpts.setContent(content);
					final InfoWindow infowindow = InfoWindow.create(infowindowOpts);
					infowindow.open(map, marker);
				}
			});
		}

	}
}
