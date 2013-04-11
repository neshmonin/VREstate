package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Building;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Site;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteType;
import com.condox.vrestate.client.document.ViewOrder.ProductType;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.SuiteInteractor;
import com.condox.vrestate.client.view.Camera.Camera;
import com.condox.vrestate.client.view.GeoItems.BuildingGeoItem;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.http.client.URL;
import com.google.gwt.i18n.client.NumberFormat;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.Window;
import com.nitrous.gwt.earth.client.api.GEHtmlDivBalloon;
import com.nitrous.gwt.earth.client.api.GEVisibility;

public class SuiteView extends _GEView {

	private static boolean isMoreInfoVisible = false;

	SuiteGeoItem suiteGeo = null;
	private Suite currSuite = null;
	private SuiteType currSuiteType = null;
	private Building currBuilding = null;
	private Site currSite = null;

	public SuiteView(IGeoItem geoItem) {
		super(geoItem);
		suiteGeo = (SuiteGeoItem) geoItem;
		currSuite = (suiteGeo != null) ? suiteGeo.suite : null;
		currSuiteType = (currSuite != null) ? currSuite.getSuiteType() : null;
		currBuilding = (currSuite != null) ? currSuite.getParent() : null;
		currSite = (currBuilding != null) ? currBuilding.getParent() : null;
	}

	@Override
	public void setEnabled(boolean enabling) {
		super.setEnabled(enabling);
		if (Document.targetViewOrder == null)
			Filter.get().setVisible(enabling);
		else if (Document.targetViewOrder.getProductType() == ProductType.PublicListing
				|| Document.targetViewOrder.getProductType() == ProductType.Building3DLayout)
			Filter.get().setVisible(enabling);

		if (enabling) {
			GE.getPlugin().getNavigationControl()
					.setVisibility(GEVisibility.VISIBILITY_HIDE);
			if (_interactor == null)
				_interactor = new SuiteInteractor(this);
			_interactor.setEnabled(true);
		} else {
			_interactor.setEnabled(false);
			_interactor = null;
			HideBalloon();
		}
	}

	@Override
	public void Select(String type, int id) {
		int parentId = theGeoItem.getParent_id();

		if (type == null) {
			// they clicked outside of anything - want to exit
			_AbstractView.Pop();
			return;
		}

		int suiteId = theGeoItem.getId();
		if (id == suiteId)
			return; // they clicked the same suite

		if (type.equals("building")) {
			// they clicked placemark of a building
			BuildingGeoItem buildingGeoItem = _AbstractView
					.getBuildingGeoItem(parentId);
			buildingGeoItem.onSelectionChanged(false);
			BuildingGeoItem newBuildingGeoItem = _AbstractView
					.getBuildingGeoItem(id);
			newBuildingGeoItem.onSelectionChanged(true);
			_AbstractView.Pop_Pop_Push(new BuildingView(newBuildingGeoItem));
			return;
		}

		if (type.equals("suite")) {
			// they clicked placemark of a different suite
			// 1. check if this suite is still within tha same building?
			SuiteGeoItem newSuiteGeo = _AbstractView.getSuiteGeoItem(id);
			int newParentId = newSuiteGeo.getParent_id();
			if (newParentId == parentId) {
				SuiteView suiteView = new SuiteView(newSuiteGeo);
				_AbstractView.Pop_Push(suiteView);
			} else {
				Select("building", newParentId);
				_AbstractView.AddSelection(newSuiteGeo);
			}
		}
	}

	GEHtmlDivBalloon balloon = GE.getPlugin().createHtmlDivBalloon("");

