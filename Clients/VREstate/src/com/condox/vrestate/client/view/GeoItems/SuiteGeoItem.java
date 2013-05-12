package com.condox.vrestate.client.view.GeoItems;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.Position;
import com.condox.vrestate.client.document.Building;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view._AbstractView;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.json.client.JSONValue;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlLineString;
import com.nitrous.gwt.earth.client.api.KmlMultiGeometry;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlPoint;
import com.nitrous.gwt.earth.client.api.KmlStyle;

public class SuiteGeoItem implements IGeoItem {
	private final double initialRange_m = 30;
	private final double initialTilt_d = 75;

	public Suite suite = null;
	
	String href = null;
	public SuiteGeoItem(Suite suite){
		Init(suite);
	}
	public void Init(Suite suite){

		this.suite = suite;
		
		float scale = 1.0F;
		KmlStyle style = GE.getPlugin().createStyle("");
		switch (suite.getStatus()) {
		case Available:
			href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=65280&shdClr=65280&frame=0";
			style.getLineStyle().getColor().set("FF00FF00"); // GREEN
			style.getLineStyle().setWidth(2.0F);
			break;
		case Sold:
			if (Options.getShowSold()) {
				href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
				+ suite.getName()
				+ "&txtClr=16711680&shdClr=0&frame=0";
				style.getLineStyle().getColor().set("FF0000FF"); // RED
				style.getLineStyle().setWidth(2.0F);
			}
			else
			{
				href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text=.&txtClr=0&shdClr=0&frame=0";
				style.getLineStyle().getColor().set("00000000"); // transparent
				style.getLineStyle().setWidth(2.0F);
			}
			break;
		case ResaleAvailable:
			href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=1048575&shdClr=0&frame=0";
			style.getLineStyle().getColor().set("FFFFFF00"); // BLUE ??
			style.getLineStyle().setWidth(2.0F);
			break;
		case AvailableRent:
			href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=14854399&shdClr=0&frame=0";
			style.getLineStyle().getColor().set("FFE2A8FF"); // LIGHT-VIOLET
			style.getLineStyle().setWidth(2.0F);
			break;
		case Selected:
			href = Options.HOME_URL + "gen/txt?height=30&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=16764108&shdClr=0&frame=0";
			style.getLineStyle().getColor().set("FFCCCCFF"); // LIGHT-RED
			style.getLineStyle().setWidth(4.0F);
			scale = 1.0F;
			break;
		case Layout:
			href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=16777215&shdClr=0&frame=0";
			style.getLineStyle().getColor().set("FFFFFFFF"); // WHITE
			style.getLineStyle().setWidth(3.0F);
			break;
		default:
			break;
		}
	
		if (href == null) return;

		KmlIcon icon = GE.getPlugin().createIcon("");
	
		icon.setHref(href);
		style.getIconStyle().setIcon(icon);
		style.getIconStyle().setScale(scale);

		if (extended_data_label == null)
		{
			extended_data_label = GE.getPlugin().createPlacemark("");
			// Snippet
			JSONObject obj = new JSONObject();
			JSONValue type = new JSONString("suite");
			JSONValue id = new JSONNumber(suite.getId());
			obj.put("type", type);
			obj.put("id", id);
			extended_data_label.setSnippet(obj.toString());
	
			extended_data_label.setVisibility(false);
		
			KmlMultiGeometry geometry1 = GE.getPlugin().createMultiGeometry("");
			
			KmlPoint point = GE.getPlugin().createPoint("");
	
			Position position = suite.getPosition();
			position.setTilt(initialTilt_d);
			position.setRange(initialRange_m);
			
			point.setLatitude(position.getLatitude());
			point.setLongitude(position.getLongitude());
			
			Building parent = suite.getParent();
			if ((parent != null) && (parent.hasAltitudeAdjustment())) {
				point.setAltitude(position.getAltitude()
						+ parent.getAltitudeAdjustment());
				point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_ABSOLUTE);
			} else {
				point.setAltitude(position.getAltitude());
				point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
			}
			
			geometry1.getGeometries().appendChild(point);
			extended_data_label.setGeometry(geometry1);
			GE.getPlugin().getFeatures().appendChild(extended_data_label);
		
			extended_data_lines = GE.getPlugin().createPlacemark("");
			
			KmlMultiGeometry geometry2 = GE.getPlugin().createMultiGeometry("");
			for (int j = 0; j < suite.getPoints().size(); j += 6) {
				KmlLineString line_string = GE.getPlugin().createLineString("");
				line_string
						.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
				if ((parent != null) && (parent.hasAltitudeAdjustment()))
					line_string
							.setAltitudeMode(KmlAltitudeMode.ALTITUDE_ABSOLUTE);
		
				line_string.getCoordinates().pushLatLngAlt(suite.getPoints().get(j + 0),
						suite.getPoints().get(j + 1), suite.getPoints().get(j + 2));
				line_string.getCoordinates().pushLatLngAlt(suite.getPoints().get(j + 3),
						suite.getPoints().get(j + 4), suite.getPoints().get(j + 5));
				geometry2.getGeometries().appendChild(line_string);
			}
			extended_data_lines.setGeometry(geometry2);
			extended_data_lines.setStyleSelector(style);
			extended_data_label.setStyleSelector(style);
			GE.getPlugin().getFeatures().appendChild(extended_data_lines);
		}
		else
		{
			extended_data_lines.setStyleSelector(style);
			extended_data_label.setStyleSelector(style);
		}
			

	}
	
	public void onHeadingChanged(double heading_d) {
		if (href == null) return;

		boolean visible = isFilteredIn && isVisible(heading_d);
		if (extended_data_label.getVisibility() != visible)
			extended_data_label.setVisibility(visible);
	}
	
	private boolean isFilteredIn = true;
	public void Redraw() {
		if (href == null) return;

		isFilteredIn = Filter.get().isFilteredIn(suite);
		if (!isFilteredIn && filteredOutNotificationHandler != null)
			filteredOutNotificationHandler.onFilteredOut();

		if (extended_data_lines.getVisibility() != isFilteredIn)
			extended_data_lines.setVisibility(isFilteredIn);
		if (extended_data_label.getVisibility() != isFilteredIn)
			extended_data_label.setVisibility(isFilteredIn);
	}
	
	private FilteredOutNotification filteredOutNotificationHandler = null;
	public void registerForFilteredOutNotification(FilteredOutNotification handler) {
		filteredOutNotificationHandler = handler;
	}
	
	public void unregisterForFilteredOutNotification() {
		filteredOutNotificationHandler = null;
	}
	
	private boolean isVisible(double heading_d) {
		if (href == null) return false;

		heading_d += 180;
		if (heading_d > 360)
			heading_d -= 360;
		double diff = Math.abs(heading_d - suite.getPosition().getHeading());
		return (diff < 50) || (diff > 310);
	}

	
	/*-------------------- IGeoItem -----------------------*/
	@Override
	public Position getPosition() {
		return suite.getPosition();
	}

	@Override
	public String getName() {
		return suite.getName();
	}

	@Override
	public int getParent_id() {
		return suite.getParent_id();
	}

	@Override
	public int getId() {
		return suite.getId();
	}

	public String getCaption() {
		BuildingGeoItem buildingGeo = _AbstractView.getBuildingGeoItem(getParent_id());
		return suite.getName() + " - " + buildingGeo.getCaption();
	}
	/*-------------------- IGeoItem -----------------------*/

	private KmlPlacemark extended_data_label = null;

	public KmlPlacemark getExtendedDataLabel() {
		return extended_data_label;
	}

	private KmlPlacemark extended_data_lines = null;
	
	public KmlPlacemark getExtendedDataLines() {
		return extended_data_lines;
	}
	
	public String getFloor_name() {
		return suite.getFloor_name();
	}
	
	public int getCellingHeight() {
		return suite.getCeiling_height_ft();
	}
	
	public int getPrice() {
		return suite.getPrice();
	}

	@Override
	public String getType() {
		return "suite";
	}

	@Override
	public String getInitialViewKml() {
		// TODO Auto-generated method stub
		return null;
	}
}
