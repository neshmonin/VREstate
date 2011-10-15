package my.vrestate.client;

import com.google.gwt.core.client.GWT;

import my.vrestate.client.core.LookAt;

public class Options {
	
//	Это адрес сервера, с которого мы будем запрашивать картинки для кнопочек, файл
//	модели и т.д.
//	В моем случае VREServer установлен в C:\Program Files\VREServer\
//	Соответственно, искомые файлы должны лежать в C:\Program Files\VREServer\WebRoot
	public static String SERVER_URL = "http://localhost:8026/vre/";
//	public static String SERVER_URL = GWT.getHostPageBaseURL() + "vre";
	
	public static String SessionID = "";
//	public static GEWrapper Wrapper;
	
	static double K = 360/(2*Math.PI*6371032);	 
	// Distance - вспомогательная функция для вычисления расстояния между
	// двумя точками в пространстве, заданных в координатах LatLonAlt
//	public static double Distance(LatLonAlt A, LatLonAlt B) {
//		double dLat = (A.getLat() - B.getLat()) / K;
//		double dLon = (A.getLon() - B.getLon()) * Math.cos(A.getLat() * Math.PI / 180) /K;
//		double dAlt = A.getAlt() - B.getAlt();
//		double Result = Math.sqrt(dLat*dLat + dLon*dLon + dAlt*dAlt);
//		return Result;
//	}
	public static LookAt CurrentLookAt = null;
//	public static Camera CurrentCamera = null;
	public static final double MAX_SUITE_PLACEMARK_VISIBLE = 600; 
	public static final double MIN_SUITE_PLACEMARK_VISIBLE = 50; 
	public static final double MAX_SITE_DISTANCE = 1000;
	public static final double MIN_SITE_DISTANCE = 70;
	public static final String BUTTONS_URL = SERVER_URL + "Buttons/";
	
	
	
	public static boolean SUITES_SHOW_AVAILABLE = true;
	public static boolean SUITES_SHOW_SOLVED = false;
	public static boolean SUITES_SHOW_ONHOLD = false;
	
}
