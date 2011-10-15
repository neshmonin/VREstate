package my.vrestate.client.core.Site;

import java.util.ArrayList;

import my.vrestate.client.Options;
import my.vrestate.client.core.IUserLoginedListener;
import my.vrestate.client.core.LookAt;
import my.vrestate.client.core.Point;
import my.vrestate.client.core.Requests;
import my.vrestate.client.core.User;
import my.vrestate.client.core.GEPlugin.GEPlugin;
import my.vrestate.client.core.GEPlugin.IGEPlugin;
import my.vrestate.client.core.GEPlugin.IPluginReadyListener;
import my.vrestate.client.core.GEPlugin.IViewChangedListener;
import my.vrestate.client.core.Views.SiteView;
import my.vrestate.client.interfaces.IUser;
import my.vrestate.client.Drawables.*;

import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONValue;

public class Site implements ISite, IViewChangedListener, IPluginReadyListener, IUserLoginedListener {
	private IGEPlugin GEPlugin = new GEPlugin();
	private ArrayList<IBuilding> Buildings = new ArrayList<IBuilding>();
	private Point Center = new Point();
	private int BuildingsCount = 0;
	private IUser User = new User();
	private Model Model = new Model();
	
	public ArrayList<IBuilding> getBuildings() {
		return this.Buildings;
	};
	
	public Site() {
//		View = new SiteView(this);
		this.GEPlugin.addPluginReadyListener(this);
		this.User.addUserLoginedListener(this);
		

		// GWT.log("Creating new Site...");
		// Events.AddEventHandler(this);
		// //
		// ==============================================================================
		// // Заглушка для получения Site с сервера
		// Name = "Eden Park Towers (Phase II) - Read Only";
		// GE_ID = "site25";
		// Id = 25;
		// // Tag = "Site25";
		// Position = new Position();
		// Position.setLatitude(43.83832436193);
		// Position.setLongitude(-79.39061896435);
		// Position.setAltitude(0);

		Center.setLatitude(43.83832436193);
		Center.setLongitude(-79.39061896435);
		Center.setAltitude(0);

		// Placemark = new Placemark("");
		// Placemark.setName(Name);
		// Placemark.setPosition(new Position(Position));
		// Placemark.setVisible(true);
		// ModelUrl = Options.SERVER_URL +
		// "Eden Park Towers (Phase II) - Read Only.kmz";
		// GEWrapper.UpdateSite(this);
		// GWT.log("New Site created...");
		// RequestBuildings();
		// LoadBuildings();
	};

	public void RequestBuildings() {
		GWT.log("Посылка запроса на Buildings...");
		String request = "data/building?site=25&sid=" + Options.SessionID;
		Requests.Add(request, onReceiveBuildings);
	}

	RequestCallback onReceiveBuildings = new RequestCallback() {
		@Override
		public void onResponseReceived(Request request, Response response) {
			JSONObject answer = (JSONObject) JSONParser.parseStrict(response
					.getText());
			JSONArray buildings = answer.get("buildings").isArray();
			int count = buildings.size();
			// int count = 1;
			BuildingsCount = count;
			for (int i = 0; i < count; i++) {
				JSONValue value = buildings.get(i);
				// new Building(Instance, value);
				addBuilding(value);
			}
		}

		@Override
		public void onError(Request request, Throwable exception) {
			// TODO Auto-generated method stub
		}
	};

	public void addBuilding(JSONValue value) {
		Building building = new Building();
		building.setSite(this);
		building.parse(value);
	}

	public void onBuildingLoaded(IBuilding building) {
		Buildings.add(building);
		if (this.BuildingsCount == Buildings.size()) {

			double lon = 0;
			double lat = 0;
			double alt = 0;
			for (IBuilding curr : Buildings) {
				lon += curr.getCenter().getLongitude();
				lat += curr.getCenter().getLatitude();
				alt += curr.getCenter().getAltitude();
			}
			this.Center.setLongitude(lon / Buildings.size());
			this.Center.setLatitude(lat / Buildings.size());
			this.Center.setAltitude(alt / Buildings.size());

			this.riseSiteLoaded();
		}
	}

// GEPlugin=====================================================================
	public void setGEPlugin(IGEPlugin ge_plugin) {
		this.GEPlugin = ge_plugin;
	}

	public IGEPlugin getGEPlugin() {
		return this.GEPlugin;
	}
//	User========================================================================
	public void setUser(IUser user) {
		this.User = user;
	}
	
	public IUser getUser() {
		return this.User;
	}

// Center=======================================================================
	public void setCenter(Point center) {
		Center = center;
	}

	public Point getCenter() {
		return Center;
	}

	// А здесь начинается возня с событиями
	private ArrayList<ISiteLoadedListener> SiteLoadedListeners = new ArrayList<ISiteLoadedListener>();

	public void addSiteLoadedListener(ISiteLoadedListener listener) {
		this.SiteLoadedListeners.add(listener);
	}

	public void removeSiteLoadedListener(ISiteLoadedListener listener) {
		this.SiteLoadedListeners.remove(listener);
	}

	private void riseSiteLoaded() {
		GWT.log("Rising SiteLoaded ready");
		for (ISiteLoadedListener listener : SiteLoadedListeners)
			listener.onSiteLoaded(this);
	}
//А вот методы-обработчики событий
	@Override
	public void onPluginReady() {
		
//		View = new SiteView(this);
//		GWT.log("x");
//		this.addSiteLoadedListener((SiteView)View);

		GWT.log("Загрузка плагина закончилась. Логин пользователя...");
		User.Login();
		// TODO Auto-generated method stub
		Model.setModelUrl(Options.SERVER_URL + "Eden Park Towers (Phase II) - Read Only.kmz");
		Model.setVisible(true);
		
	}
	
	@Override
	public void onUserLogined() {
		GWT.log("Пользователь залогинился.");
		SiteView View = new SiteView(this);
		this.addSiteLoadedListener(View);
		Load();
	}
	
	@Override
	public void onViewChanged() {
		for (IBuilding building : Buildings)
			building.rePaint();

		LookAt look_at = GEPlugin.getLookAt();
		look_at.setLongitude(Center.getLongitude());
		look_at.setLatitude(Center.getLatitude());
		look_at.setAltitude(Center.getAltitude());
		GEPlugin.setLookAt(look_at);
		Options.CurrentLookAt = look_at;
		// TODO Auto-generated method stub

	}


//Остальные методы
	@Override
	public void Load() {
		GWT.log("Начало загрузки сайта...");
		RequestBuildings();
	}

	@Override
	public Point getCenterPos() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void setCenterPos(Point center_pos) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public ArrayList<ISuite> getSuites() {
		ArrayList<ISuite> suites = new ArrayList<ISuite>();
		for (IBuilding building : this.getBuildings())
			suites.addAll(building.getSuites());
		return suites;
	}

}
