package com.condox.vrestate.client;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Site;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.SiteView;
import com.condox.vrestate.client.view._AbstractView;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;
import com.nitrous.gwt.earth.client.api.event.KmlLoadCallback;

public class VREstate implements EntryPoint, RequestCallback, KmlLoadCallback {

	@Override
	public void onModuleLoad() {
		Log.write("onModuleLoad -> Options.Init(this);");
		Options.Init(this);
	}

	public void LoginUser() {
		Log.write("LoginUser -> User.Login(this);");
		User.Login(this);
	};

	private GE ge = null;
	public void StartGE() {
		Log.write("StartGE -> ge.Init(vrEstate);");
		ge = new GE();
		ge.Init(this);
	};

	public void LoadView() {
		Log.write("LoadView");
		String url;
		if (Options.isViewOrder()) {
			url = Options.HOME_URL
					+ "data/view?type=viewOrder&id="
					+ Options.getViewOrderId() + "&SID=" + User.SID;
//			Window.open(url, "", "")
		} else {
			url = Options.HOME_URL
					+ "data/view?type=site&id="
					+ Options.getSiteId() + "&SID=" + User.SID;
//			Window.open(url, "", "");
		}
		GET.send(url, this);
	}

	@Override
	public void onResponseReceived(Request request,
			Response response) {
		String json = response.getText();
		Document.get().Parse(json);

		Site site = (Site) Document.get().getSites().toArray()[0];
		if (!Options.isViewOrder())
			if (site.getDisplayModelUrl() != "")
				GE.getPlugin().fetchKml(Options.HOME_URL + site.getDisplayModelUrl(), this);

		_AbstractView.CreateAllGeoItems();
		final SiteView view = new SiteView(_AbstractView.getSiteGeoItem(site.getId()));
		_AbstractView.Push(view);
		
		//=================================================================
//		KmlIcon icon = GE.getPlugin().createIcon("");
//		KmlScreenOverlay overlay = GE.getPlugin().createScreenOverlay("");
//		overlay.setIcon(icon);
//		overlay.getOverlayXY().set(50, KmlUnits.UNITS_PIXELS, 100,KmlUnits.UNITS_INSET_PIXELS);
//		overlay.getScreenXY().set(0, KmlUnits.UNITS_PIXELS, 0,KmlUnits.UNITS_INSET_PIXELS);
//		GE.getPlugin().getFeatures().appendChild(overlay);
//		String href = "Filtered 100 suites out of 200 (Prices from $1000,000 to $1000,000; Bathrooms:1,2(dens),3,4(dens); Bedrooms:1,2(dens),3,4(dens); Area: from $1000,000 to $1000,000;";
//		href = Options.HOME_URL + "gen/txt?height=15&shadow=2&text="
//				+ href + "&txtClr=16777215&shdClr=0&frame=0";
//		icon.setHref(href);
//		overlay.setVisibility(true);
//		overlay.setOpacity(0.5f);
		//=================================================================
	}

	@Override
	public void onError(Request request,
			Throwable exception) {
	}
	
	@Override
	public void onLoaded(KmlObject feature) {
		if (feature != null)
			GE.getPlugin().getFeatures().appendChild(feature);
	}

}
// private static final String EARTH_API_KEY =
// "ABQIAAAAm7LIvLNR-PkJLewH4qmS7hREGtQZq9OFJfHndXhPP8gxXzlLARQtA_EfZjc9zs77WO25FrLcaZ4ZVA";