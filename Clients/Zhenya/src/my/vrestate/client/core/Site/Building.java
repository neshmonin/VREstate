package my.vrestate.client.core.Site;

import java.util.ArrayList;

import my.vrestate.client.Options;
import my.vrestate.client.Drawables.Placemark;
import my.vrestate.client.core.Point;
import my.vrestate.client.core.Requests;

import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONValue;

public class Building implements IBuilding {
	private ArrayList<ISuite> Suites = new ArrayList<ISuite>();
	private int SuitesCount = 0;
	private Site Site = null;
	private int Id = 0;
	private String Tag = "";
	private String Name = "";
	private Point Center = new Point();
	private Point Position = new Point();
	private Placemark Placemark = new Placemark();

	public void parse (JSONValue value) {
		JSONObject object = value.isObject();
//		GWT.log("Начало парсинга нового Building'а...");
		Id = (int) object.get("id").isNumber().doubleValue();
		this.Tag += "Building" + Id;
		Name = object.get("name").toString();

		JSONObject JPosition = (JSONObject) object.get("position");
		Position.setLatitude(JPosition.get("lat").isNumber().doubleValue());
		Position.setLongitude(JPosition.get("lon").isNumber().doubleValue());
		Position.setAltitude(JPosition.get("alt").isNumber().doubleValue());

		//
//		Placemark = new Placemark(Tag + "/Placemark");
		Placemark.setName(Name);
		// Placemark.setVisible(true);

		// Wrapper.UpdatePlacemark(Placemark);
//		GWT.log("Building пропарсен. Загружаем квартиры для него.");
		RequestSuites();
		// }
		GWT.log(Name);
//		this.Site.onBuildingLoaded(this);
	}
	
	public void RequestSuites() {
		GWT.log("Requesting Suites...");
		String request = "data/suite?building=" + String.valueOf(Id) + "&sid=" + Options.SessionID;
		Requests.Add(request, onReceiveSuites);
	}
	
	RequestCallback onReceiveSuites =   
		new RequestCallback() {
			@Override
			public void onResponseReceived(Request request, Response response) {
				JSONObject answer = (JSONObject) JSONParser.parseStrict(response.getText());
//				Window.alert(response.getText());
				JSONArray suites = answer.get("suites").isArray();
				int count = suites.size();
				SuitesCount = count;
				GWT.log("SuitesCount = " + SuitesCount);
				for (int i = 0; i < count; i++) {
					JSONValue value = suites.get(i);
//					new Building(Instance, value);
					addSuite(value);
				}
			}
	
			@Override
			public void onError(Request request, Throwable exception) {
			// TODO Auto-generated method stub
			}
		};
		
	public void addSuite(JSONValue value) {
		Suite suite = new Suite();
		suite.setBuilding(this);
		suite.parse(value);
//		this.onSuiteLoaded(suite);
	}
	
	public void onSuiteLoaded(ISuite suite) {
		Suites.add(suite);
		
		if (this.SuitesCount == Suites.size()) {
//			this.UpdatePlacemark();
			
			double lon = 0;
			double lat = 0;
			double alt = 0;
			for (ISuite curr_suite : Suites) {
				lon += curr_suite.getPosition().getLongitude();
				lat += curr_suite.getPosition().getLatitude();
				alt += curr_suite.getPosition().getAltitude();
			}
			this.Center.setLongitude(lon / Suites.size());
			this.Center.setLatitude(lat / Suites.size());
			this.Center.setAltitude(alt / Suites.size());
			
			this.Site.onBuildingLoaded(this);
		}
	}
	
	public void rePaint() {
		for (ISuite suite : Suites)
			suite.rePaint();
		Placemark.setLongitude(Center.getLongitude());
		Placemark.setLatitude(Center.getLatitude());
		Placemark.setAltitude(Center.getAltitude() * 3);
		Placemark.setVisible(true);
//		GWT.log("Updating building");
	}
	
	public void setSite(Site site) {
		Site = site;
	}

	public Site getSite() {
		return Site;
	}
	
	public void setCenter(Point center) {
		Center = center.clone();
	}
	
	public Point getCenter() {
		return Center;
	}

	@Override
	public ArrayList<ISuite> getSuites() {
		return Suites;
	}

}
