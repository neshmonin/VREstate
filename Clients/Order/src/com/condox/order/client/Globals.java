package com.condox.order.client;

public class Globals {
	private static boolean testServer = true;
	private static String urlBase = testServer ? "https://vrt.3dcondox.com/vre/"
			: "https://vrt.3dcondox.com/";
	public static String getUserLogin(String uid, String pwd, String role) {
		return urlBase + "program?q=login&uid=" + uid + "&pwd=" + pwd
				+ "&role=" + role;
	}
}
