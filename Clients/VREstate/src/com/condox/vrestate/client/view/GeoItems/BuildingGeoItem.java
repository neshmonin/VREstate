package com.condox.vrestate.client.view.GeoItems;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.Position;
import com.condox.vrestate.client.document.Building;
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

		// ������ ��� ����������
		KmlStyle style = GE.getPlugin().createStyle("");
		String href = Options.HOME_URL + "gen/txt?height=40&shadow=2&text="
				+ building.getName() + "&txtClr=16777215&shdClr=0&frame=0";
		KmlIcon icon = GE.getPlugin().createIcon("");
		icon.setHref(href);
		style.getIconStyle().setIcon(icon);
		placemark.setStyleSelector(style);

		KmlPoint point = GE.getPlugin().createPoint("");
		point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		point.setExtrude(true);
		Position position = building.getPosition();
		// TODO - �������� � position
		if (position != null) {
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
	
	public void onSelectionChanged(boolean selected)
	{
		String old_href = placemark.getComputedStyle().getIconStyle().getIcon().getHref();
		String new_href = old_href;

		if (selected)
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
		return building.getPosition();
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
}