package my.vrestate.client.Drawables;

import com.google.gwt.core.client.GWT;
import com.google.gwt.user.client.Window;

import my.vrestate.client.Options;
import my.vrestate.client.VREstate;
import my.vrestate.client.core.GEPlugin.GEPlugin;

public class Placemark implements IDrawable {
	
	private double Longitude = 0;
	private double Latitude = 0;
	private double Altitude = 0;

	private String Id = "";
	private String Name = "";
	private int DrawId = -1;
	private boolean Visible = false;
	
	private String Description = "";
	private boolean DescriptionVisible = false;
	private boolean Highlighted = false;
	

	@Override
	public String getKML() {
		String kml = "" +
				"<kml xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
//				"<Placemark id=\"" + Id + "\">" +
				"<Placemark>" +
//						"<Style>" +
//						"<LabelStyle>" +
//							"<color>" + ((this.Highlighted)? "ffaa00cc":"ff00ffcc") + "</color>" +
//							"<scale>" + ((this.Highlighted)? 2:1) + "</scale>" +
//						"</LabelStyle>" +
//						"<IconStyle>" +
//							"<scale>0.5</scale>" +
//							"<Icon>" +
//							"<href>" + Options.BUTTONS_URL + "help.png" + "</href>" +
//							"</Icon>" +
//						"</IconStyle>" +
//						"<LineStyle>" +
//							"<gx:LabelVisibility>1</gx:LabelVisibility>" +
//						"</LineStyle>" +
//						"<BalloonStyle>" +
//						"<bgColor>7fff0000</bgColor>" +
//						"</BalloonStyle>" +
//						"</Style>" +
					"<name>" + Name + "</name>" +
							"<visibility>1</visibility>" +
//					"<description>" +
//						"I am a description" +
////						this.Description + 
//					"</description>" +
//					"<gx:balloonVisibility>" +
//						"0" +
////						((this.DescriptionVisible) ? 1 : 0) +
//					"</gx:balloonVisibility>" +
//					"<LineString>" +
//						"<coordinates>" + 
//							Longitude + "," + 
//							Latitude + "," + 
//							Altitude + " " + 
//							Longitude + "," + 
//							(Latitude+1) + "," + 
//							(Altitude+2) + " " + 
//							0 + 
//						"</coordinates>" +
//					"</LineString>" +
					"<Point>" +
						"<altitudeMode>" +
							"relativeToGround" +
						"</altitudeMode>" +
						"<coordinates>" + 
							Longitude + "," + 
							Latitude + "," + 
							Altitude + 
						"</coordinates>" +
					"</Point>" +
				"</Placemark>" +
				"</kml>";
//		Window.alert(kml);
		return kml;
	}

	
	public void setId( String id) {
		this.Id = id;
	}
	public String getId() {
		return this.Id;
	}
	
	@Override
	public void setDrawId(int draw_id) {
		this.DrawId = draw_id;
	}

	@Override
	public int getDrawId() {
		return DrawId;
	}

	@Override
	public void setVisible(boolean visible) {
//		if (this.Selected)
//			Window.alert(getKML());
		this.Visible = !visible;
		GEPlugin.UpdateDrawable(this);
		this.Visible = visible;
		GEPlugin.UpdateDrawable(this);
	}

	@Override
	public boolean isVisible() {
		return this.Visible;
	}
	
	public void setName(String name) {
		this.Name = name;
	}
	public void setLongitude(double longitude) {
		this.Longitude = longitude;
	}
	public void setLatitude(double latitude) {
		this.Latitude = latitude;
	}
	public void setAltitude(double altitude) {
		this.Altitude = altitude;
	}


	public void setDescription(String description) {
		Description = description;
	}


	public String getDescription() {
		return Description;
	}


	public void setDescriptionVisible(boolean descriptionVisible) {
		DescriptionVisible = descriptionVisible;
	}


	public boolean isDescriptionVisible() {
		return DescriptionVisible;
	}
	
	public void setSelected(boolean selecte) {
//		this.Selected = selected;
//		GWT.log("Placemark=>Selected = " + this.Selected);
	}

}
