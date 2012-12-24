package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Filter;
import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Building;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteType;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.SuiteInteractor;
import com.google.gwt.dom.client.Element;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.Window;
import com.nitrous.gwt.earth.client.api.GEHtmlDivBalloon;
import com.nitrous.gwt.earth.client.api.GEVisibility;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlFeature;
import com.nitrous.gwt.earth.client.api.KmlLookAt;
import com.nitrous.gwt.earth.client.api.KmlVec2;

public class SuiteView extends GEView {

	private SuiteInteractor interactor = null;
	private Suite selection = null;
	private KmlLookAt look_at = GE.getPlugin().createLookAt("");
	private static boolean isMoreInfoVisible = false;

	public SuiteView(Suite suite) {
		super();
		selection = suite;
		interactor = new SuiteInteractor(this);
		setEnabled(true);
	}

	@Override
	public void setEnabled(boolean value) {
		Log.write("SuiteView::setActive()" + value);
		super.setEnabled(value);
		if (value) {
			look_at.setLatitude(selection.getPosition().getLatitude());
			look_at.setLongitude(selection.getPosition().getLongitude());
			look_at.setAltitude(selection.getPosition().getAltitude());
			look_at.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
			heading = selection.getPosition().getHeading() + 180;
			// Нормализация (0 - 360)
			while (heading < 360)
				heading += 360;
			while (heading > 360)
				heading -= 360;
			
			look_at.setHeading(heading);
			tilt = 45;
			look_at.setTilt(tilt);
			range = 35;
			look_at.setRange(range);
			Update(1);
//			ShowMoreInfo();
			GE.getPlugin().getNavigationControl().setVisibility(GEVisibility.VISIBILITY_HIDE);
		} else {
			GE.getPlugin().getNavigationControl().setVisibility(GEVisibility.VISIBILITY_AUTO);
			HideMoreInfo();
		}
		interactor.setEnabled(value);
		Filter.get().setVisible(!value);
	}

	@Override
	public boolean isSelected(Object item) {
		if (selection.equals(item))
			return true;
		for (Building building : Document.get().getBuildings())
			if (building.equals(item))
				return (selection.getParent_id() == building.getId());
		return false;
	}

	@Override
	public void Select(String type, int id) {
		Pop();
		// if (type.equals("building"))
		// if (selection.getId() == id)
		// Pop();
		// else
		// for (Building building : Document.get().getBuildings())
		// if (building.getId() == id) {
		// selection = building;
		// setActive(true);
		// // UpdateLookAt();
		// // Update();
		// }
	}

	// @Override
	// protected void onPlacemarkClick(String snippet) {
	// JSONObject obj = JSONParser.parseLenient(snippet).isObject();
	// String type = obj.get("type").isString().stringValue();
	// int id = Integer.valueOf((int) obj.get("id").isNumber().doubleValue());
	// if (type.equals("BUILDING")) {
	// Building building = Building.get(id);
	// if (Viewable.isSelected(building))
	// Site.get(building.getParent_id()).Select();
	// else
	// BuildingView.Create(id);
	// }
	// else if (type.equals("SUITE")) {
	// Suite suite = Suite.get(id);
	// SuiteView.Create(id);
	// // if (Viewable.isSelected(suite))
	// // Site.get(suite.getParent_id()).Select();
	// // else
	// // BuildingView.Create(id);
	// }
	//
	// super.onPlacemarkClick(snippet);
	// }

	public void ShowMoreInfo() {
		if (isMoreInfoVisible)
			return;
		isMoreInfoVisible = true;
		GEHtmlDivBalloon balloon = GE.getPlugin().createHtmlDivBalloon("");
		balloon.setCloseButtonEnabled(false);
		balloon.setContentDiv(Options.SUITE_INFO_TEMPLATE);
		KmlFeature feature = (KmlFeature) selection.getExtendedData();
		Log.write("KML: " + feature.getKml());
		balloon.setFeature(feature);
		GE.getPlugin().setBalloon(balloon);
		addElement(((Element) balloon.getContentDiv()), getJsonParams());
	};

	public void HideMoreInfo() {
		if (!isMoreInfoVisible)
			return;
		isMoreInfoVisible = false;
		GEHtmlDivBalloon balloon = (GEHtmlDivBalloon) GE.getPlugin()
				.getBalloon();
		removeElement(((Element) balloon.getContentDiv()));
		GE.getPlugin().setBalloon(null);
	}