	public void ShowBalloon() {
		if (isMoreInfoVisible)
			return;
		isMoreInfoVisible = true;

		setCallbacks();
		String templateUrl = null;
		switch (Options.ROLE) {
		case KIOSK:
			if (currSite != null)
				templateUrl = currSite.getBubbleKioskTemplateUrl();
			if (templateUrl == null || templateUrl.isEmpty())
				if (currBuilding != null)
					templateUrl = currBuilding.getBubbleKioskTemplateUrl();
			break;
		default:
			if (currSite != null)
				templateUrl = currSite.getBubbleWebTemplateUrl();
			if (templateUrl == null || templateUrl.isEmpty())
				if (currBuilding != null)
					templateUrl = currBuilding.getBubbleWebTemplateUrl();
			break;
		}

		if (templateUrl == null || templateUrl.isEmpty()
				|| Options.isViewOrder())
			templateUrl = "templates/default.html";
		// templateUrl =
		// "https://models.3dcondox.com/vre/templates/default.html";

		// TODO remove
		// Window.alert(url);

		String content = "<iframe id=\"VRT_SuiteInfo\" src=\"";
		content += templateUrl;
		content += "#";
		content += URL.encode(getSuiteInfo());
		content += "\" frameborder=0></iframe>";
		balloon.setCloseButtonEnabled(false);
		balloon.setContentDiv(content);
		balloon.setFeature(suiteGeo.getExtendedDataLabel());
		GE.getPlugin().setBalloon(balloon);
	}

	public void HideBalloon() {
		if (!isMoreInfoVisible)
			return;
		isMoreInfoVisible = false;
		// ((Element) balloon.getContentDiv()).removeFromParent();
		GE.getPlugin().setBalloon(null);
	}

	private String getSuiteInfo() {
		Suite suite = suiteGeo.suite;
		SuiteType type = suite.getSuiteType();
		JSONObject json = new JSONObject();
		// Suite
		json.put("VRT_name", new JSONString(suite.getName()));
		json.put("VRT_floor", new JSONString(suite.getFloor_name()));
		json.put("VRT_bedrooms", new JSONString(type.getRoomsStr()));
		json.put("VRT_bathrooms", new JSONNumber(type.getBathrooms()));
		if (type.getArea() > 0)
			json.put("VRT_area", new JSONNumber(type.getArea()));
		if (!type.getAreaUm().isEmpty())
			json.put("VRT_areaUm", new JSONString(type.getAreaUm()));
		NumberFormat fmt = NumberFormat.getDecimalFormat();
		if (suite.getPrice() > 0)
			json.put("VRT_price",
					new JSONString("$" + fmt.format(suite.getPrice())));
		if (type.getBalconies() > 0)
			json.put("VRT_balconies", new JSONNumber(type.getBalconies()));
		else
			json.put("VRT_balconies", new JSONString("none"));
		// Link "More info..."
		String moreInfo = suite.getInfoUrl();
		if (moreInfo != null && moreInfo.length() > 0)
			json.put("VRT_moreInfo", new JSONString(moreInfo));
		// Link "Virtual tour"
		String vTourUrl = suiteGeo.suite.getVTourUrl();
		if (vTourUrl != null && vTourUrl.length() > 0)
			json.put("VRT_virtualTour", new JSONString(vTourUrl));
		// Floorplan button
		String floorplanUrl = type.getFloorPlanUrl();
		if (floorplanUrl != null && floorplanUrl.length() > 0)
			json.put("VRT_floorplanUrl", new JSONString(floorplanUrl));
		// PanoramicView button
		json.put("VRT_panoramicViewUrl", new JSONString("#"));

		return json.toString();
	};

