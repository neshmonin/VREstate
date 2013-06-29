
package com.condox.vrestate.client.interactor;


import com.condox.clientshared.communication.Options;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.I_SB_View;
import com.condox.vrestate.client.view._AbstractView;
import com.google.gwt.event.shared.HandlerRegistration;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.event.MouseListener;

public class SB_Kiosk_Interactor extends OverlayHelpers
						   implements MouseListener,
						   			  I_AbstractInteractor{

	private HandlerRegistration mouse_listener = null;
	KmlScreenOverlay overlayZoomUnzoom = null;
	private OvlRectangle rectCentralBar = null;
	private OvlRectangle rectZoomUnzoom = null;

	private I_SB_View view = null;

	// Constructor
	public SB_Kiosk_Interactor(I_SB_View view) {
		this.view = view;
	    int WinH = GE.getEarthHeight();

	    // ZoomUnzoomBar.png - dimentions 148x964
	    int buttonHeightPixels = WinH * 80 / 100;
	    int buttonWidthPixels = buttonHeightPixels * 148 / 964;

	    rectZoomUnzoom = new OvlRectangle(
				new OvlPoint(new OvlDimension(buttonWidthPixels*3/4), 
							 new OvlDimension(0.45f)),
				new OvlDimension(buttonWidthPixels),
				new OvlDimension(buttonHeightPixels)
			);
	    rectCentralBar = new OvlRectangle(
				new OvlPoint(new OvlDimension(0.5f), 
							 new OvlDimension(0.5f)),
				new OvlDimension(0.15f),
				new OvlDimension(1f)
			);
	}
	
	private enum WhereIsMouse
	{
	   OnZoomUnzoomBar,
	   OnCentralBar,
	   AnywhereElse
	}
	
	private WhereIsMouse HitTest(int x, int y)
	{
		if (rectZoomUnzoom.ContainsPixel(x, y))
			return WhereIsMouse.OnZoomUnzoomBar;
		else if (rectCentralBar.ContainsPixel(x, y))
			return WhereIsMouse.OnCentralBar;
		
		return WhereIsMouse.AnywhereElse;
	}

	@Override
	public void setEnabled(boolean enabling) {
		//Log.write("SB_Interactor: setEnabled = " + enabling);
		if (enabling) {
			if (mouse_listener == null)
				mouse_listener = GE.getPlugin().getWindow()
						.addMouseListener(this);
		} else {
			mouse_listener.removeHandler();
			mouse_listener = null;
		}

		if (enabling) {
			if (overlayZoomUnzoom == null) {
				KmlIcon icon = GE.getPlugin().createIcon("");
				String href = Options.ZOOM_UNZOOM_URL;
				icon.setHref(href);
				overlayZoomUnzoom = GE.getPlugin().createScreenOverlay("");
				overlayZoomUnzoom.setIcon(icon);
	
				// Set the ScreenOverlay's position and size
				rectZoomUnzoom.InitScreenOverlay(overlayZoomUnzoom);
	
				GE.getPlugin().getFeatures().appendChild(overlayZoomUnzoom);
			}
			overlayZoomUnzoom.setVisibility(enabling);
	
		}
		else // disabling 
		{
			if (overlayZoomUnzoom != null) {
				overlayZoomUnzoom.setVisibility(enabling);
				GE.getPlugin().getFeatures().removeChild(overlayZoomUnzoom);
				overlayZoomUnzoom = null;
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
	
	boolean cameraPositionChanged = false;

	@Override
	public void onClick(KmlMouseEvent event) {
		event.preventDefault();

		if (cameraPositionChanged)
			return;
		
		if (event.getTarget().getType().equals("KmlPlacemark")) {
			KmlPlacemark placemark = (KmlPlacemark) event.getTarget();
			String json = placemark.getSnippet();
			JSONObject obj = JSONParser.parseLenient(json).isObject();

			String type = obj.get("type").isString().stringValue();
			int id = (int) obj.get("id").isNumber().doubleValue();
			this.view.Select(type, id);
		}
		else
		{   // they click Navigation Controls
	        GE.getPlugin().getOptions().setFlyToSpeed(this.view.getTransitionSpeed());
	        this.view.ApplyCamera();
	        GE.getPlugin().getOptions().setFlyToSpeed(this.view.getRegularSpeed());
		}
	}

	@Override
	public void onDoubleClick(KmlMouseEvent event) {
	}

	@Override
	public void onMouseDown(KmlMouseEvent event) {
		_AbstractView.ResetTimeOut();
		event.preventDefault();
		x = event.getClientX();
		y = event.getClientY();

		switch (HitTest(x, y))
		{
		case OnZoomUnzoomBar:
			break;
		case OnCentralBar:
			break;
		case AnywhereElse:
			break;
		}

	}

	@Override
	public void onMouseUp(KmlMouseEvent event) {
		_AbstractView.ResetTimeOut();
		event.preventDefault();
		if (cameraPositionChanged)
		{
			cameraPositionChanged = false;
			view.onHeadingChanged();
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
		double dZ;
		switch (HitTest(newX, newY))
		{
		case OnZoomUnzoomBar:
			if (event.getButton() == 0) {
				dZ = newY - y;
				ChangeRange(-dZ);
				y += dZ;
			}
			break;
		case OnCentralBar:
			if (view.getGeoItem().getType() == "building" &&
				Options.SUPPORT_PAN) {
				if (event.getButton() == 0) {
					dZ = newY - y;
					this.view.Pan(dZ);
					y += dZ;
				}
				break;
			}
		case AnywhereElse:
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
				dZ = newY - y;
				ChangeRange(-dZ);
				y += dZ;
				break;
			}
			break;
		}
	}
}
