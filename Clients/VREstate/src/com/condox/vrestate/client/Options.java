package com.condox.vrestate.client;

import java.util.List;
import java.util.Map;

import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.Window;

public class Options implements RequestCallback{
	public static boolean DEBUG_MODE = true;

	public static String URL_VRT;
	public static String URL_STATIC;
	public static String URL_MODEL;

	public static String URL_BUTTONS;
	public static String URL_BUTTON_UNZOOM;
	public static String URL_BUTTON_ZOOM;
//	public static String URL_BUTTON_PANORAMIC_VIEW;
//	public static String URL_BUTTON_EXIT_PANORAMIC_VIEW;
//	public static String URL_BUTTON_CENTER_PANORAMIC_VIEW;
	
	
	public static int BUILDING_ID;
	public static int SUITE_ID;
	public static String HOME_URL;
	public static String ZOOM_IN_URL;
	public static String ZOOM_OUT_URL;
	public static String URL_BUTTON_PANORAMIC_VIEW;
	public static String URL_BUTTON_EXIT_PANORAMIC_VIEW;
	public static String URL_BUTTON_CENTER_PANORAMIC_VIEW;
	public static String SUITE_INFO_TEMPLATE;
	public static Integer SUITE_DISTANCE;
	public static boolean SHOW_SOLD = false;
	public static boolean USE_FILTER = true;

	private static Options theOptions = null;
	private VREstate vrEstate = null;

	private Options(VREstate vrEstate)
	{
		this.vrEstate = vrEstate;
	}
	
	public static boolean isViewOrder()
	{
		return Options.getViewOrderId() != null;
	}
	 
	
	public static void Init(VREstate vrEstate) {
		Map<String, List<String>> params = Window.Location.getParameterMap();
		// DEVELOPER_ID = params.containsKey("DeveloperId") ? "Developer#"
		// + params.get("DeveloperId").get(0) : "-1";

//		SITE_ID = params.containsKey("SiteId") ? Integer.valueOf(params.get(
//				"SiteId").get(0)) : -1;

		BUILDING_ID = params.containsKey("BuildingId") ? Integer.valueOf(params
				.get("BuildingId").get(0)) : -1;

//		SUITE_ID = params.containsKey("SuiteId") ? Integer.valueOf(params.get(
//				"SuiteId").get(0)) : -1;

		SHOW_SOLD = params.containsKey("ShowSold") ? Boolean.valueOf(params
				.get("ShowSold").get(0)) : false;
		USE_FILTER = params.containsKey("UseFilter") ? Boolean.valueOf(params
				.get("UseFilter").get(0)) : true;

		// BUILDING_ID = params.containsKey("BuildingId") ? "Building#"
		// + params.get("BuildingId").get(0) : "-1";
		// SUITE_ID = params.containsKey("SuiteId") ? "Suite#"
		// + params.get("SuiteId").get(0) : "-1";
		// SUITE_DISTANCE = Integer.valueOf(params.containsKey("Distance")?
		// params.get("Distance").get(0) : "-1");

		if (BUILDING_ID != -1)
			DEBUG_MODE = params.containsKey("test") ? Boolean.valueOf(params
					.get("test").get(0)) : false;
		else
	    	DEBUG_MODE = (GWT.getModuleBaseURL().contains("/vre/")); 

		Log.write("DEBUG_MODE=" + DEBUG_MODE);
	    
		if (DEBUG_MODE) {
			URL_VRT = "https://vrt.3dcondox.com/vre/";
			URL_STATIC = "https://static.3dcondox.com/vre/";
			URL_MODEL = "https://model.3dcondox.com/vre/";
		} else {
			URL_VRT = "https://vrt.3dcondox.com/";
			URL_STATIC = "https://static.3dcondox.com/";
			URL_MODEL = "https://model.3dcondox.com/";
		}

		URL_BUTTONS = URL_VRT + "buttons/";
		URL_BUTTON_UNZOOM = URL_BUTTONS + "Unzoom.png";
		URL_BUTTON_ZOOM = URL_BUTTONS + "Zoom.png";
//		URL_BUTTON_PANORAMIC_VIEW = URL_BUTTONS + "PanoramicView.png";
//		URL_BUTTON_EXIT_PANORAMIC_VIEW = URL_BUTTONS + "Back.png";
//		URL_BUTTON_CENTER_PANORAMIC_VIEW = URL_BUTTONS + "Center.png";
		
		HOME_URL = URL_VRT;
		
		ZOOM_IN_URL = HOME_URL + "buttons/Unzoom.png";
		ZOOM_OUT_URL = HOME_URL + "buttons/Zoom.png";
		URL_BUTTON_PANORAMIC_VIEW = HOME_URL + "buttons/PanoramicView.png";
		URL_BUTTON_EXIT_PANORAMIC_VIEW = HOME_URL + "buttons/Back.png";
		URL_BUTTON_CENTER_PANORAMIC_VIEW = HOME_URL + "buttons/Center.png";
		// SUITE_INFO_TEMPLATE = HOME_URL + "templates/SuiteInfo.html";
		String request = HOME_URL + "ReducedInfo.html";
		
//		Log.write(request);
		theOptions = new Options(vrEstate);
		GET.send(request, theOptions);
//		 isReady = true;
	};

	@Override
	public void onResponseReceived(Request request, Response response) {
		// TODO Auto-generated method stub
		SUITE_INFO_TEMPLATE = response.getText();
//		Log.write(SUITE_INFO_TEMPLATE);
		vrEstate.LoginUser();
	}

	@Override
	public void onError(Request request, Throwable exception) {
		// TODO Auto-generated method stub
	}
	
//	public boolean isUrlReadable() {
//		if (params.containsKey("type")&&params.containsKey("id"))
//			return true;
//		return false;
//	}
	
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
		if (params.containsKey("ShowSold"))
		{
			String flag = params.get("ShowSold").get(0);
			return flag.equalsIgnoreCase("true");
		}
		return false;
	}

}
