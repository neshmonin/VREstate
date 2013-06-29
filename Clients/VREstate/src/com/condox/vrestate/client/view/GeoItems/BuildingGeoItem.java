package com.condox.vrestate.client.view.GeoItems;


import com.condox.clientshared.abstractview.IGeoItem;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.document.Building;
import com.condox.clientshared.document.Position;
import com.condox.vrestate.client.ge.GE;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.json.client.JSONValue;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlMultiGeometry;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlPoint;
import com.nitrous.gwt.earth.client.api.KmlStyle;

public class BuildingGeoItem implements IGeoItem {
	private final double initialRange_m = 120;
	private final double initialTilt_d = 45;

	private Building building = null;
	private KmlPlacemark placemark = null;
	
	public BuildingGeoItem(Building building) {
		this.building = building;
		
		placemark = (KmlPlacemark) building.getExtendedData();
		placemark = GE.getPlugin().createPlacemark("");
		placemark.setVisibility(true);
		placemark.setName("");
		// Snippet
		JSONObject obj = new JSONObject();
		JSONValue type = new JSONString("building");
		JSONValue id = new JSONNumber(building.getId());
		obj.put("type", type);
		obj.put("id", id);
		placemark.setSnippet(obj.toString());

		// Иконка для плейсмарка
		KmlStyle style = GE.getPlugin().createStyle("");
		String href = Options.HOME_URL + "gen/txt?height=40&shadow=2&text="
				+ building.getName() + "&txtClr=16777215&shdClr=0&frame=0";
		KmlIcon icon = GE.getPlugin().createIcon("");
		icon.setHref(href);
//		style.getIconStyle().setScale(2.0F);
		style.getIconStyle().setIcon(icon);
		placemark.setStyleSelector(style);

		KmlPoint point = GE.getPlugin().createPoint("");
		point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		point.setExtrude(true);
		Position position = building.getCenter();
		if (position != null) {
			position.setTilt(initialTilt_d);
			position.setRange(initialRange_m);
			KmlMultiGeometry geometry = GE.getPlugin().createMultiGeometry(
					"");

			point.setLatLngAlt(position.getLatitude(),
					position.getLongitude(),
					building.getMax_suite_altitude() + 20);
			geometry.getGeometries().appendChild(point);

			placemark.setGeometry(geometry);
			GE.getPlugin().getFeatures().appendChild(placemark);
		}
		GE.getPlugin().getFeatures().appendChild(placemark);

	}
	
	public void onSelectionChanged(boolean selecting)
	{
		String old_href = placemark.getComputedStyle().getIconStyle().getIcon().getHref();
		String new_href = old_href;

		if (selecting)
			new_href = Options.HOME_URL
					+ "gen/txt?height=40&shadow=2&text="
					+ building.getName()
					+ "&txtClr=16777215&shdClr=0&frame=1";
		else
			new_href = Options.HOME_URL
					+ "gen/txt?height=40&shadow=2&text="
					+ building.getName()
					+ "&txtClr=16777215&shdClr=0&frame=0";
		
		if (!new_href.equals(old_href)) {
			KmlStyle style = GE.getPlugin().createStyle("");
			KmlIcon icon = GE.getPlugin().createIcon("");
			icon.setHref(new_href);

			style.getIconStyle().setIcon(icon);
			placemark.setStyleSelector(style);
		}
	}
	
	@Override
	public Position getPosition() {
		return building.getCenter();
	}

	@Override
	public String getName() {
		return building.getName();
	}

	@Override
	public int getParent_id() {
		return building.getParent_id();
	}

	@Override
	public int getId() {
		return building.getId();
	}

	@Override
	public String getCaption() {
		return building.getAddress();
	}

	@Override
	public String getType() {
		return "building";
	}

	@Override
	public String getInitialViewKml() {
		return null;
	}
}
