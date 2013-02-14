package com.condox.orders.client;

import com.google.gwt.core.client.GWT;
import com.google.gwt.user.client.Window;

public class Config {
	private static boolean DEBUG_MODE = 
		(Window.Location.getParameter("test") != null)?
				Boolean.valueOf(Window.Location.getParameter("test")):
					GWT.getModuleBaseURL().contains("/vre/");
	public static String URL_BASE = DEBUG_MODE? 
			"https://vrt.3dcondox.com/vre/":
				"https://vrt.3dcondox.com/";

}
