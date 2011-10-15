package my.vrestate.client.core;

public class Constants {
	public enum EventTypes {
		GE_PLUGIN_READY,
		// GEPlugin инициализирован.
		TOUCH,			
		// Произошло касание окна GEPlugin (мышка, TouchScreen).
		// Класс: TouchEvent.java.
		// Источники: GEWrapper.
		// Получатели: VREstate.
		VIEW_CHANGED, 
		HV_CLOSE,
		HV_OPEN_BUILDING,
		LOGIN_OK, 
		SITE_RECEIVED,
		BUILDINGS_RECEIVED, 
		SUITE_TYPES_RECEIVED, 
		SUITES_RECEIVED, 
		UPDATE_HV, SHOW_SUITE_INFO, VIEW_BACK, VIEW_SUITE, VIEW_CHANGING, 
		MOUSE_DOWN, 
		MOUSE_MOVE, 
		MOUSE_UP,
		MOUSE_CLICK,
		FRAME_END, GO_PANORAMIC_VIEW, REPAINT, CLICK_BUTTON,
		BUTTON_MOUSE_MOVE,
		BUTTON_MOUSE_CLICK
	}
	public enum ButtonStates {
		BUTTON_NORMAL,
		BUTTON_ACTIVATED,
		BUTTON_PRESSED
	}
	public static final String HELPER_BUTTON_ID = "HELPER_BUTTON_ID";
}
