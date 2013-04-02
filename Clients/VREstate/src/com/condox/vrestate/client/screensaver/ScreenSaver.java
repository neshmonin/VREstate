package com.condox.vrestate.client.screensaver;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.view._AbstractView;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.Window;

public class ScreenSaver extends Timer {


	private static ScreenSaver instance = new ScreenSaver();

	private ScreenSaver() {
		schedule(TimeoutInterval);
	}

	public static ScreenSaver get() {
		return instance;
	}

	private int TimeoutInterval =  1000;

	@Override
	public void run() {
		_AbstractView.PopToTheBottom();
		notifyTimeout();
	}

	public void reset() {
		cancel();
		if (Options.ROLE.equals(Options.ROLES.KIOSK))
			notifyReset();
		schedule(TimeoutInterval);
	}
	
	private native void notifyTimeout() /*-{
		if (typeof($wnd.parent.parent.postMessage) == 'function')
			$wnd.parent.parent.postMessage('timeout', '*');
	}-*/;
	
	private native void notifyReset() /*-{
		if (typeof($wnd.parent.parent.postMessage) == 'function')
			$wnd.parent.parent.postMessage('reset', '*');
	}-*/;

}
