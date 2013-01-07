package com.condox.vrestate.client;

import com.google.gwt.core.client.GWT;
import com.google.gwt.user.client.Window;

public class Log {
	public static void write(String message) {
		GWT.log(message);
//		Window.alert(message);
//		VREstate.lbLog.addItem(message);
	};

}
