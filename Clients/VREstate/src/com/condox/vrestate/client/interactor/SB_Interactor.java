package com.condox.vrestate.client.interactor;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.I_SB_View;
import com.google.gwt.event.logical.shared.ResizeEvent;
import com.google.gwt.event.logical.shared.ResizeHandler;
import com.google.gwt.event.shared.HandlerRegistration;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.Window;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.event.MouseClickListener;
import com.nitrous.gwt.earth.client.api.event.MouseListener;

public class SB_Interactor extends OverlayHelpers
						   implements MouseListener,
						   			  I_AbstractInteractor{

	private HandlerRegistration mouse_listener = null;
	KmlScreenOverlay zoom_overlay = null;
	KmlScreenOverlay unzoom_overlay = null;
	private OvlRectangle zoomRect = null;
	private OvlRectangle unzoomRect = null;
	
	private I_SB_View view = null;

	// Constructor
	public SB_Interactor(I_SB_View view) {
		this.view = view;

		int WinW = GE.getEarth().getOffsetWidth();
	    int WinH = GE.getEarth().getOffsetHeight();
	    int buttonDimention = Math.min(WinW, WinH) / 10;
		
		zoomRect = new OvlRectangle(
						new OvlPoint(new OvlDimension(0.07f), 
									 new OvlDimension(0.3f)),
//						new OvlPoint(new OvlDimension(100), 
//									 new OvlDimension(200)),
						new OvlDimension(buttonDimention),
						new OvlDimension(buttonDimention)
					);

		unzoomRect = new OvlRectangle(
						new OvlPoint(new OvlDimension(0.07f), 
									 new OvlDimension(0.15f)),
//						new OvlPoint(new OvlDimension(100), 
//									 new OvlDimension(100)),
						new OvlDimension(buttonDimention),
						new OvlDimension(buttonDimention)
					);
	}
	
	private enum WhereIsMouse
	{
	   OnZoomButton,
	   OnUnzoomButton,
	   AnywhereElse
	}
	
	private WhereIsMouse HitTest(int x, int y)
	{
		if (zoomRect.ContainsPixel(x, y))
			return WhereIsMouse.OnZoomButton;
		if (unzoomRect.ContainsPixel(x, y))
			return WhereIsMouse.OnUnzoomButton;
		
		return WhereIsMouse.AnywhereElse;
	}

	@Override
	public void setEnabled(boolean enabling) {
		Log.write("SB_Interactor: setEnabled = " + enabling);
		if (enabling) {
			if (mouse_listener == null)
				mouse_listener = GE.getPlugin().getWindow()
						.addMouseListener(this);
		} else {
			// Log.write("1");
			mouse_listener.removeHandler();
			mouse_listener = null;
			// Log.write("2");
		}

		if (enabling) {
			if (zoom_overlay == null) {
				KmlIcon icon = GE.getPlugin().createIcon("");
				String href = Options.ZOOM_IN_URL;
				icon.setHref(href);
				zoom_overlay = GE.getPlugin().createScreenOverlay("");
				zoom_overlay.setIcon(icon);
	
				// Set the ScreenOverlay's position and size
				zoomRect.InitScreenOverlay(zoom_overlay);
	
				GE.getPlugin().getFeatures().appendChild(zoom_overlay);
			}
			zoom_overlay.setVisibility(enabling);
	
			if (unzoom_overlay == null) {
				KmlIcon icon = GE.getPlugin().createIcon("");
				String href = Options.ZOOM_OUT_URL;
				icon.setHref(href);
				unzoom_overlay = GE.getPlugin().createScreenOverlay("");
				unzoom_overlay.setIcon(icon);
	
				// Set the ScreenOverlay's position and size
				unzoomRect.InitScreenOverlay(unzoom_overlay);
	
				GE.getPlugin().getFeatures().appendChild(unzoom_overlay);
			}
			unzoom_overlay.setVisibility(enabling);
			
//			Window.addResizeHandler(new ResizeHandler() {
//
//				@Override
//				public void onResize(ResizeEvent event) {
//					Log.write("Resize: " + event.getWidth() + "x" + event.getHeight());
//					zoomRect.InitScreenOverlay(zoom_overlay);
//					unzoomRect.InitScreenOverlay(unzoom_overlay);
//				}});
		}
		else // disabling 
		{
			if (zoom_overlay != null) {
				zoom_overlay.setVisibility(enabling);
				GE.getPlugin().getFeatures().removeChild(zoom_overlay);
			}

			if (unzoom_overlay != null) {
				unzoom_overlay.setVisibility(enabling);
				GE.getPlugin().getFeatures().removeChild(unzoom_overlay);
			}
		}
	}

	private void ChangeHeading(double dH) {
		this.view.Move(dH, 0, 0);
	}

	private void ChangeTilt(double dT) {
		this.view.Move(0, dT, 0);
	}

	private void ChangeRange(double dR) {
		this.view.Move(0, 0, dR);
	}

	/*==================================================*/
	private int x = 0;
	private int y = 0;

	boolean action = false;
	boolean cameraPositionChanged = false;
	boolean autoZoomUnzoom = false;
	Timer zoomer = new Timer() {

		@Override
		public void run() {
			ChangeRange(50);
		}
	};

	Timer unzoomer = new Timer() {

		@Override
		public void run() {
			ChangeRange(-50);
		}
	};

	@Override
	public void onClick(KmlMouseEvent event) {
		event.preventDefault();

		// Log.write("SiteInteractor::onClick()");

		if (cameraPositionChanged || autoZoomUnzoom)
			return;
		
		if (event.getTarget().getType().equals("KmlPlacemark")) {
			KmlPlacemark placemark = (KmlPlacemark) event.getTarget();
			String json = placemark.getSnippet();
			JSONObject obj = JSONParser.parseLenient(json).isObject();

			String type = obj.get("type").isString().stringValue();
			int id = (int) obj.get("id").isNumber().doubleValue();

			//Log.write("" + obj.toString());
			this.view.Select(type, id);
		}
		else
		{
	        GE.getPlugin().getOptions().setFlyToSpeed(this.view.getTransitionSpeed());
	        this.view.getCamera().Apply();
	        GE.getPlugin().getOptions().setFlyToSpeed(this.view.getRegularSpeed());
		}
	}

	@Override
	public void onDoubleClick(KmlMouseEvent event) {
	}

	@Override
	public void onMouseDown(KmlMouseEvent event) {
		event.preventDefault();
		x = event.getClientX();
		y = event.getClientY();
	    int WinW = GE.getEarth().getOffsetWidth();
	    int WinH = GE.getEarth().getOffsetHeight();
    	WinW = Integer.valueOf(GE.getEarth().getElement().getAttribute("width"));
		Log.write("WinW: " + WinW + ", WinH: " + WinH);
		Log.write("X: " + x + ", Y: " + y);

		switch (HitTest(x, y))
		{
		case OnZoomButton:
			zoomer.scheduleRepeating(100);
			autoZoomUnzoom = true;
			break;
		case OnUnzoomButton:
			unzoomer.scheduleRepeating(100);
			autoZoomUnzoom = true;
			break;
		case AnywhereElse:
			action = true;
			break;
		}

	}

	@Override
	public void onMouseUp(KmlMouseEvent event) {
		event.preventDefault();
		if (cameraPositionChanged)
		{
			cameraPositionChanged = false;
			view.onHeadingChanged();
		}

		action = false;
		
		if (autoZoomUnzoom)
		{
			autoZoomUnzoom = false;
			zoomer.cancel();
			unzoomer.cancel();
		}
	}

	@Override
	public void onMouseOver(KmlMouseEvent event) {
	}

	@Override
	public void onMouseOut(KmlMouseEvent event) {
	}

	@Override
	public void onMouseMove(KmlMouseEvent event) {
		event.preventDefault();
		int newX = event.getClientX();
		int newY = event.getClientY();
		
		if (HitTest(newX, newY) != WhereIsMouse.AnywhereElse)
			return;
		
		if (action) {
			switch (event.getButton()) {
			case 0: // LEFT
				double dX = newX - x;
				double dY = newY - y;
				cameraPositionChanged = true;
				ChangeHeading(dX);
				ChangeTilt(dY);
				x += dX;
				y += dY;
				break;
			case 2: // RIGHT
				double dZ = newY - y;
				ChangeRange(-dZ);
				y += dZ;
				break;
			}
		}
	}
}
