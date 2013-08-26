package com.condox.vrestate.client;

import com.condox.clientshared.abstractview.I_AbstractView;
import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.I_Login;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.UpdatesFromServer;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.document.Building;
import com.condox.clientshared.document.Document;
import com.condox.clientshared.document.Site;
import com.condox.clientshared.document.ViewOrder.ProductType;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.HelicopterView;
import com.condox.vrestate.client.view.ProgressBar;
import com.condox.vrestate.client.view.SiteView;
import com.condox.vrestate.client.view._AbstractView;
import com.condox.vrestate.client.view.GeoItems.SiteGeoItem;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.event.KmlLoadCallback;

public class VREstate implements EntryPoint, RequestCallback, KmlLoadCallback, I_Login {

	/**
	 * @wbp.parser.entryPoint
	 */
	@Override
	public void onModuleLoad() {
		// _AbstractView.init();
		// init();
		Options.Init();
		LoginUser();
	}

	// private native void init() /*-{
	// $wnd.onTimerExpire = function(expired) {
	// if (expired)
	// @com.condox.vrestate.client.view._AbstractView::onTimerTimeout()();
	// else
	// @com.condox.vrestate.client.view._AbstractView::onTimerReset()();
	// }
	// }-*/;

	public void LoginUser() {
		User.Login(this);
	};

	private GE ge = null;

	public void StartGE() {
		ge = new GE();
		ge.Init(this);
	};

	public static int checkChangesPeriodSec;

	public void LoadView() {
		String url;
		if (Options.isViewOrder()) {
			url = Options.HOME_URL + "data/view?type=viewOrder&id="
					+ Options.getViewOrderId() + "&track=true&&SID=" + User.SID;
		} else {
			url = Options.HOME_URL + "data/view?type=site&id="
					+ Options.getSiteId() + "&track=true&SID=" + User.SID;
		}
		GET.send(url, this);
	}

	I_AbstractView firstView = null;

	@Override
	public void onResponseReceived(Request request, Response response) {
		String json = response.getText();
		Document.progressBar = new ProgressBar();
		if (Document.get().Parse(json)) {
			Site site = (Site) Document.get().getSites().values().toArray()[0];
			if (!Options.isViewOrder()
					|| Document.targetViewOrder.getProductType() == ProductType.PublicListing
					|| Document.targetViewOrder.getProductType() == ProductType.Building3DLayout) {
				boolean useSiteModel = true;
				for (Building bldng : Document.get().getBuildings().values()) {
					if (bldng.getDisplayModelUrl() != "") {
						GE.getPlugin().fetchKml(bldng.getDisplayModelUrl(),this);
						useSiteModel = false;
					}
					if (bldng.getOverlayUrl() != "")
						GE.getPlugin().fetchKml(bldng.getOverlayUrl(), this);
					if (bldng.getPOIUrl() != "")
						GE.getPlugin().fetchKml(bldng.getPOIUrl(), this);
				}
				if (useSiteModel)
					if (site.getDisplayModelUrl() != "")
						GE.getPlugin().fetchKml(site.getDisplayModelUrl(), this);
			}

			_AbstractView.CreateAllGeoItems();

			switch (Options.ROLE) {
			case KIOSK: {
				_AbstractView.enableTimeout(true);
				int id = site.getId();
				SiteGeoItem geoItem = _AbstractView.getSiteGeoItem(id);
				firstView = new HelicopterView(geoItem);
				_AbstractView.ResetTimeOut();
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

			Document.abstractView = _AbstractView.getCurrentView();
			UpdatesFromServer.RegisterHandler(Document.get());
			UpdatesFromServer.RenewCheckChangesThread();
		}
		// =================================================================
		// KmlIcon icon = GE.getPlugin().createIcon("");
		// KmlScreenOverlay overlay = GE.getPlugin().createScreenOverlay("");
		// overlay.setIcon(icon);
		// overlay.getOverlayXY().set(50, KmlUnits.UNITS_PIXELS,
		// 100,KmlUnits.UNITS_INSET_PIXELS);
		// overlay.getScreenXY().set(0, KmlUnits.UNITS_PIXELS,
		// 0,KmlUnits.UNITS_INSET_PIXELS);
		// GE.getPlugin().getFeatures().appendChild(overlay);
		// String href =
		// "Filtered 100 suites out of 200 (Prices from $1000,000 to $1000,000; Bathrooms:1,2(dens),3,4(dens); Bedrooms:1,2(dens),3,4(dens); Area: from $1000,000 to $1000,000;";
		// href = Options.HOME_URL + "gen/txt?height=15&shadow=2&text="
		// + href + "&txtClr=16777215&shdClr=0&frame=0";
		// icon.setHref(href);
		// overlay.setVisibility(true);
		// overlay.setOpacity(0.5f);
		// =================================================================
	}

	@Override
	public void onError(Request request, Throwable exception) {
	}

	@Override
	public void onLoaded(KmlObject feature) {
		if (feature != null)
			GE.getPlugin().getFeatures().appendChild(feature);
	}

	@Override
	public void onLoginSucceed() {
		StartGE();
	}

	@Override
	public void onLoginFailed(Throwable exception) {
		Log.write("Failed to Login: " + exception.toString());
	}
}
// private static final String EARTH_API_KEY =
// "ABQIAAAAm7LIvLNR-PkJLewH4qmS7hREGtQZq9OFJfHndXhPP8gxXzlLARQtA_EfZjc9zs77WO25FrLcaZ4ZVA";