package com.condox.vrestate.client.view;

import java.util.ArrayList;

import com.condox.vrestate.client.Filter;
import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.Position;
import com.condox.vrestate.client.document.Building;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteStatus;
import com.condox.vrestate.client.document.SuiteType;
import com.condox.vrestate.client.ge.GE;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.json.client.JSONValue;
import com.nitrous.gwt.earth.client.api.BatchFunction;
import com.nitrous.gwt.earth.client.api.GEPlugin;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlLineString;
import com.nitrous.gwt.earth.client.api.KmlLookAt;
import com.nitrous.gwt.earth.client.api.KmlMultiGeometry;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlPoint;
import com.nitrous.gwt.earth.client.api.KmlStyle;

public abstract class GEView extends View {

	public GEView() {
		super();
	}

	static KmlObject suites = null;

	@Override
	public void Draw() {
		// Log.write("GEView::Update");
		// GE.getPlugin().getOptions().setMouseNavigationEnabled(false);
		// GE.getPlugin().getNavigationControl().setVisibility(GEVisibility.VISIBILITY_HIDE);
		// int counter = Document.get().getSuites().size();
		GE.getPlugin().executeBatch(new BatchFunction() {
			@Override
			public void run(GEPlugin plugin) {
				int counter = Document.get().getSuites().size();
				for (Suite suite : Document.get().getSuites()) {
					// Log.write("Left " + (counter--) + " suites.");
					UpdateSuite(suite);
				}
			}
		});
		// int count = Document.get().getSuites().size();
		// Log.write("Left " + (count--) + " suites");
		// }
		// kml += "</Folder>";

		for (Building building : Document.get().getBuildings())
			UpdateBuilding(building);
		// for (Site site : Document.get().getSites())
		// UpdateSite(site);
		// GE.getPlugin().getOptions().setMouseNavigationEnabled(true);
		// GE.getPlugin().getNavigationControl()
		// .setVisibility(GEVisibility.VISIBILITY_SHOW);
		// Log.write("GEView::Updated");
	}

