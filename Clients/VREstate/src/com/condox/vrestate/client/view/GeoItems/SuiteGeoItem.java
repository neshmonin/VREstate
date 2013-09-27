package com.condox.vrestate.client.view.GeoItems;


import com.condox.clientshared.abstractview.IGeoItem;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.document.Building;
import com.condox.clientshared.document.Document;
import com.condox.clientshared.document.Position;
import com.condox.clientshared.document.Suite;
import com.condox.clientshared.document.ViewOrder;
import com.condox.clientshared.document.Suite.Orientation;
import com.condox.clientshared.utils.StringFormatter;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view._AbstractView;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.json.client.JSONValue;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlMultiGeometry;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlPoint;
import com.nitrous.gwt.earth.client.api.KmlStyle;

public class SuiteGeoItem implements IGeoItem {
	public enum GeoStatus {
		Sold, 
		Available, 
		OnHold, 
		ResaleAvailable,
		Selected,
		Layout,
		AvailableRent,
		NotSupported
	}
	
	public static final int MaxRentTreshold = 10000;

	private final double initialRange_m = 30;
	private final double initialTilt_d = 75;
	public String kmlPlacemark = "";
	public String kmlStyle = "";

	public Suite suite = null;
	GeoStatus geoStatus = GeoStatus.NotSupported;
	String href = null;
	public SuiteGeoItem(Suite suite){
		Init(suite);
	}
	public void Init(Suite suite){

		this.suite = suite;

		if (Document.targetViewOrder!=null&&
			Document.targetViewOrder.getProductType() == ViewOrder.ProductType.Building3DLayout)
			geoStatus = GeoStatus.Layout;
		else {
			Suite.Status status = suite.getStatus();
			switch (status) {
			case Sold: geoStatus = GeoStatus.Sold; break;
			case Available: geoStatus = GeoStatus.Available; break;
			case OnHold: geoStatus = GeoStatus.OnHold; break;
			case ResaleAvailable: geoStatus = GeoStatus.ResaleAvailable; break;
			case AvailableRent: geoStatus = GeoStatus.AvailableRent; break;
			case NotSupported: geoStatus = GeoStatus.NotSupported; break;
			default:
				geoStatus = GeoStatus.NotSupported; break;
			}
		}
		
		String colorKml = "";
		String widthKml = "";
		float scale = 1.0F;
		KmlStyle style = GE.getPlugin().createStyle("");		

		switch (getGeoStatus()) {
		case Available:
			href = Options.URL_VRT + "gen/txt?height=25&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=65280&shdClr=65280&frame=0";
			colorKml = "FF00FF00"; // GREEN
			widthKml = "2";
			break;
		case OnHold:
			href = Options.URL_VRT + "gen/txt?height=25&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=16776960&shdClr=16776960&frame=0";
			colorKml = "FF00FFFF"; // YELLOW
			widthKml = "2";
			break;
		case Sold:
			if (Options.getShowSold()) {
				href = Options.URL_VRT + "gen/txt?height=25&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=16711680&shdClr=0&frame=0";
				colorKml = "FF0000FF"; // RED
				widthKml = "2";
			}
			else
			{
				href = Options.URL_VRT + "gen/txt?height=25&shadow=2&text=.&txtClr=0&shdClr=0&frame=0";
				colorKml = "00000000"; // transparent
				widthKml = "2";
			}
			break;
		case ResaleAvailable:
			href = Options.URL_VRT + "gen/txt?height=25&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=1048575&shdClr=0&frame=0";
			colorKml = "FFFFFF00"; // LIGHT BLUE
			widthKml = "2";
			break;
		case AvailableRent:
			href = Options.URL_VRT + "gen/txt?height=25&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=14854399&shdClr=0&frame=0";
			colorKml = "FFE2A8FF"; // PINK
			widthKml = "2";
			break;
		case Selected:
			href = Options.URL_VRT + "gen/txt?height=30&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=16764108&shdClr=0&frame=0";
			colorKml = "FFCCCCFF"; // LIGHT RED
			widthKml = "4";
			scale = 1.0F;
			break;
		case Layout:
			href = Options.URL_VRT + "gen/txt?height=25&shadow=2&text="
					+ suite.getName()
					+ "&txtClr=16777215&shdClr=0&frame=0";
			colorKml = "FFFFFFFF"; // WHITE
			widthKml = "3";
			break;
		default:
			break;
		}
	
		kmlStyle = StringFormatter.format(
				"<Style id=\"s{0}\">" +
					"<LineStyle>" +
						"<color>{1}</color>" +
						"<width>{2}</width>" +
					"</LineStyle>" +
				"</Style>",
				suite.getId(),
				colorKml,
				widthKml);

		if (href == null) return;

		KmlIcon icon = GE.getPlugin().createIcon("");
		
		icon.setHref(href);
		style.getIconStyle().setIcon(icon);
		style.getIconStyle().setScale(scale);

		String altitudeMode = "relativeToGround";

		if (extended_data_label != null)
			extended_data_label.setStyleSelector(style);
		else {
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
			if (suite.orientation == Orientation.Horizontal) {
				position.setTilt(85);
				position.setRange(50);
			}
			else {
				position.setTilt(initialTilt_d);
				position.setRange(initialRange_m);
			}
			
			point.setLatitude(position.getLatitude());
			point.setLongitude(position.getLongitude());
			
			Building parent = suite.getParent();
			if ((parent != null) && (parent.hasAltitudeAdjustment())) {
				point.setAltitude(position.getAltitude()
						+ parent.getAltitudeAdjustment() + parent.getPosition().getAltitude() + 1);
				point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_ABSOLUTE);
				altitudeMode = "absolute";
			} else {
				point.setAltitude(position.getAltitude());
				point.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
			}
			
			geometry1.getGeometries().appendChild(point);
			extended_data_label.setGeometry(geometry1);
			extended_data_label.setStyleSelector(style);

			GE.getPlugin().getFeatures().appendChild(extended_data_label);
		}
	
