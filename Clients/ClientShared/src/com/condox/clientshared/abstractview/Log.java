package com.condox.clientshared.abstractview;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.google.gwt.core.client.GWT;

public class Log {
	private static List<String> logs = new ArrayList<String>();
	private static int maxSize = 100;
	
	public static void write(String message) {
		GWT.log(new Date().toString() + " >> " + message);
		logs.add("\r\n" + new Date().toString() + " >> " + message);
		if (logs.size() > maxSize)
			logs.remove(logs.get(0));
	};
	
	public static String getLogs() {
		String result = "";
		for (String log : logs)
			result += log;
		return result;
	}

}
