package com.condox.vrestate.client.interactor;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.SuiteView;
import com.condox.vrestate.client.view._AbstractView;
import com.condox.vrestate.client.view.GeoItems.FilteredOutNotification;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.event.shared.HandlerRegistration;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.event.MouseListener;

public class SuiteInteractor extends OverlayHelpers
							 implements MouseListener,
							 			I_AbstractInteractor,
							 			FilteredOutNotification{

	private HandlerRegistration mouse_listener = null;
	KmlScreenOverlay overlayNextSuite = null;
	KmlScreenOverlay overlayPrevSuite = null;
	private OvlRectangle rectNextSuite = null;
	private OvlRectangle rectPrevSuite = null;

	private SuiteView view = null;

	public SuiteInteractor(SuiteView view) {
		this.view = view;

		if (Filter.get().howManyFilteredIn() > 1)
		{
		    rectNextSuite = new OvlRectangle(
					new OvlPoint(new OvlDimension(0.94f), 
								 new OvlDimension(0.12f)),
					new OvlDimension(0.07f),
					new OvlDimension(0.1f)
				);
		    rectPrevSuite = new OvlRectangle(
					new OvlPoint(new OvlDimension(0.06f), 
								 new OvlDimension(0.12f)),
					new OvlDimension(0.07f),
					new OvlDimension(0.1f)
				);
		}
	}

	private enum WhereIsMouse
	{
	   OnNext,
	   OnPrev,
	   AnywhereElse
	}
	
	private WhereIsMouse HitTest(int x, int y)
	{
//		Window.alert("X:" + x + ", Y:" + y);
		if (rectNextSuite != null && rectNextSuite.ContainsPixel(x, y))
			return WhereIsMouse.OnNext;
		else
		if (rectPrevSuite != null && rectPrevSuite.ContainsPixel(x, y))
			return WhereIsMouse.OnPrev;
		
		return WhereIsMouse.AnywhereElse;
	}

	private void enableNextPrevButtons(boolean enabling){
		if (enabling) {
			if (overlayNextSuite == null) {
				KmlIcon icon = GE.getPlugin().createIcon("");
				String href = Options.URL_BUTTONS + "_rightDown.png";
				icon.setHref(href);
				overlayNextSuite = GE.getPlugin().createScreenOverlay("");
				overlayNextSuite.setIcon(icon);
	
				// Set the ScreenOverlay's position and size
				rectNextSuite.InitScreenOverlay(overlayNextSuite);
	
				GE.getPlugin().getFeatures().appendChild(overlayNextSuite);
			}
			overlayNextSuite.setVisibility(enabling);
	
			if (overlayPrevSuite == null) {
				KmlIcon icon = GE.getPlugin().createIcon("");
				String href = Options.URL_BUTTONS + "_leftDown.png";
				icon.setHref(href);
				overlayPrevSuite = GE.getPlugin().createScreenOverlay("");
				overlayPrevSuite.setIcon(icon);
	
				// Set the ScreenOverlay's position and size
				rectPrevSuite.InitScreenOverlay(overlayPrevSuite);
	
				GE.getPlugin().getFeatures().appendChild(overlayPrevSuite);
			}
			overlayPrevSuite.setVisibility(enabling);
		}
		else // disabling 
		{
			if (overlayNextSuite != null) {
				overlayNextSuite.setVisibility(enabling);
				GE.getPlugin().getFeatures().removeChild(overlayNextSuite);
				overlayNextSuite = null;
			}

			if (overlayPrevSuite != null) {
				overlayPrevSuite.setVisibility(enabling);
				GE.getPlugin().getFeatures().removeChild(overlayPrevSuite);
				overlayPrevSuite = null;
			}
		}
	}	
	
	@Override
	public void setEnabled(boolean enabling) {
		//Log.write("SuiteInteractor: setEnabled = " + enabling);
		SuiteGeoItem suiteGeo = (SuiteGeoItem)view.getGeoItem();
		if (enabling) {
			if (mouse_listener == null)
				mouse_listener = GE.getPlugin().getWindow()
						.addMouseListener(this);
			suiteGeo.registerForFilteredOutNotification(this);
		} else {
			mouse_listener.removeHandler();
			mouse_listener = null;
			suiteGeo.unregisterForFilteredOutNotification();
		}
	
		if (enabling && Filter.get().howManyFilteredIn() > 1)
			enableNextPrevButtons(true);
		else
			enableNextPrevButtons(false);
	}

	/*========================================*/
	@Override
	public void onClick(KmlMouseEvent event) {
		_AbstractView.ResetTimeOut();
		event.preventDefault();
//		==================================================
//		String msg = "";
//		int x = event.getClientX();
//		int y = event.getClientY();
//		msg += "mouseX:" + x + "\r\n";
//		msg += "mouseY:" + y + "\r\n";
//		
//		int WinW = GE.getEarthWidth();
//    	int WinH = GE.getEarthHeight();
//    	
//        int pixX = (rectPrevSuite.origin.x.Units == KmlUnits.UNITS_PIXELS) ? rectPrevSuite.origin.x.iVal : (int)(WinW * rectPrevSuite.origin.x.fVal);
//        int pixY = (rectPrevSuite.origin.y.Units == KmlUnits.UNITS_PIXELS) ? rectPrevSuite.origin.y.iVal : (int)(WinH * (1f-rectPrevSuite.origin.y.fVal));
//        
//        int pixWidth = (rectPrevSuite.width.Units == KmlUnits.UNITS_PIXELS) ? rectPrevSuite.width.iVal : (int)(WinW * rectPrevSuite.width.fVal);
//        int pixHeight = (rectPrevSuite.height.Units == KmlUnits.UNITS_PIXELS) ? rectPrevSuite.height.iVal : (int)(WinH * rectPrevSuite.height.fVal);
//
//        msg += "rectX:" + pixX + "\r\n";
//		msg += "rectY:" + pixY + "\r\n";
//		msg += "rectW:" + pixWidth + "\r\n";
//		msg += "rectH:" + pixHeight + "\r\n";
//		
//		float fval = rectPrevSuite.origin.y.fVal;
//		msg += "WinH:" + WinH + "\r\n";
//		msg += "fval:" + fval + "\r\n";
//		msg += "1f - fval:" + (1f - fval) + "\r\n";
//		msg += "WinH * (1f - fval):" + (WinH * (1f - fval)) + "\r\n";
//		msg += "(int)(WinH * (1f - fval)):" + ((int)(WinH * (1f - fval))) + "\r\n";
//		
//		Window.alert(msg);
//		==================================================
		
		switch (HitTest(event.getClientX(), event.getClientY()))
		{
		case OnNext:
//			Window.alert("Next");
			view.selectNextSuite();
			break;
		case OnPrev:
//			Window.alert("Prev");
			view.selectPrevSuite();
			break;
		case AnywhereElse:
//			Window.alert("Else");
			if (event.getTarget().getType().equals("KmlPlacemark")) {
				KmlPlacemark placemark = (KmlPlacemark) event.getTarget();
				String json = placemark.getSnippet();
				JSONObject obj = JSONParser.parseLenient(json).isObject();
				String type = obj.get("type").isString().stringValue();
				int id = (int) obj.get("id").isNumber().doubleValue();
				view.Select(type, id);
			} else
			view.Select(null, 0);
			break;
		}
	}

	@Override public void onDoubleClick(KmlMouseEvent event) {}
	@Override public void onMouseDown(KmlMouseEvent event){event.preventDefault();}
	@Override public void onMouseUp(KmlMouseEvent event){event.preventDefault();}
	@Override public void onMouseOver(KmlMouseEvent event) {}
	@Override public void onMouseOut(KmlMouseEvent event) {}
	@Override public void onMouseMove(KmlMouseEvent event){event.preventDefault();}

	@Override
	public void onFilteredOut() {
		view.ThisSuiteIsFilteredOut();
	}
}
