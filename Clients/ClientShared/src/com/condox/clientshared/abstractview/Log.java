package com.condox.clientshared.abstractview;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.google.gwt.core.client.GWT;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.ScrollPanel;

public class Log {
	private static List<String> logs = new ArrayList<String>();
	private static int maxSize = 100;
	
	public static void write(String message) {
		GWT.log(new Date().toString() + " >> " + message);
		logs.add("<br><b>" + new Date().toString() + " >> </b>" + message);
		if (logs.size() > maxSize)
			logs.remove(logs.get(0));
	};
	
	public static void popup() {
		String result = "";
		for (String log : logs)
			result += log;
		
		DialogBox popup = new DialogBox();
		ScrollPanel scroll = new ScrollPanel();
		scroll.setHeight("400px");
		scroll.setWidth("600px");
		HTML html = new HTML();
		html.setWordWrap(true);
//		html.setSize("400px", "300px");
		html.setWidth("100%");
		html.setHTML(result);
		scroll.add(html);
		popup.setWidget(scroll);
		popup.setAutoHideEnabled(true);
		popup.setModal(true);
		popup.setGlassEnabled(true);
		popup.center();
	}

}
