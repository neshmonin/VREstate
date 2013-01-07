package com.condox.vrestate.client.view;

import java.util.Stack;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.ge.GE;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;

public abstract class View implements IView {

	private static Stack<IView> views = new Stack<IView>();
	
	public View() {
		Push(this);
	}

	private static void Push(IView view) {
		if (!views.isEmpty())
			views.peek().setEnabled(false);
		views.push(view);
//		views.peek().setEnabled(true);
		for (IView item : views)
			Log.write(item.getClass().getName());
	}

	public void Pop() {
		views.peek().setEnabled(false);
		if (views.size() > 1)
			views.pop();
		views.peek().setEnabled(true);
		for (IView item : views)
			Log.write(item.getClass().getName());
	}

	KmlScreenOverlay info_overlay = null;
	KmlIcon icon = null;
	
	public void setEnabled(boolean value) {
		if (info_overlay == null) {
			icon = GE.getPlugin().createIcon("");
			info_overlay = GE.getPlugin().createScreenOverlay("");
			info_overlay.setIcon(icon);
			info_overlay.getOverlayXY().set(0.5, KmlUnits.UNITS_FRACTION, 30,
					KmlUnits.UNITS_INSET_PIXELS);
			info_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
					KmlUnits.UNITS_FRACTION);
			GE.getPlugin().getFeatures().appendChild(info_overlay);
		}
		String href = Options.HOME_URL + "gen/txt?height=40&shadow=2&text="
				+ getCaption() + "&txtClr=16777215&shdClr=0&frame=0";
		icon.setHref(href);
		info_overlay.setVisibility(value);
//		Update();
	};
	public abstract boolean isSelected(Object item);

	public static void ApplyFilter() {
		if (!views.isEmpty())
			((View)views.peek()).Draw();
	}

	public abstract String getCaption();

	public abstract void Update();

	public abstract void Select(String type, int id);
//	public abstract void Move(Position position);
	
	public double heading = 0;
	public double tilt = 45;
	public double range = 0;
	
}
