package com.condox.vrestate.client;

import com.condox.vrestate.client.document.Building;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Site;
import com.condox.vrestate.client.document.ViewOrder.ProductType;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.HelicopterView;
import com.condox.vrestate.client.view.I_AbstractView;
import com.condox.vrestate.client.view.SiteView;
import com.condox.vrestate.client.view._AbstractView;
import com.condox.vrestate.client.view.GeoItems.SiteGeoItem;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.event.KmlLoadCallback;

public class VREstate implements EntryPoint, RequestCallback, KmlLoadCallback {

	/**
	 * @wbp.parser.entryPoint
	 */
	@Override
	public void onModuleLoad() {
		Options.Init(this);
	}

	public void LoginUser() {
		User.Login(this);
	};

	private GE ge = null;
	public void StartGE() {
		ge = new GE();
		ge.Init(this);
	};

	private static int counter = 0;

	public static void RenewCheckChangesThread() {
		counter++;
		String request = Options.HOME_URL + "ev?sid=" + User.SID + "&generation=" + counter;
		GET.send(request, Document.getCallback());
	}
	public static int checkChangesPeriodSec;

	public void LoadView() {
		String url;
		if (Options.isViewOrder()) {
			url = Options.HOME_URL
					+ "data/view?type=viewOrder&id="
					+ Options.getViewOrderId() + "&track=true&SID=" + User.SID;
		} else {
			url = Options.HOME_URL
					+ "data/view?type=site&id="
					+ Options.getSiteId() + "&track=true&SID=" + User.SID;
		}
		GET.send(url, this);

		VREstate.RenewCheckChangesThread();
	}

	I_AbstractView firstView = null;

	@Override
	public void onResponseReceived(Request request,
			Response response) {
		String json = response.getText();
		if (Document.get().Parse(json)) {
			Site site = (Site) Document.get().getSites().toArray()[0];
			if (!Options.isViewOrder() ||
					Document.targetViewOrder.getProductType() == ProductType.PublicListing ||
					Document.targetViewOrder.getProductType() == ProductType.Building3DLayout) {
				if (site.getDisplayModelUrl() != "")
					GE.getPlugin().fetchKml(Options.HOME_URL + site.getDisplayModelUrl(), this);
				
				for (Building bldng : Document.get().getBuildings()) {
					if (bldng.getDisplayModelUrl() != "")
						GE.getPlugin().fetchKml(bldng.getDisplayModelUrl(), this);
				}
			}

			_AbstractView.CreateAllGeoItems();
			
			switch (Options.ROLE) {
			case KIOSK: {
				_AbstractView.enableTimeout(true);
				int id = site.getId();
				SiteGeoItem geoItem = _AbstractView.getSiteGeoItem(id);
				firstView = new HelicopterView(geoItem);
				break;
			}
			case VISITOR: {
				_AbstractView.enableTimeout(false);
				int id = site.getId();
				SiteGeoItem geoItem = _AbstractView.getSiteGeoItem(id);
				firstView = new SiteView(geoItem);
				break;
			}
			}

			_AbstractView.Push(firstView);
		}
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