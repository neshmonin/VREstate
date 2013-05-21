package com.condox.vrestate.client;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import com.google.gwt.core.client.GWT;
import com.google.gwt.dom.client.FrameElement;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.Window;

public class Options implements RequestCallback {
	public enum ROLES {KIOSK, VISITOR};
	public static ROLES ROLE = ROLES.VISITOR;

	public static boolean DEBUG_MODE = false;

	public static String URL_VRT;
	public static String URL_STATIC;
	public static String URL_MODEL;

	public static String URL_BUTTONS;
	public static int BUILDING_ID = -1;
	public static int SUITE_ID;
	public static String HOME_URL;
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

	private Options(VREstate vrEstate) {
	}

	public static boolean isViewOrder() {
		return Options.getViewOrderId() != null;
	}

	public static String context = "";
	public static void Init(VREstate vrEstate) {
		Map<String, List<String>> params = Window.Location.getParameterMap();
		Log.write(params.toString());

		Map<String, List<String>> contextMap = new HashMap<String, List<String>>(params);
		//--------------------------------------------------//
		List<String> oneValueList = new ArrayList<String>();
		oneValueList.add("true");
		contextMap.put("includeImported", oneValueList);
		//--------------------------------------------------//

		if (params.containsKey("gwt.codesvr"))			
			contextMap.remove("gwt.codesvr"); // if we run from Eclipse, better strip this down
		if (params.containsKey("viewOrderId"))			
			contextMap.remove("viewOrderId"); // we handle this parameter outside the Init
		
		if (params.containsKey("UseFilter")) {			
			USE_FILTER = Boolean.valueOf(params.get("UseFilter").get(0));
			contextMap.remove("UseFilter");
		}

		if (params.containsKey("UseHistorical")) {			
			USE_HISTORICAL = Boolean.valueOf(params.get("UseHistorical").get(0));
			contextMap.remove("UseHistorical");
		}

		if (params.containsKey("BuildingId")) {			
			BUILDING_ID = Integer.valueOf(params.get("BuildingId").get(0));
			contextMap.remove("BuildingId");
		}

		if (BUILDING_ID != -1) {
			if (params.containsKey("test")) {
				DEBUG_MODE = Boolean.valueOf(params.get("test").get(0));
				contextMap.remove("test");
			}
		}
		else
			DEBUG_MODE = (GWT.getModuleBaseURL().contains("/vre/"));

		Log.write("DEBUG_MODE=" + DEBUG_MODE);

		if (DEBUG_MODE) {
			URL_VRT = "https://vrt.3dcondox.com/vre/";
			URL_STATIC = "https://static.3dcondox.com/vre/";
			URL_MODEL = "https://models.3dcondox.com/vre/";
		} else {
			URL_VRT = "https://vrt.3dcondox.com/";
			URL_STATIC = "https://static.3dcondox.com/";
			URL_MODEL = "https://models.3dcondox.com/";
		}

		if (params.containsKey("SchemaPath")) {			
			URL_BUTTONS = params.get("SchemaPath").get(0);
			contextMap.remove("BuildingId");
		}
		else URL_BUTTONS = URL_VRT + "buttons/";
		
		HOME_URL = URL_VRT;

		ZOOM_UNZOOM_URL = URL_BUTTONS + "ZoomUnzoomBar.png";
		ZOOM_IN_URL = HOME_URL + "buttons/Unzoom.png";
		ZOOM_OUT_URL = HOME_URL + "buttons/Zoom.png";
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
		
		vrEstate.LoginUser();
	};

	@Override
	public void onResponseReceived(Request request, Response response) {
//		SUITE_INFO_TEMPLATE = response.getText();
////		SUITE_INFO= response.getText();
//		vrEstate.LoginUser();
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
