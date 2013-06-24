package com.condox.clientshared.abstractview;

import com.google.gwt.core.client.GWT;

public class Log {
	public static void write(String message) {
		GWT.log(message);
//		Window.alert(message);
//		VREstate.lbLog.addItem(message);
	};

}