	protected void UpdateSuite(Suite suite) {
		// Log.write("Updating suite " + suite.getName());
		// Log.write("UpdateSuite");
		KmlPlacemark placemark = (KmlPlacemark) suite.getExtendedData();
		if (placemark == null) {
			// Log.write("Creating placemark for suite " + suite.getId());
			placemark = GE.getPlugin().createPlacemark("");
			// Snippet
			JSONObject obj = new JSONObject();
			JSONValue type = new JSONString("suite");
			JSONValue id = new JSONNumber(suite.getId());
			obj.put("type", type);
			obj.put("id", id);
			placemark.setSnippet(obj.toString());

			placemark.setVisibility(false);
			// placemark.setName("");
			// Иконка
			KmlStyle style = GE.getPlugin().createStyle("");
			String href = "";
			// Log.write("?");
			switch (suite.getStatus()) {
			case STATUS_AVAILABLE:
				href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
						+ suite.getName()
						+ "&txtClr=65280&shdClr=65280&frame=0";
				style.getLineStyle().getColor().set("FF00FF00"); // GREEN
				break;
			case STATUS_SOLD:
				href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
						+ suite.getName()
						+ "&txtClr=16711680&shdClr=16711680&frame=0";
				style.getLineStyle().getColor().set("FF0000FF"); // RED
				break;
			case STATUS_RESALE_AVAILABLE:
				href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
						+ suite.getName()
						+ "&txtClr=1048575&shdClr=1048575&frame=0";
				style.getLineStyle().getColor().set("FFFFFF00"); // BLUE ??
				break;
			}

			KmlIcon icon = GE.getPlugin().createIcon("");

			icon.setHref(href);
			style.getIconStyle().setIcon(icon);
			style.getIconStyle().setScale(1);
			style.getLineStyle().setWidth(2);
			placemark.setStyleSelector(style);

			KmlMultiGeometry geometry = GE.getPlugin().createMultiGeometry("");

			KmlPoint point = GE.getPlugin().createPoint("");
			Position position = suite.getPosition();
			// point.setLatLngAlt(position.getLatitude(),
			// position.getLongitude(),
			// position.getAltitude());

			point.setLatitude(position.getLatitude());
			point.setLongitude(position.getLongitude());
			// Ищем билдинг данной квартиры
			Building parent = null;
			for (Building item : Document.get().getBuildings())
				if (item.getId() == suite.getParent_id())
					parent = item;

			if ((parent != null) && (parent.hasAltitudeAdjustment())) {
				point.setAltitude(position.getAltitude()
						+ parent.getAltitudeAdjustment());
				point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_ABSOLUTE);
			} else {
				point.setAltitude(position.getAltitude());
				point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
			}

			geometry.getGeometries().appendChild(point);

			final double METERS_PER_DEGREES = 111111;
			int i = 0;
			ArrayList<Double> points = new ArrayList<Double>();
			// TODO Всю эту возню с типами квартир надо будет переделать. Но не
			// сейчас.
			SuiteType suite_type = suite.getSuiteType();
			// Log.write("SuiteType: " + suite_type);
			// Log.write("getPoints().size(): " +
			// suite_type.getPoints().size());
			while (i < suite_type.getPoints().size()) {
				// Log.write("i: " + i);
				// X
				double lat = suite.getPosition().getLatitude()
						+ (suite_type.getPoints().get(i) + 50 / 2.54)
						* 0.0254
						* Math.cos(Math.toRadians(suite.getPosition()
								.getHeading())) / METERS_PER_DEGREES;
				double lon = suite.getPosition().getLongitude()
						+ (suite_type.getPoints().get(i) + 50 / 2.54)
						* 0.0254
						* Math.sin(Math.toRadians(suite.getPosition()
								.getHeading()))
						/ METERS_PER_DEGREES
						/ Math.cos(Math.toRadians(suite.getPosition()
								.getLatitude()));
				i++;
				// Y
				lat += suite_type.getPoints().get(i)
						* 0.0254
						* Math.sin(Math.toRadians(suite.getPosition()
								.getHeading())) / METERS_PER_DEGREES;
				lon -= suite_type.getPoints().get(i)
						* 0.0254
						* Math.cos(Math.toRadians(suite.getPosition()
								.getHeading()))
						/ METERS_PER_DEGREES
						/ Math.cos(Math.toRadians(suite.getPosition()
								.getLatitude()));
				i++;
				// Z
				double alt = suite.getPosition().getAltitude()
						+ suite_type.getPoints().get(i) * 0.0254;
				if ((parent != null) && (parent.hasAltitudeAdjustment()))
					alt += parent.getAltitudeAdjustment();
				i++;
				points.add(lat);
				points.add(lon);
				points.add(alt);
			}
			for (int j = 0; j < points.size(); j += 6) {
				// Log.write("C");

				// // Коррекция АА
				// if (suite.building.getAltitudeAdjustment() != null)
				// line_string += "<altitudeMode>absolute</altitudeMode>";
				// else
				// =============
				KmlLineString line_string = GE.getPlugin().createLineString("");
				line_string
						.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
				if ((parent != null) && (parent.hasAltitudeAdjustment()))
					line_string
							.setAltitudeMode(KmlAltitudeMode.ALTITUDE_ABSOLUTE);

				line_string.getCoordinates().pushLatLngAlt(points.get(j + 0),
						points.get(j + 1), points.get(j + 2));
				line_string.getCoordinates().pushLatLngAlt(points.get(j + 3),
						points.get(j + 4), points.get(j + 5));
				geometry.getGeometries().appendChild(line_string);
			}

			placemark.setGeometry(geometry);

			GE.getPlugin().getFeatures().appendChild(placemark);
//			Log.write(placemark.getKml());
			suite.setExtendedData(placemark);
		}
		// {
		
		boolean visible = isSuiteVisible(suite);
		boolean suite_filtered = true;
		// А вот здесь должно бы быть вот так:
		// boolean suite_filtered = isSuiteFiltered(suite);
		

		float icon_scale = placemark.getComputedStyle().getIconStyle()
				.getScale();
		if (visible != (icon_scale > 0)) {
			icon_scale = (visible) ? 1 : 0;
			// TODO Тут можуть бути проблемы з getComputedStyle()
			// (модифікація)
			placemark.getComputedStyle().getIconStyle().setScale(icon_scale);
		}

