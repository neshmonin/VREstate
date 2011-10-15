package my.vrestate.client.core.GEPlugin;

public class MouseEventData {
	public enum ButtonType {MB_LEFT, MB_MIDDLE, MB_RIGHT};
	public enum MouseEventType {ME_DOWN, ME_MOVE, ME_UP, ME_CLICK};
	
	public ButtonType Button = ButtonType.MB_LEFT;
	public MouseEventType Type = MouseEventType.ME_DOWN;
	
	public int X = 0;
	public int Y = 0;
	public String Tag = "";
}
