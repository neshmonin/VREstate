package com.condox.vrestate.client;

import com.google.gwt.core.client.GWT;
import com.google.gwt.user.client.Window;


public class VRE {
	
	public final static boolean RELEASE = GWT.getModuleBaseURL().contains("vrt.3dcondox.com");
	
	public static int DEVELOPER_ID = -1;
	public static int SITE_ID = -1;
	public static int BUILDING_ID = -1;
	public static int SUITE_ID = -1;
	
	public static String VRT;
	
	public static String MODELS; 
	public static String DEFAULTS;
	public static String OVERLAYS;
	
	public static String MODEL_KMZ_URL; 
	public static String DEFAULT_KML_URL; 
	public static String OVERLAY_KMZ_URL; 
	public static String SUITE_CLASSES_DIR; 
	// Options
	public static boolean OPTIONS_SHOW_SOLD_STATUS = false;
	public static boolean OPTIONS_USE_FILTER = true;
	public static boolean OPTIONS_NAVIGATION_CONTROLS = true;
	
	// Разные константы
	
	/*	Минимальное расстояние, на которое может приближаться камера
	 * к осматриваемому объекту. Используется в SiteView*/
	public static int LOOKAT_MIN_RANGE = 0;
	public static String WINDOW_TITLE = "Brand New Project! (VRT v.1.0.0.00017)";
//	public static String WINDOW_TITLE = "";
	
	//=======================================================
	public static final String EARTH_API_KEY = "ABQIAAAAm7LIvLNR-PkJLewH4qmS7hREGtQZq9OFJfHndXhPP8gxXzlLARQtA_EfZjc9zs77WO25FrLcaZ4ZVA";

	public static String BUTTON_PANORAMIC_VIEW;
	public static String BUTTON_BACK_VIEW;
	public static String BUTTON_CENTER_VIEW;
	public static String BUTTON_HELP;
	public static String BUTTON_ZOOM_IN;
	public static String BUTTON_ZOOM_OUT;
	public static String BUTTON_DISCLAIMER_TEXT;
	
	public static void Init() {
		if (RELEASE)
			VRT = GWT.getModuleBaseURL().replace("vrestate/", "");
		else
			VRT = "https://vrt.3dcondox.com/vre/";
		
		String DeveloperId = Window.Location.getParameter("DeveloperId");
		if (DeveloperId != "")
			DEVELOPER_ID = Integer.valueOf(DeveloperId);
		
		MODELS = (VRT.contains("vrt"))? VRT.replace("vrt", "models") : VRT.replace("server", "models"); 
		DEFAULTS = (VRT.contains("vrt"))? VRT.replace("vrt", "static") : VRT.replace("server", "static");
		OVERLAYS = (VRT.contains("vrt"))? VRT.replace("vrt", "static") : VRT.replace("server", "static");

		MODEL_KMZ_URL = MODELS+"SuperServer/"+DEVELOPER_ID+"/"+SITE_ID+"/Model.kmz"; 
		DEFAULT_KML_URL = DEFAULTS+"SuperServer/"+DEVELOPER_ID+"/"+SITE_ID+"/Default.kml"; 
		OVERLAY_KMZ_URL = OVERLAYS+"SuperServer/"+DEVELOPER_ID+"/"+SITE_ID+"/Overlay.kmz";
		
		SUITE_CLASSES_DIR = VRT+"SuperServer/"+DEVELOPER_ID+"/"+SITE_ID+"/SuitesWeb/"; 
		
//		LOOKAT_MIN_RANGE = 500;
//		if ((DEVELOPER_ID==5)&&(SITE_ID==9)) WINDOW_TITLE = "Devonwood";
		if ((DEVELOPER_ID==1)&&(SITE_ID==4)) WINDOW_TITLE = "Eden Park Towers";
		if ((DEVELOPER_ID==1)&&(SITE_ID==5)) WINDOW_TITLE = "Key West";
		if ((DEVELOPER_ID==2)&&(SITE_ID==7)) WINDOW_TITLE = "The Vue";
		if ((DEVELOPER_ID==4)&&(SITE_ID==8)) WINDOW_TITLE = "London Eye";
		if ((DEVELOPER_ID==6)&&(SITE_ID==18)) WINDOW_TITLE = "Mimico";
		WINDOW_TITLE += " (VRT v.1.0.0.17)";
		

		BUTTON_PANORAMIC_VIEW = VRT + "buttons/PanoramicView.png";
		BUTTON_BACK_VIEW = VRT + "buttons/Back.png";
		BUTTON_CENTER_VIEW = VRT + "buttons/Center.png";
		BUTTON_HELP = VRT + "buttons/Help.png";
		BUTTON_ZOOM_IN = VRT + "buttons/Unzoom.png";
		BUTTON_ZOOM_OUT = VRT + "buttons/Zoom.png";
		BUTTON_DISCLAIMER_TEXT = VRT + "buttons/DisclaimerText.png";
		
		
		String ShowSoldStatus = Window.Location.getParameter("ShowSold");
		OPTIONS_SHOW_SOLD_STATUS = 
			(ShowSoldStatus == null)? false : Boolean.valueOf(ShowSoldStatus);
		
		String UseFilter = Window.Location.getParameter("UseFilter");
		OPTIONS_USE_FILTER = 
			(UseFilter == null)? true : Boolean.valueOf(UseFilter);

		String NavigationControls = Window.Location.getParameter("NavigationControls");
		OPTIONS_NAVIGATION_CONTROLS = 
			(NavigationControls == null)? true : Boolean.valueOf(NavigationControls);
	};

}
