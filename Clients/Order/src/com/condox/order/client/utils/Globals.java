package com.condox.order.client.utils;

public class Globals {
	private static boolean TEST_SERVER = true;
	private static String USER_NAME;
	private static String SID;

	public static void initialize() {

	}

	public static String getBaseUrl() {
		return TEST_SERVER ? "https://vrt.3dcondox.com/vre/"
				: "https://vrt.3dcondox.com/";
	}

	public static String getLoginRequest(String role, String uid, String pwd) {
		String result = getBaseUrl();
		result += "program?q=login";
		result += "&role=" + role;
		result += "&uid=" + uid;
		result += "&pwd=" + pwd;
		return result;
		/*// TODO For now returning default value
		return getBaseUrl() + "program?q=login&role=visitor&uid=web&pwd=web";*/
	}

	public static void setUserName(String name) {
		USER_NAME = name;
	}

	public String getUserName() {
		return USER_NAME;
	}

	public static void setSID(String sid) {
		SID = sid;
	}

	public static String getSID() {
		return SID;
	}

	public static String getBuildingsListRequest() {
		String result = getBaseUrl();
		result += "data/building?scopeType=address&ad_mu=Toronto&ed=Resale";
		result += "&sid=" + getSID();
		return result;
	}
}
