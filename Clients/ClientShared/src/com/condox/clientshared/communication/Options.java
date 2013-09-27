package com.condox.clientshared.communication;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import com.condox.clientshared.abstractview.Log;
import com.google.gwt.dom.client.FrameElement;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.Window;

public class Options implements RequestCallback {

	public static enum MODE {
		TEST, WORK
	};

	public static MODE SERVER_MODE;

	public static String URL_VRT;
	public static String URL_STATIC;
	public static String URL_MODELS;

	// ======================================================>
	public enum ROLES {
		KIOSK, VISITOR
	};

	public static ROLES ROLE = ROLES.VISITOR;

	public static String URL_BUTTONS;
	public static int BUILDING_ID = -1;
	public static int SUITE_ID;
//	public static String HOME_URL;
	public static String ZOOM_UNZOOM_URL;
	public static String ZOOM_IN_URL;
	public static String ZOOM_OUT_URL;
	public static String URL_BUTTON_PANORAMIC_VIEW;
	public static String URL_BUTTON_EXIT_PANORAMIC_VIEW;
	public static String URL_BUTTON_CENTER_PANORAMIC_VIEW;
	public static String SUITE_INFO_TEMPLATE;
	public static FrameElement SUITE_INFO;
	public static Integer SUITE_DISTANCE;
	public static boolean USE_FILTER = true;
	public static boolean USE_HISTORICAL = false;
	// If SUPPORT_PAN is true, user can pan up and down a building
	public static boolean SUPPORT_PAN = false;
	public static String context = "";

	public static boolean isViewOrder() {
		return Options.getViewOrderId() != null;
	}

	public static void Init() {
		// ---------------------
		SERVER_MODE = MODE.TEST;

		switch (SERVER_MODE) {
		case TEST:
			URL_VRT = "https://vrt.3dcondox.com/vre/";
			break;
		case WORK:
			URL_VRT = "https://vrt.3dcondox.com/";
			break;
		}
		URL_STATIC = URL_VRT.replaceFirst("https://vrt.", "https://static.");
		URL_MODELS = URL_VRT.replaceFirst("https://vrt.", "https://models.");
		// ---------------------
		
		
		
		
		
		
		Map<String, List<String>> params = Window.Location.getParameterMap();
//		Log.write(params.toString());

		Map<String, List<String>> contextMap = new HashMap<String, List<String>>(
				params);
		// --------------------------------------------------//
		List<String> oneValueList = new ArrayList<String>();
		oneValueList.add("true");
		contextMap.put("includeImported", oneValueList);
		// --------------------------------------------------//

		if (params.containsKey("gwt.codesvr"))
			contextMap.remove("gwt.codesvr"); // if we run from Eclipse, better
												// strip this down
		if (params.containsKey("viewOrderId"))
			contextMap.remove("viewOrderId"); // we handle this parameter
												// outside the Init

		if (params.containsKey("UseFilter")) {
			USE_FILTER = Boolean.valueOf(params.get("UseFilter").get(0));
			contextMap.remove("UseFilter");
		}

		if (params.containsKey("UseHistorical")) {
			USE_HISTORICAL = Boolean
					.valueOf(params.get("UseHistorical").get(0));
			contextMap.remove("UseHistorical");
		}

		if (params.containsKey("BuildingId")) {
			BUILDING_ID = Integer.valueOf(params.get("BuildingId").get(0));
			contextMap.remove("BuildingId");
		}

		if (params.containsKey("SchemaPath")) {
			URL_BUTTONS = params.get("SchemaPath").get(0);
			contextMap.remove("BuildingId");
		} else
			URL_BUTTONS = URL_VRT + "buttons/";

		ZOOM_UNZOOM_URL = URL_BUTTONS + "ZoomUnzoomBar.png";
		ZOOM_IN_URL = URL_VRT + "buttons/Unzoom.png";
		ZOOM_OUT_URL = URL_VRT + "buttons/Zoom.png";
		URL_BUTTON_PANORAMIC_VIEW = URL_BUTTONS + "PanoramicView.png";
		URL_BUTTON_EXIT_PANORAMIC_VIEW = URL_BUTTONS + "Back.png";
		URL_BUTTON_CENTER_PANORAMIC_VIEW = URL_BUTTONS + "Center.png";

		if (params.containsKey("role")) {
			if (params.get("role").get(0).equals("kiosk"))
				ROLE = ROLES.KIOSK;
			contextMap.remove("role");
		}
		Log.write("ROLE:" + ROLE);

		Iterator<String> keyIterator = contextMap.keySet().iterator();
		while (keyIterator.hasNext()) {
			String key = keyIterator.next();
			String param = contextMap.get(key).get(0);
			context += key + "=" + param;
			if (keyIterator.hasNext())
				context += "&";
		}
		Log.write("context=" + context);
	};

	@Override
	public void onResponseReceived(Request request, Response response) {
		// SUITE_INFO_TEMPLATE = response.getText();
		// // SUITE_INFO= response.getText();
		// vrEstate.LoginUser();
	}

	@Override
	public void onError(Request request, Throwable exception) {
	}

	// public boolean isUrlReadable() {
	// if (params.containsKey("type")&&params.containsKey("id"))
	// return true;
	// return false;
	// }

	public static String getViewOrderId() {
		Map<String, List<String>> params = Window.Location.getParameterMap();
		if (params.containsKey("viewOrderId"))
			return params.get("viewOrderId").get(0);
		return null;
	}

	public static String getSiteId() {
		Map<String, List<String>> params = Window.Location.getParameterMap();
		if (params.containsKey("SiteId"))
			return params.get("SiteId").get(0);
		if (params.containsKey("type"))
			if (params.get("type").get(0).equals("site"))
				return params.get("id").get(0);

		return null;
	}

	public static String getBuildingId() {
		Map<String, List<String>> params = Window.Location.getParameterMap();
		if (params.containsKey("BuildingId"))
			return params.get("BuildingId").get(0);
		return null;
	}

	public static boolean getShowSold() {
		Map<String, List<String>> params = Window.Location.getParameterMap();
		if (params.containsKey("ShowSold")) {
			String flag = params.get("ShowSold").get(0);
			return flag.equalsIgnoreCase("true");
		}
		return false;
	}

}
