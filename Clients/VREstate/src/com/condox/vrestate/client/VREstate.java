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
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.tree.PopupContainer;
import com.condox.vrestate.client.tree.VREstateTree;
import com.condox.vrestate.client.view.HelicopterView;
import com.condox.vrestate.client.view.ProgressBar;
import com.condox.vrestate.client.view.SiteView;
import com.condox.vrestate.client.view._AbstractView;
import com.condox.vrestate.client.view.GeoItems.SiteGeoItem;
import com.google.gwt.core.client.EntryPoint;
import com.google.gwt.dom.client.Element;
import com.google.gwt.dom.client.NodeList;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.user.client.ui.HTML;
import com.nitrous.gwt.earth.client.api.GEHtmlStringBalloon;
import com.nitrous.gwt.earth.client.api.GEPlugin;
import com.nitrous.gwt.earth.client.api.KmlContainer;
import com.nitrous.gwt.earth.client.api.KmlFeature;
import com.nitrous.gwt.earth.client.api.KmlLink;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlNetworkLink;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.KmlObjectList;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlStyle;
import com.nitrous.gwt.earth.client.api.event.KmlLoadCallback;
import com.nitrous.gwt.earth.client.api.event.MouseClickListener;

public class VREstate implements EntryPoint, RequestCallback, KmlLoadCallback,
		I_Login {
	
	public static VREstate instance = null;
	private State _state; 

	/**
	 * @wbp.parser.entryPoint
	 */
	@Override
	public void onModuleLoad() {
		// Log.write(GWT.getHostPageBaseURL());
		// Log.write(GWT.getModuleBaseForStaticFiles());
		// Log.write(GWT.getModuleBaseURL());
		_state = State.Create(); 
		Options.Init();
//		LoginUser();
		
		instance = this;
		
		
		// my testing
		VREstateTree tree = new VREstateTree();
		tree.go(new PopupContainer());
		
		// my2 testing
//		DefaultPresenter Default = new DefaultPresenter(null);
//		Default.go(null);
//		VREstatePresenter vrestate = new VREstatePresenter(Default, new VREstateView());
//		vrestate.go(new PopupContainer());
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
		User.Login(this, "web", "web", User.UserRole.Visitor);
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
			url = Options.URL_VRT + "data/view?type=viewOrder&id="
					+ Options.getViewOrderId() + "&track=true&SID=" + User.SID;
		} else {
			url = Options.URL_VRT + "data/view?type=site&id="
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
			GEPlugin ge = GE.getPlugin();
			for (String structrure : Document.get().getStructures()) {
				KmlNetworkLink networkLink = ge.createNetworkLink("");
				//networkLink.setDescription("NetworkLink open to fetched content");
				//networkLink.setName("Open NetworkLink");
				networkLink.setFlyToView(false);
		 
				// create a Link object
				KmlLink link = ge.createLink("");
				link.setHref(Options.URL_MODELS + structrure);
		 
				// attach the Link to the NetworkLink
				networkLink.setLink(link);
		 
				// add the NetworkLink feature to Earth
				ge.getFeatures().appendChild(networkLink);
			}
			
			for (Site site : Document.get().getSites().values()) {
				if (site.getPOIUrl() != "")
					ge.fetchKml(site.getPOIUrl(), this);
			}
			boolean useSiteModel = true;
			for (Building bldng : Document.get().getBuildings().values()) {
				if (bldng.getDisplayModelUrl() != "") {
					ge.fetchKml(bldng.getDisplayModelUrl(),
							this);
					useSiteModel = false;
				}
				if (bldng.getOverlayUrl() != "")
					ge.fetchKml(bldng.getOverlayUrl(), this);
				if (bldng.getPOIUrl() != "")
					ge.fetchKml(bldng.getPOIUrl(), this);
			}

			Site theSite = (Site) Document.get().getSites().values().toArray()[0];
			if (useSiteModel)
				if (theSite.getDisplayModelUrl() != "")
					ge.fetchKml(theSite.getDisplayModelUrl(), this);

			_AbstractView.CreateAllGeoItems();

			switch (Options.ROLE) {
			case KIOSK: {
				_AbstractView.enableTimeout(true);
				int id = theSite.getId();
				SiteGeoItem geoItem = _AbstractView.getSiteGeoItem(id);
				firstView = new HelicopterView(geoItem);
				_AbstractView.ResetTimeOut();
				break;
			}
			case VISITOR: {
				_AbstractView.enableTimeout(false);
				int id = theSite.getId();
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
		if (feature != null) {
			GE.getPlugin()
					.getFeatures()
					.appendChild(correct_placemarks(remove_dublicates(feature)));
		}
	}

	private KmlObject remove_dublicates(KmlObject feature) {
		try {
			KmlContainer container = (KmlContainer) feature;

			// To be added
			KmlObjectList new_placemarks = container
					.getElementsByType("KmlPlacemark");

			// Already contained by GE
			KmlObjectList curr_placemarks = GE.getPlugin().getElementsByType(
					"KmlPlacemark");

			// new placemarks
			int i = 0;
			int j = 0;
			while (i < new_placemarks.getLength()) {
				j = i + 1;
				while (j < new_placemarks.getLength()) {
					KmlPlacemark first = (KmlPlacemark) new_placemarks.item(i);
					KmlPlacemark second = (KmlPlacemark) new_placemarks.item(j);
					if (first.getName().equals(second.getName())) {
						// container.getFeatures().removeChild(second);
						second.setVisibility(false);
						new_placemarks = container
								.getElementsByType("KmlPlacemark");
						i = 0;
						j = 0;
						break;
					}

					j++;
				}
				if (i == 0 && j == 0)
					break;
				i++;
			}

			// Log.write(container.getKml());
			// new vs old placemarks
			i = 0;
			j = 0;
			while (i < new_placemarks.getLength()) {
				while (j < curr_placemarks.getLength()) {
					KmlPlacemark first = (KmlPlacemark) new_placemarks.item(i);
					KmlPlacemark second = (KmlPlacemark) curr_placemarks
							.item(j);
					// Log.write("i: " + i + ", j: " + j);
					if (first.getName().equals(second.getName())) {
						// I don't know why it doesn't work...
						// container.getFeatures().removeChild(first);
						first.setVisibility(false);
						new_placemarks = container
								.getElementsByType("KmlPlacemark");
						i = 0;
						j = 0;
						break;
					}
					j++;
				}
				if (i == 0 && j == 0)
					break;
				i++;
			}
			return container;
		} catch (Exception e) {
			// TODO Auto-generated catch block
			return feature;
			// e.printStackTrace();
		}
	}

	private KmlObject correct_placemarks(KmlObject feature) {
		Log.write("Feature: " + ((KmlFeature)feature).getKml());
		Log.write("Feature type: " + feature.getType());
		if ("KmlDocument".equals(feature.getType())) {
			KmlContainer container = (KmlContainer) feature;
			Log.write("container: " + container.getKml());
			KmlObjectList placemarks = container
					.getElementsByType("KmlPlacemark");
			for (int index = 0; index < placemarks.getLength(); index++) {
				final KmlPlacemark placemark = (KmlPlacemark) placemarks
						.item(index);

				Element description = new HTML().getElement();
				description.setInnerHTML(placemark.getDescription());

				correct_empty_icons(description);
				correct_hrefs(description);
				String html = description.getInnerHTML();

				if (html.isEmpty())
					html += " ";

				placemark.setDescription(html);

				placemark.addMouseClickListener(new MouseClickListener() {

					@Override
					public void onClick(KmlMouseEvent event) {
						// TODO Auto-generated method stub
						event.preventDefault();
						String content = placemark.getDescription();
						GEHtmlStringBalloon balloon = GE.getPlugin()
								.createHtmlStringBalloon("");
						balloon.setFeature(placemark);
						balloon.setContentString(content);
						GE.getPlugin().setBalloon(balloon);
					}

					@Override
					public void onDoubleClick(KmlMouseEvent event) {
						// TODO Auto-generated method stub

					}

					@Override
					public void onMouseDown(KmlMouseEvent event) {
						// TODO Auto-generated method stub

					}

					@Override
					public void onMouseUp(KmlMouseEvent event) {
						// TODO Auto-generated method stub

					}
				});
				// placemark size
				KmlStyle style = placemark.getComputedStyle();
				style.getIconStyle().setScale(1.3f);
				placemark.setStyleSelector(style);

			}
			return container;
		} else
			return feature;
	}

	private void correct_empty_icons(Element container) {
		NodeList<Element> items = container.getElementsByTagName("img");
		for (int i = 0; i < items.getLength(); i++) {
			Element item = items.getItem(i);
			if ("none".equals(item.getStyle().getDisplay()))
				item.getParentElement().removeChild(item);
		}
	}

	private void correct_hrefs(Element container) {
		// Log.write("before: " + container.getInnerHTML());
		NodeList<Element> items = container.getElementsByTagName("*");
		int i = 0;
		while (i < items.getLength()) {
			Element item = items.getItem(i);
			// Log.write(item.getTagName());
			// if (item.getTagName().equalsIgnoreCase("SCRIPT")) {
			// item.getParentElement().removeChild(item);
			// items = container.getElementsByTagName("*");
			// i = 0;
			// } else
			if (!item.getAttribute("href").isEmpty()) {
				item.setAttribute("onclick", "return !window.open(this.href)");
				i++;
			} else
				i++;
		}

		// Log.write("after: " + container.getInnerHTML());
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