		if (suite_filtered != placemark.getVisibility())
			placemark.setVisibility(suite_filtered);

	}

	// protected String getSuiteKml(Suite suite) {
	// // Log.write("getSuiteKml: " + suite.getName());
	// String kml = (String) suite.getExtendedData();
	// if (kml != null) {
	// // if(isSuiteFiltered(suite));
	// // TODO Уродливо, ненадёжно. Может, когда-нибудь переделаю
	// if (isSuiteVisible(suite))
	// kml = kml.replace("<visibility>false</visibility>",
	// "<visibility>true</visibility>");
	// else
	// kml = kml.replace("<visibility>true</visibility>",
	// "<visibility>false</visibility>");
	// } else {
	// kml = "<Placemark>";
	// // Snippet
	// JSONObject obj = new JSONObject();
	// JSONValue type = new JSONString("suite");
	// JSONValue id = new JSONNumber(suite.getId());
	// obj.put("type", type);
	// obj.put("id", id);
	// String snippet = obj.toString();
	// kml += "<Snippet>";
	// kml += snippet.replace('"', '\"');
	// kml += "</Snippet>";
	// kml += "<visibility>true</visibility>";
	// kml += "<name></name>";
	// kml += "<Style>";
	//
	// String href = "";
	// String line_color = "";
	// switch (suite.getStatus()) {
	// case STATUS_AVAILABLE:
	// href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
	// + suite.getName()
	// + "&txtClr=65280&shdClr=65280&frame=0";
	// line_color = "ff00ff00";
	// break;
	// case STATUS_SOLD:
	// href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
	// + suite.getName()
	// + "&txtClr=16711680&shdClr=16711680&frame=0";
	// line_color = "ff0000ff";
	// break;
	// case STATUS_ON_HOLD:
	// href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
	// + suite.getName()
	// + "&txtClr=16776960&shdClr=16776960&frame=0";
	// line_color = "ffff0000";
	// break;
	// }
	// href = href.replaceAll("&", "&amp;");
	//
	// kml += "<IconStyle>";
	// kml += "<Icon>";
	// kml += "<href>" + href + "</href>";
	// kml += "<scale>0</scale>";
	// kml += "</Icon>";
	// kml += "</IconStyle>";
	// kml += "<LineStyle>";
	// kml += "<color>" + line_color + "</color>";
	// kml += "<width>2</width>";
	// kml += "</LineStyle>";
	//
	// kml += "</Style>";
	// kml += "<MultiGeometry>";
	// kml += "	<Point>";
	// kml += "		<altitudeMode>relativeToGround</altitudeMode>";
	// kml += "		<coordinates>";
	// kml += suite.getPosition().getLongitude() + ",";
	// kml += suite.getPosition().getLatitude() + ",";
	// kml += suite.getPosition().getAltitude();
	// kml += "		</coordinates>";
	// kml += "	</Point>";
	//
	// // LINES
	// final double METERS_PER_DEGREES = 111111;
	// int i = 0;
	// ArrayList<Double> points = new ArrayList<Double>();
	// // TODO Всю эту возню с типами квартир надо будет переделать. Но не
	// // сейчас.
	// SuiteType suite_type = suite.getSuiteType();
	// while (i < suite_type.getPoints().size()) {
	// // X
	// double lat = suite.getPosition().getLatitude()
	// + (suite_type.getPoints().get(i) + 0.5)
	// * 0.0254
	// * Math.cos(Math.toRadians(suite.getPosition()
	// .getHeading())) / METERS_PER_DEGREES;
	// double lon = suite.getPosition().getLongitude()
	// + (suite_type.getPoints().get(i) + 0.5)
	// * 0.0254
	// * Math.sin(Math.toRadians(suite.getPosition()
	// .getHeading()))
	// / METERS_PER_DEGREES
	// / Math.cos(Math.toRadians(suite.getPosition()
	// .getLatitude()));
	// i++;
	// // Y
	// lat += suite_type.getPoints().get(i)
	// * 0.0254
	// * Math.sin(Math.toRadians(suite.getPosition()
	// .getHeading())) / METERS_PER_DEGREES;
	// lon -= suite_type.getPoints().get(i)
	// * 0.0254
	// * Math.cos(Math.toRadians(suite.getPosition()
	// .getHeading()))
	// / METERS_PER_DEGREES
	// / Math.cos(Math.toRadians(suite.getPosition()
	// .getLatitude()));
	// i++;
	// // Z
	// double alt = suite.getPosition().getAltitude()
	// + suite_type.getPoints().get(i) * 0.0254;
	// i++;
	// points.add(lat);
	// points.add(lon);
	// points.add(alt);
	// }
	// for (int j = 0; j < points.size(); j += 6) {
	// // // Коррекция АА
	// // if (suite.building.getAltitudeAdjustment() != null)
	// // line_string += "<altitudeMode>absolute</altitudeMode>";
	// // else
	// // =============
	// kml += "<LineString>";
	// kml += "	<altitudeMode>relativeToGround</altitudeMode>";
	// kml += "		<coordinates>";
	// kml += points.get(j + 1) + ",";
	// kml += points.get(j + 0) + ",";
	// kml += points.get(j + 2) + " ";
	// kml += points.get(j + 4) + ",";
	// kml += points.get(j + 3) + ",";
	// kml += points.get(j + 5) + " ";
	// kml += "		</coordinates>";
	// kml += "</LineString>";
	// }
	//
	// // END LINES
	//
	// // POLYGON
	// // final double METERS_PER_DEGREES = 111111;
	// kml += "<Polygon>";
	// kml += "	<altitudeMode>relativeToGround</altitudeMode>";
	// kml += "<outerBoundaryIs>";
	// kml += "<LinearRing>";
	// kml += "<coordinates>";
	// points = new ArrayList<Double>();
	// points.add(100.0);
	// points.add(100.0);
	// points.add(100.0);
	//
	// points.add(100.0);
	// points.add(-100.0);
	// points.add(100.0);
	//
	// points.add(100.0);
	// points.add(-100.0);
	// points.add(-100.0);
	//
	// points.add(100.0);
	// points.add(100.0);
	// points.add(-100.0);
	//
	// points.add(100.0);
	// points.add(100.0);
	// points.add(100.0);
	//
	// i = 0;
	// while (i < points.size()) {
	// // X
	// double lat = suite.getPosition().getLatitude()
	// + (points.get(i) + 0.5)
	// * 0.0254
	// * Math.cos(Math.toRadians(suite.getPosition()
	// .getHeading())) / METERS_PER_DEGREES;
	// double lon = suite.getPosition().getLongitude()
	// + (points.get(i) + 0.5)
	// * 0.0254
	// * Math.sin(Math.toRadians(suite.getPosition()
	// .getHeading()))
	// / METERS_PER_DEGREES
	// / Math.cos(Math.toRadians(suite.getPosition()
	// .getLatitude()));
	// i++;
	// // Y
	// lat += points.get(i)
	// * 0.0254
	// * Math.sin(Math.toRadians(suite.getPosition()
	// .getHeading())) / METERS_PER_DEGREES;
	// lon -= points.get(i)
	// * 0.0254
	// * Math.cos(Math.toRadians(suite.getPosition()
	// .getHeading()))
	// / METERS_PER_DEGREES
	// / Math.cos(Math.toRadians(suite.getPosition()
	// .getLatitude()));
	// i++;
	// // Z
	// double alt = suite.getPosition().getAltitude() + points.get(i)
	// * 0.0254;
	// i++;
	//
	// kml += lon + ",";
	// kml += lat + ",";
	// kml += alt + " ";
	// }
	// kml += "</coordinates>";
	// kml += "</LinearRing>";
	// kml += "</outerBoundaryIs>";
	// kml += "</Polygon>";
	// // END LINES
	// kml += "</MultiGeometry>";
	// kml += "</Placemark>";
	// suite.setExtendedData(kml);
	// }
	// Log.write(kml);
	// return kml;
	// }

	private boolean isSuiteFiltered(Suite suite) {
		// Log.write("Filtered:");
		boolean filtered = true;
		filtered &= (!suite.getStatus().equals(SuiteStatus.STATUS_SOLD));
		filtered &= (!suite.getStatus().equals(SuiteStatus.STATUS_ON_HOLD));
		filtered &= Filter.get().isPriceFiltered(suite.getPrice());
		SuiteType type = suite.getSuiteType();
		filtered &= Filter.get().isBedroomsFiltered(type.getBedrooms());
		filtered &= Filter.get().isBathroomsFiltered(type.getBathrooms());
		filtered &= Filter.get().isAreaFiltered(type.getArea());
		filtered &= Filter.get().isBalconyFiltered(type.getBalconies() > 0);
		// TODO Сопли, которые надо убрать!
		// Log.write("Filtered:" + filtered);
		Filter.filtered_suites += ((filtered) ? 1 : 0);
		Filter.all_suites++;
		// Log.write("filtered suites:" + Filter.filtered_suites);
		// Log.write("all suites:" + Filter.all_suites);
		return filtered;
	}

	private boolean isSuiteVisible(Suite suite) {
		double heading = getHeading();
		heading += 180;
		if (heading > 360)
			heading -= 360;
		double diff = Math.abs(heading - suite.getPosition().getHeading());
		return (diff < 50) || (diff > 310);
	}

	private double getHeading() {
		KmlLookAt look_at = GE.getView().copyAsLookAt(
				KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		if (look_at != null)
			return look_at.getHeading();
		return 0;
	};

	protected void UpdateBuilding(Building building) {
		// Log.write("Updating building " + building.getName());
		KmlPlacemark placemark = (KmlPlacemark) building.getExtendedData();
		if (placemark == null) {
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
			style.getIconStyle().setIcon(icon);
			placemark.setStyleSelector(style);

			KmlPoint point = GE.getPlugin().createPoint("");
			point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
			point.setExtrude(true);
			Position position = building.getPosition();
			// TODO - Проблемы с position
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
			building.setExtendedData(placemark);
		} else {

			String old_href = placemark.getComputedStyle().getIconStyle()
					.getIcon().getHref();
			String new_href = old_href;
			// TODO сопли
			if (isSelected(building))
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
				// style.getIconStyle().setScale(
				// (float) ((getSelected_building().equals(building)) ? 1.2 :
				// 1.0));
				style.getIconStyle().setIcon(icon);
				placemark.setStyleSelector(style);
			}
		}
	}

	// private void DrawSite(Site site) {
	// // Модель для сайта
	// site.setExtendedData(GE.getPlugin().createNetworkLink(""));
	// KmlNetworkLink networkLink = (KmlNetworkLink) site.getExtendedData();
	// KmlLink link = GE.getPlugin().createLink("");
	// link.setHref(Options.HOME_URL + site.getDisplayModelUrl());
	// networkLink.setLink(link);
	// GE.getPlugin().getFeatures().appendChild(networkLink);
	//
	// // // Создание плейсмарка для сайта
	// // site.setExtendedData(GE.getPlugin().createPlacemark(""));
	// // KmlPlacemark placemark = (KmlPlacemark) site.getExtendedData();
	// // placemark.setVisibility(false);
	// // placemark.setName("");
	// // // Snippet
	// // JSONObject obj = new JSONObject();
	// // JSONValue type = new JSONString(site.getType().toString());
	// // JSONValue id = new JSONNumber(site.getId());
	// // obj.put("type", type);
	// // obj.put("id", id);
	// // placemark.setSnippet(obj.toString());
	// //
	// // Position position = site.getPosition();
	// // // TODO - Проблемы с position
	// // if (position != null) {
	// // KmlMultiGeometry geometry = GE.getPlugin().createMultiGeometry("");
	// // // Point
	// // KmlPoint point = GE.getPlugin().createPoint("");
	// // point.setLatitude(position.getLatitude());
	// // point.setLongitude(position.getLongitude());
	// // point.setAltitude(position.getAltitude());
	// // point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
	// // // Модель для сайта
	// // KmlLocation loc = GE.getPlugin().createLocation("");
	// // loc.setLatitude(position.getLatitude());
	// // loc.setLongitude(position.getLongitude());
	// // loc.setAltitude(position.getAltitude());
	// // // loc.setAltitude(0);
	// //
	// // KmlLink link = GE.getPlugin().createLink("");
	// // link.setHref(Options.HOME_URL + site.getDisplayModelUrl());
	// // Log.write(link.getHref());
	// //
	// // KmlModel model = GE.getPlugin().createModel("");
	// // model.setLink(link);
	// // model.setLocation(loc);
	// //
	// //
	// // geometry.getGeometries().appendChild(point);
	// // geometry.getGeometries().appendChild(model);
	// //
	// // placemark.setGeometry(geometry);
	// // GE.getPlugin().getFeatures().appendChild(placemark);
	// // }
	// // UpdateSite(site);
	// }
	//
	// private void UpdateSite(Site site) {
	// // KmlPlacemark placemark = (KmlPlacemark) site.getExtendedData();
	// // boolean visibility = true;
	// // if (visibility != placemark.getVisibility())
	// // placemark.setVisibility(visibility);
	// }
	//
	// // ===============================================
	//
	// private HandlerRegistration mouse_listener = null;
	// private HandlerRegistration view_listener = null;
	//
	// @Override
	// protected void setActive(boolean value) {
	// Log.write("GEView::setActive, value = " + value);
	// if (value) {
	// mouse_listener = GE.getPlugin().getWindow()
	// .addMouseListener(new MouseListener() {
	//
	// private int x = 0;
	// private int y = 0;
	// private int z = 0;
	// private boolean dragging = false;
	//
	// @Override
	// public void onClick(KmlMouseEvent event) {
	// event.preventDefault();
	// if (event.getTarget().getType()
	// .equals("KmlPlacemark")) {
	// KmlPlacemark placemark = (KmlPlacemark) event
	// .getTarget();
	// onPlacemarkClick(placemark.getSnippet());
	// // JSONObject obj =
	// // JSONParser.parseLenient(json)
	// // .isObject();
	// // ViewableType type = ViewableType.valueOf(obj
	// // .get("type").isString().stringValue());
	// // int id = Integer.valueOf((int) obj.get("id")
	// // .isNumber().doubleValue());
	// // // Viewable viewable = Viewable.get(type,
	// // id);
	// // // TODO Здесь всё на соплях - обязательно
	// // нужно
	// // // будет переделать!!
	// // // if
	// // //
	// // ((viewable.equals(selected))&&(type.equals(ViewableType.BUILDING)))
	// // // Select(Viewable.get(ViewableType.SITE,
	// // // ((Building)viewable).getParent_id()));
	// // // else
	// // // Select(viewable);
	// }
	// }
	//
	// @Override
	// public void onDoubleClick(KmlMouseEvent event) {
	// }
	//
	// @Override
	// public void onMouseDown(KmlMouseEvent event) {
	// event.preventDefault();
	// switch (event.getButton()) {
	// case 0:
	// x = event.getClientX();
	// y = event.getClientY();
	// dragging = true;
	// break;
	// case 2:
	// z = event.getClientY();
	// dragging = true;
	// break;
	// }
	// }
	//
	// @Override
	// public void onMouseUp(KmlMouseEvent event) {
	// event.preventDefault();
	// dragging = false;
	// }
	//
	// @Override
	// public void onMouseOver(KmlMouseEvent event) {
	// }
	//
	// @Override
	// public void onMouseOut(KmlMouseEvent event) {
	// }
	//
	// @Override
	// public void onMouseMove(KmlMouseEvent event) {
	// event.preventDefault();
	// if (dragging) {
	// switch (event.getButton()) {
	// case 0: // LEFT
	// double dX = event.getClientX() - x;
	// double dY = event.getClientY() - y;
	// onUpdate(dX, dY, 0);
	// x += dX;
	// y += dY;
	// break;
	// case 2: // RIGHT
	// double dZ = event.getClientY() - z;
	// onUpdate(0, 0, dZ);
	// z += dZ;
	// break;
	// }
	// }
	// }
	// });
	// } else {
	// Log.write("1");
	// // if (mouse_listener != null)
	// mouse_listener.removeHandler();
	// Log.write("2");
	// }
	//
	// if (value) {
	// view_listener = GE.getView().addViewChangeListener(
	// new ViewChangeListener() {
	// @Override
	// public void onViewChangeBegin() {
	// // TODO Auto-generated method stub
	//
	// }
	//
	// @Override
	// public void onViewChange() {
	// // TODO Auto-generated method stub
	//
	// }
	//
	// private double centerX = GE.getEarth().getOffsetWidth() / 2;
	// private double centerY = GE.getEarth()
	// .getOffsetHeight() / 2;
	//
	// @Override
	// public void onViewChangeEnd() {
	// Viewable.Draw();
	// }
	// });
	// } else {
	// // Log.write("1");
	// if (view_listener != null)
	// view_listener.removeHandler();
	// // Log.write("2");
	// }
	//
	// }

}
