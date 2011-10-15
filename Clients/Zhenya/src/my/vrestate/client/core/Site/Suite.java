package my.vrestate.client.core.Site;

import my.vrestate.client.Options;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

import my.vrestate.client.Drawables.Placemark;
import my.vrestate.client.core.LookAt;
import my.vrestate.client.core.Point;
import my.vrestate.client.core.GEPlugin.IGEPlugin;

public class Suite implements ISuite {
	
	private Building Building = null; // Дом, в котором находится данная квартира
	private Point Position = new Point(); // Координаты 
	private Placemark Placemark = new Placemark(); // Подпись к квартире
	private boolean Visible = false; // Указывает на то, видима ли квартира в данный момент
	
	private int Id = 0;
	private String Tag = "";
	private String Name = "";
	private double HDG = 0;
	private Status Status = ISuite.Status.AVAILABLE;
	
	public void parse (JSONValue value) {
		JSONObject object = value.isObject();
//		GWT.log("Suite=>parse...");
		Id = (int) object.get("id").isNumber().doubleValue();
		this.Tag += "Suite" + Id;
		Name = object.get("name").toString();

		JSONObject JPosition = (JSONObject) object.get("position");
		Position.setLatitude(JPosition.get("lat").isNumber().doubleValue());
		Position.setLongitude(JPosition.get("lon").isNumber().doubleValue());
		Position.setAltitude(JPosition.get("alt").isNumber().doubleValue());
		HDG = JPosition.get("hdg").isNumber().doubleValue();
		int status_code = (int)object.get("status").isNumber().doubleValue();
		switch(status_code) {
		case 0: {
			setStatus(ISuite.Status.AVAILABLE);
			break;
		}
		case 1: {
			setStatus(ISuite.Status.ON_HOLD);
			break;
		}
		case 2: {
			setStatus(ISuite.Status.SOLVED);
			break;
		}
		
		}
		
		Placemark.setId(Tag);
		
		Placemark.setName(Name.replace('\"', ' '));
		Placemark.setLatitude(Position.getLatitude());
		Placemark.setLongitude(Position.getLongitude());
		Placemark.setAltitude(Position.getAltitude());
//		GWT.log("LAT:" + Position.getLatitude());
//		GWT.log("ALT:" + Position.getAltitude());
		
		this.Building.onSuiteLoaded(this);
	}
	
	

	@Override
	public void rePaint() {
//		if (Options.CurrentLookAt == null)
//			Options.CurrentLookAt = GEPlugin.getLookAt();
		// LookAt LookAt = GEWrapper.getLookAt();
		LookAt look_at = Options.CurrentLookAt;

		boolean visible = true;
		visible &= (look_at.getRange() < Options.MAX_SUITE_PLACEMARK_VISIBLE);
		visible &= (look_at.getRange() > Options.MIN_SUITE_PLACEMARK_VISIBLE);
		double suite_hdg = HDG + 180;
		double lookat_hdg = look_at.getHeading();
		double d = suite_hdg - lookat_hdg;
		while (d < -180) d += 360;
		while (d >  180) d -= 360;
		d = Math.abs(d);
		visible &= (d < 70);
		if (!Options.SUITES_SHOW_AVAILABLE)
			visible &= (getStatus() != my.vrestate.client.core.Site.ISuite.Status.AVAILABLE);
		
		if (!Options.SUITES_SHOW_ONHOLD)
			visible &= (getStatus() != my.vrestate.client.core.Site.ISuite.Status.ON_HOLD);
		
		if (!Options.SUITES_SHOW_SOLVED)
			visible &= (getStatus() != my.vrestate.client.core.Site.ISuite.Status.SOLVED);
		
//		visible &= (getStatus() != my.vrestate.client.interfaces.ISuite.Status.SOLVED);
		
		this.setVisible(visible);
	}


	@Override
	public void setSelected(boolean selected) {
		this.Placemark.setSelected(selected);
		this.rePaint();
	}
	
	private void setVisible(boolean visible) {
		this.Visible = visible;
		if (Placemark.isVisible() != visible) {
			Placemark.setVisible(visible);
		}
		
	}
	
	public boolean isVisible() {
		return this.Visible;
	}


	@Override
	public void showInfo() {
		String info_data = "" +
				"<table border=\"1\">" +
					"<tr>" +
						"<td align = \"center\">" +
							"Статус" +
						"</td>" +
						"<td align = \"center\">" +
							((getStatus() == ISuite.Status.AVAILABLE)? 	"<a style = \"color:#00ff00\">Available	</a>":"") +
							((getStatus() == ISuite.Status.ON_HOLD)? 	"<a style = \"color:#ffff00\">On hold	</a>":"") +
							((getStatus() == ISuite.Status.SOLVED)? 	"<a style = \"color:#ff0000\">Solved	</a>":"") +
						"</td>" +
					"</tr>" +
						"<td>" +
							"Column1" +
						"</td>" +
						"<td>" +
							"Column2" +
						"</td>" +
					"<tr>" +
					"</tr>" +
				"</table>";
		int id = getPlacemark().getDrawId();
		IGEPlugin ge_plugin = getBuilding().getSite().getGEPlugin();
		ge_plugin.ShowBalloon(id, info_data);
	}


	@Override
	public void setStatus(my.vrestate.client.core.Site.ISuite.Status status) {
		this.Status = status;
	}

	@Override
	public my.vrestate.client.core.Site.ISuite.Status getStatus() {
		return this.Status;
	}

//Остача - все сеттеры и геттеры
	public void setBuilding(Building building) {
		Building = building;
	}
	
	public IBuilding getBuilding() {
		return Building;
	}
	
	@Override
	public void setPosition(Point position) {
		
	}

	public Point getPosition() {
		return this.Position;
	}
	
	private Placemark getPlacemark() {
		return this.Placemark;
	};

	@Override
	public String getTag() {
		return this.Tag;
	}
	
	@Override
	public String getName() {
		return this.Name;
	}



	@Override
	public double getHDG() {
		return HDG;
	}
	
	
}