		/****************************************************/
		String kmlLineStrings = "";
		for (int j = 0; j < suite.getPoints().size(); j += 6) {
			kmlLineStrings += StringFormatter.format(
					"<LineString>" +
						"<altitudeMode>{0}</altitudeMode>"+
						"<coordinates>" +
							"{1},{2},{3} {4},{5},{6} " +
						"</coordinates>" +
					"</LineString>", 
					altitudeMode,
					suite.getPoints().get(j + 1), 
					suite.getPoints().get(j + 0),
					suite.getPoints().get(j + 2),
					suite.getPoints().get(j + 4),
					suite.getPoints().get(j + 3),
					suite.getPoints().get(j + 5)
					);
		}
		
		kmlPlacemark = StringFormatter.format(
				"<Placemark id=\"{0}\">"+
					"<styleUrl>#s{1}</styleUrl>"+
					"<visibility>1</visibility>"+
					"<MultiGeometry>" +
						"{2}"+
					"</MultiGeometry>" +
				"</Placemark>", 
				suite.getId(), 
				suite.getId(), 
				kmlLineStrings);

		//String kmlWires = StringFormatter.format(
		//		"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
		//		"<kml xmlns=\"http://www.opengis.net/kml/2.2\">" +
		//			"<Document>" +
		//				"{0}" +
		//			"</Document>" +
		//		"</kml>", kmlStyle + kmlPlacemark);
		
		//extended_data_lines = (KmlPlacemark) GE.getPlugin().parseKml(kmlWires);

		/****************************************************/

		//GE.getPlugin().getFeatures().appendChild(GE.getPlugin().parseKml(kmlWires));
	}
	
	public GeoStatus getGeoStatus() {
		return geoStatus;
	}
	
	public void setGeoStatus(GeoStatus stat) {
		geoStatus = stat;
	}
	
	public void onHeadingChanged(double heading_d) {
		if (href == null) return;

		boolean visible = isFilteredIn && isVisible(heading_d);
		if (extended_data_label.getVisibility() != visible)
			extended_data_label.setVisibility(visible);
	}
	
	private boolean isFilteredIn = true;
	public boolean ShowIfFilteredIn() {
		if (href == null)
			return false;

		isFilteredIn = Filter.get().isFilteredIn(this);
		if (!isFilteredIn && filteredOutNotificationHandler != null)
			filteredOutNotificationHandler.onFilteredOut();

		//if (extended_data_lines.getVisibility() != isFilteredIn)
		//	extended_data_lines.setVisibility(isFilteredIn);
		if (extended_data_label.getVisibility() != isFilteredIn)
			extended_data_label.setVisibility(isFilteredIn);
		Integer intId = suite.getId();
		KmlObject kmlObject = GE.getPlugin().getElementById(intId.toString());
		if (kmlObject != null) {
			KmlPlacemark wiresPlacemark = (KmlPlacemark) kmlObject; 
			wiresPlacemark.setVisibility(isFilteredIn);
		}
		
		return isFilteredIn;
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
		
		if (suite.orientation == Orientation.Horizontal)
			return true; // always visible
		
		double suiteHeading = suite.getPosition().getHeading();
		heading_d += 180;
		if (heading_d > 360)
			heading_d -= 360;
		double diff = Math.abs(heading_d - suiteHeading);
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

	//private KmlPlacemark extended_data_lines = null;
	
	//public KmlPlacemark getExtendedDataLines() {
	//	return extended_data_lines;
	//}
	
	public String getFloor_name() {
		return suite.getFloorName();
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