	// private String getSuiteInfo() {
	// Suite suite = suiteGeo.suite;
	// SuiteType type = suite.getSuiteType();
	// JSONObject obj = new JSONObject();
	// // Suite's attributes
	// obj.put("name", new JSONString(suite.getName()));
	// obj.put("floor", new JSONString(suite.getFloor_name()));
	// obj.put("bedrooms", new JSONString(type.getRoomsStr()));
	// obj.put("bathrooms", new JSONNumber(type.getBathrooms()));
	// if (type.getArea() > 0)
	// obj.put("area", new JSONNumber(type.getArea()));
	// if (!type.getAreaUm().isEmpty())
	// obj.put("areaUm", new JSONString(type.getAreaUm()));
	// NumberFormat fmt = NumberFormat.getDecimalFormat();
	// if (suite.getPrice() > 0)
	// obj.put("price", new JSONString("$" + fmt.format(suite.getPrice())));
	// if (type.getBalconies() > 0)
	// obj.put("balconies", new JSONNumber(type.getBalconies()));
	// else
	// obj.put("balconies", new JSONString("none"));
	// // Link "More info..."
	// String moreInfo = suite.getInfoUrl();
	// if (moreInfo != null && moreInfo.length() > 0)
	// obj.put("moreInfo", new JSONString(moreInfo));
	// // Link "Virtual tour"
	// String vTourUrl = suiteGeo.suite.getVTourUrl();
	// if (vTourUrl != null && vTourUrl.length() > 0)
	// obj.put("virtualTour", new JSONString(vTourUrl));
	// // Floorplan button
	// String floorplanUrl = type.getFloorPlanUrl();
	// if (floorplanUrl != null && floorplanUrl.length() > 0)
	// obj.put("floorplanUrl", new JSONString(floorplanUrl));
	// // PanoramicView button
	// obj.put("panoramicViewUrl", new JSONString("#"));
	//
	// return obj.toString();
	// };

	private native void setCallbacks() /*-{
		var view = this;

		$wnd.VRT_setBalloonSize = function(width, height) {
			var frame = $doc.getElementById('VRT_SuiteInfo');
			if (typeof frame != 'undefined') {
				frame.style.width = width + 'px';
				frame.style.height = height + 'px';
			}
			return false;
		};

		$wnd.VRT_showVirtualTour = function() {
			view.@com.condox.vrestate.client.view.SuiteView::VRT_showVirtualTour()();
		}
		$wnd.VRT_showMoreInfo = function() {
			view.@com.condox.vrestate.client.view.SuiteView::VRT_showMoreInfo()();
		}
		$wnd.VRT_showPanoramicView = function() {
			view.@com.condox.vrestate.client.view.SuiteView::ShowPanoramicView()();
		}
		$wnd.VRT_showFloorplan = function() {
			view.@com.condox.vrestate.client.view.SuiteView::ShowFloorPlan()();
		}
		return false;
	}-*/;

	private void VRT_showVirtualTour() {
		String vTourUrl = suiteGeo.suite.getVTourUrl();
		if (vTourUrl != null && vTourUrl.length() > 0)
			Window.open(vTourUrl, "_blank", null);
	}

	private void VRT_showMoreInfo() {
		String moreInfo = suiteGeo.suite.getInfoUrl();
		if (moreInfo != null && moreInfo.length() > 0)
			Window.open(moreInfo, "_blank", null);
		else
			Window.open(
					"http://02ea89a.netsolhost.com/beyondsea/beachcomber.html",
					"_blank", null);
	}

	private void ShowPanoramicView() {
		IGeoItem suiteGeo = _AbstractView.getSuiteGeoItem(theGeoItem.getId());
		_AbstractView.Push(new PanoramicView(suiteGeo));
	}

	private void ShowFloorPlan() {
		String floorPlanUrl = suiteGeo.suite.getSuiteType().getFloorPlanUrl();
		if (floorPlanUrl != null)
			Window.open(floorPlanUrl, "_blank", null);
	}

	@Override
	public void onViewChanged() {
	}

	@Override
	public void setupCamera(I_AbstractView poppedView) {
		setupStandardLookAtCamera(poppedView);
		_camera.attributes.Heading_d = Camera.NormalizeHeading_d(theGeoItem
				.getPosition().getHeading() + 180);
	}

	@Override
	public void onTransitionStopped() {
		onHeadingChanged();
		ShowBalloon();
	}

	public boolean selectNextSuite() {
		IGeoItem suiteGeo = Filter.get().getNextGeoItem();
		if (suiteGeo != null) {
			Select(suiteGeo.getType(), suiteGeo.getId());
			return true;
		}
		return false;
	}

	public boolean selectPrevSuite() {
		IGeoItem suiteGeo = Filter.get().getPrevGeoItem();
		if (suiteGeo != null) {
			Select(suiteGeo.getType(), suiteGeo.getId());
			return true;
		}
		return false;
	}

	public void ThisSuiteIsFilteredOut() {
		if (!selectNextSuite())
			_AbstractView.Pop();
	}
}