	private String getJsonParams() {
		JSONObject obj = new JSONObject();
		if (selection.getPrice() > 0)
			obj.put("price", new JSONString("$" + selection.getPrice()));
		obj.put("suitName", new JSONString(selection.getName()));
		obj.put("Floor", new JSONString(selection.getFloor_name()));

		SuiteType type = selection.getSuiteType();
//		if (type.getBedrooms() >= 0)
			obj.put("bedrooms", new JSONNumber(type.getBedrooms()));
//		if (type.getBalconies() >= 0)
			obj.put("balcony", new JSONNumber(type.getBalconies()));
		 obj.put("ceiling", new JSONNumber(selection.getCeiling_height_ft()));

		 if (type.getFloorPlanUrl() != null)
			 obj.put("more", new JSONString(type.getFloorPlanUrl()));
		 obj.put("panoramicViewURL", new JSONString(""));
		// if (suite_type.area > 0)
		 obj.put("area", new JSONNumber(type.getArea()));
		// obj.put("photo", new JSONString("PhotoUrl"));
		// obj.put("more", new JSONString("MoreInfoUrl"));
		// obj.put("mail", new JSONString("MailUrl"));
		// obj.put("phone", new JSONString("123456789"));
		// Log.write("json:" + obj.toString());
		// Log.write("balconies: " + suite_type.balconies);
		// Log.write("suite_type: " + suite_type.name);
		return obj.toString();
	};

	public native void addElement(Element element, String json) /*-{
		//********************************
		$doc.getElementsByTagName('body')[0].appendChild(element);
		element.style.left = "10px";
		var scripts = element.getElementsByTagName("script");

		for (i = 0; i < scripts.length; i++) {
			// if src, eval it, otherwise eval the body
			if (scripts[i].hasAttribute("src")) {
				var src = scripts[i].getAttribute("src");
				var script = $doc.createElement('script');
				script.setAttribute("src", src);
				$doc.getElementsByTagName('body')[0].appendChild(script);
			} else {
				$wnd.eval(scripts[i].innerHTML);
			}
		}
		//********************************
		var suite = this;
		var show_panoramic_view = function() {
			suite.@com.condox.vrestate.client.view.SuiteView::ShowPanoramicView()();
		}
		var show_more = function() {
			suite.@com.condox.vrestate.client.view.SuiteView::ShowMore()();
			//		return false;
		}
		$wnd.project(element, json, show_panoramic_view, show_more);
	}-*/;

	public native void removeElement(Element element) /*-{
		element.parentNode.removeChild(element);
	}-*/;

	private void ShowPanoramicView() {
//		Window.alert("PanoramicView");
//		GE.getPlugin().setBalloon(null);
		for (Suite suite : Document.get().getSuites())
			if (suite.getId() == selection.getId())
		new PanoramicView(suite);
	}

	private void ShowMore() {
		SuiteType type = selection.getSuiteType();
		if (type.getFloorPlanUrl() != null)
			Window.open(type.getFloorPlanUrl(), "_blank", null);
		// Error
		// else
		// Window.open("google.com", "name", null);

		// Window.alert("FloorPlan");
	}

	@Override
	public String getCaption() {
		for (Building building : Document.get().getBuildings())
			if (building.getId() == selection.getParent_id())
				return selection.getName() + " - " + building.getAddress();
		return "";
	}

	public void Update(double speed) {
		double old_speed = GE.getPlugin().getOptions().getFlyToSpeed();
		double new_speed = speed;
		if (new_speed == 0)
			new_speed = GE.getPlugin().getFlyToSpeedTeleport();
		GE.getPlugin().getOptions().setFlyToSpeed(new_speed);
		GE.getView().setAbstractView(look_at);
		GE.getPlugin().getOptions().setFlyToSpeed(old_speed);
		Draw();
		
	}

	@Override
	public void Update() {
		KmlVec2 coords = GE.getView().project(selection.getPosition().getLatitude(),
				selection.getPosition().getLongitude(), selection.getPosition().getAltitude(),
				KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		if (coords == null)
			Update(1);
		else {
			double screen_center_X = GE.getEarth().getOffsetWidth() / 2;
			double screen_center_Y = GE.getEarth().getOffsetHeight() / 2;
			double dX = Math.abs(screen_center_X - coords.getX());
			double dY = Math.abs(screen_center_Y - coords.getY());
			if ((dX > 10) || (dY > 10))
				Update(0.1);
			else
				Update(0);
		}
		
	}

}
