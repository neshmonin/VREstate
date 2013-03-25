package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Document;
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
import com.google.gwt.core.client.JavaScriptObject;
import com.google.gwt.dom.client.Element;
import com.google.gwt.dom.client.Style.Visibility;
import com.google.gwt.http.client.RequestException;
import com.google.gwt.i18n.client.NumberFormat;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.user.client.DOM;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.RootPanel;
import com.nitrous.gwt.earth.client.api.GEHtmlDivBalloon;
import com.nitrous.gwt.earth.client.api.GEVisibility;

public class SuiteView extends _GEView {

	private static boolean isMoreInfoVisible = false;

	SuiteGeoItem suiteGeo = null;

	public SuiteView(IGeoItem geoItem) {
		super(geoItem);

		suiteGeo = (SuiteGeoItem) geoItem;
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

	// @Override
	// protected void onPlacemarkClick(String snippet) {
	// JSONObject obj = JSONParser.parseLenient(snippet).isObject();
	// String type = obj.get("type").isString().stringValue();
	// int id = Integer.valueOf((int) obj.get("id").isNumber().doubleValue());
	// if (type.equals("BUILDING")) {
	// Building building = Building.get(id);
	// if (Viewable.isSelected(building))
	// Site.get(building.getParent_id()).Select();
	// else
	// BuildingView.Create(id);
	// }
	// else if (type.equals("SUITE")) {
	// Suite suite = Suite.get(id);
	// SuiteView.Create(id);
	// // if (Viewable.isSelected(suite))
	// // Site.get(suite.getParent_id()).Select();
	// // else
	// // BuildingView.Create(id);
	// }
	//
	// super.onPlacemarkClick(snippet);
	// }

	private native void F() /*-{
		var view = this;
		$wnd.showPanoramicView = function() {
			view.@com.condox.vrestate.client.view.SuiteView::ShowPanoramicView()();
			return false;
		};
		$wnd.showFloorPlan = function() {
			view.@com.condox.vrestate.client.view.SuiteView::ShowFloorPlan()();
			return false;
		};
		$wnd.showMoreInfo = function() {
//			view.@com.condox.vrestate.client.view.SuiteView::ShowMoreInfo()();
			if (typeof (parent.parent.parent.onMoreInfo) == "function")
				parent.parent.parent.onMoreInfo();
			else
				view.@com.condox.vrestate.client.view.SuiteView::ShowFloorPlan()();
//			parent.parent.parent.HideVR();
//			parent.parent.parent.ShowElement("viewer");
			return false;
		};
		//		$wnd.A();
		//			suite.@com.condox.vrestate.client.view.SuiteView::A()();
		//		var show_more = function() {
		//			suite.@com.condox.vrestate.client.view.SuiteView::ShowMore()();
		//		}
	}-*/;

	public void ShowBalloon() {
		F();
		if (isMoreInfoVisible)
			return;
		isMoreInfoVisible = true;

		GEHtmlDivBalloon balloon = GE.getPlugin().createHtmlDivBalloon("");
		balloon.setCloseButtonEnabled(false);

		Options.SUITE_INFO.getStyle().setVisibility(Visibility.VISIBLE);

		com.google.gwt.dom.client.FrameElement frameElement = (com.google.gwt.dom.client.FrameElement) Options.SUITE_INFO;
		com.google.gwt.dom.client.Document doc = frameElement
				.getContentDocument();

		SuiteType type = suiteGeo.suite.getSuiteType();
		// NAME
		Element name = doc.getElementById("SuiteName");
		if (name != null)
			if (suiteGeo.getName() != null && !suiteGeo.getName().isEmpty())
				name.setInnerText(suiteGeo.getName());
		// TYPE_NAME
		Element type_name = doc.getElementById("SuiteTypeName");
		if (type_name != null)
			if (type.getName() != null && !type.getName().isEmpty())
				type_name.setInnerText(type.getName());
		// BEDROOMS
		Element bedrooms = doc.getElementById("SuiteBedroomsCount");
		if (bedrooms != null)
			if (type.getBedrooms() > 0)
				bedrooms.setInnerText("" + type.getBedrooms());
		// BATHROOMS
		Element bathrooms = doc.getElementById("SuiteBathroomsCount");
		if (bathrooms != null)
			if (type.getBathrooms() > 0)
				bathrooms.setInnerText("" + type.getBathrooms());
		// AREA
		Element area = doc.getElementById("SuiteArea");
		if (area != null)
			if (type.getArea() > 0)
				area.setInnerText("" + type.getArea());
		// AREA_MEAS
		Element area_meas = doc.getElementById("SuiteAreaMeas");
		if (area_meas != null)
			if (type.getAreaUm() != null && !type.getAreaUm().isEmpty())
				area_meas.setInnerText(type.getAreaUm());
		// FLOORNAME
		Element floorname = doc.getElementById("SuiteFloorName");
		if (floorname != null)
			if (suiteGeo.getFloor_name() != null
					&& !suiteGeo.getFloor_name().isEmpty())
				floorname.setInnerText(suiteGeo.getFloor_name());
		// BALCONIES
		Element balconies = doc.getElementById("SuiteBalconyTerrace");
		if (balconies != null)
			if (type.getBalconies() > 0)
				balconies.setInnerText("Yes");
			else
				balconies.setInnerText("No");
		// PRICE
		Element price = doc.getElementById("SuitePrice");
		if (price != null)
			if (suiteGeo.getPrice() > 0) {
				NumberFormat fmt = NumberFormat.getDecimalFormat();
				price.setInnerText("$" + fmt.format(suiteGeo.getPrice()));
			}
		// FLOORPLAN
		Element floorplan = doc.getElementById("SuiteFloorPlanImg");
		if (floorplan != null) {
			String url = suiteGeo.suite.getSuiteType().getFloorPlanUrl();
			if (url != null && !url.isEmpty())
				floorplan.setAttribute("src", url);
		}

		// // TODO For debug purposes:
		// price.setInnerText("$" + (int) (Math.random() * 100000));
		// bedrooms.setInnerText("" + (int) (Math.random() * 10));
		// bathrooms.setInnerText("" + (int) (Math.random() * 10));

		Element panoramic = doc.getElementById("onShowPanoramicView");
		panoramic.setAttribute("onclick", "showPanoramicView();");
		
//		floorplan = doc.getElementById("onShowFloorPlan");
//		floorplan.setAttribute("onclick", "showFloorPlan();");
		
		Element moreinfo = doc.getElementById("onShowMoreInfo");
		moreinfo.setAttribute("onclick", "showMoreInfo();");

		balloon.setContentDiv(doc.getBody().getInnerHTML());
		balloon.setFeature(suiteGeo.getExtendedDataLabel());
		GE.getPlugin().setBalloon(balloon);
	};

	public void HideBalloon() {
		if (!isMoreInfoVisible)
			return;
		isMoreInfoVisible = false;
//		GEHtmlDivBalloon balloon = (GEHtmlDivBalloon) GE.getPlugin()
//				.getBalloon();
//		removeElement(((Element) balloon.getContentDiv()));
		GE.getPlugin().setBalloon(null);
	}

	private String getJsonParams() {
		JSONObject obj = new JSONObject();
		if (suiteGeo.suite.getPrice() > 0)
			obj.put("price", new JSONString("$" + suiteGeo.suite.getPrice()));
		obj.put("suitName", new JSONString(suiteGeo.suite.getName()));
		obj.put("Floor", new JSONString(suiteGeo.suite.getFloor_name()));

		SuiteType type = suiteGeo.suite.getSuiteType();

		obj.put("bedrooms", new JSONString(type.getRoomsStr()));
		if (type.getBalconies() > 0)
			obj.put("balcony", new JSONNumber(type.getBalconies()));
		else
			obj.put("balcony", new JSONString("none"));

		String vTourUrl = suiteGeo.suite.getVTourUrl();
		if (vTourUrl != null && vTourUrl.length() > 0)
			obj.put("virtualTour", new JSONString(vTourUrl));

		String floorPlanUrl = type.getFloorPlanUrl();
		if (floorPlanUrl != null && floorPlanUrl.length() > 0)
			obj.put("more", new JSONString(floorPlanUrl));

		obj.put("panoramicViewURL", new JSONString(""));

		if (type.getArea() > 0)
			obj.put("area", new JSONNumber(type.getArea()));

		String infoUrl = suiteGeo.suite.getInfoUrl();
		if (infoUrl != null && infoUrl.length() > 0)
			obj.put("moreInfo", new JSONString(infoUrl));
		else if (Options.DEBUG_MODE)
			obj.put("moreInfo", new JSONString(
					"http://02ea89a.netsolhost.com/beyondsea/beachcomber.html"));

		// obj.put("mail", new JSONString("MailUrl"));
		// obj.put("phone", new JSONString("123456789"));
		return obj.toString();
	};

	private JavaScriptObject getParams() {
		JSONObject obj = new JSONObject();
		obj.put("price", new JSONString("$" + suiteGeo.suite.getPrice()));
		return obj.getJavaScriptObject();
	}

	// private void addParams(Element div, JavaScriptObject params) {
	// // alert(div.innerHTML);
	// // $doc.getElementsByTagName('body')[0].appendChild(div);
	// // div.addParams(params);
	// if (suiteGeo.suite.getPrice() > 0) {
	// Element price = DOM.createElement("<span>");
	// price.setId("SuitePrice");
	// div.appendChild(price);
	// };

	// public native void addElement(Element element, String json) /*-{
	// //********************************
	// $doc.getElementsByTagName('body')[0].appendChild(element);
	// element.style.left = "10px";
	//
	//
	//
	// var scripts = element.getElementsByTagName("script");
	// for (i = 0; i < scripts.length; i++)
	// $wnd.eval(scripts[i].innerHTML);
	//
	// // $wnd.alert($doc.getElementsByTagName('head')[0].innerHTML);
	// // var style = $doc.getElementsByTagName('style')[0];
	// var styles = element.getElementsByTagName("style");
	// for (i = 0; i < styles.length; i++)
	// $doc.getElementsByTagName('head')[0].appendChild(styles[i]);
	//
	// // $wnd.alert($doc.innerHTML);
	//
	//
	//
	//
	//
	// // for (i = 0; i < scripts.length; i++) {
	// // if src, eval it, otherwise eval the body
	// // if (scripts[i].hasAttribute("src")) {
	// // if (scripts[i]['src'] != null) {
	// // var src = scripts[i].getAttribute("src");
	// // var script = $doc.createElement('script');
	// // script.setAttribute("src", src);
	// // $doc.getElementsByTagName('body')[0].appendChild(script);
	// // } else {
	// // $wnd.eval(scripts[i].innerHTML);
	// // }
	// // }
	//
	// //********************************
	// var suite = this;
	// var show_panoramic_view = function() {
	// suite.@com.condox.vrestate.client.view.SuiteView::ShowPanoramicView()();
	// }
	// var show_more = function() {
	// suite.@com.condox.vrestate.client.view.SuiteView::ShowMore()();
	// }
	//
	//
	// $doc.project(element, json, show_panoramic_view, show_more);
	// }-*/;

	public native void removeElement(Element element) /*-{
		element.parentNode.removeChild(element);
	}-*/;

	private void ShowPanoramicView() {
		IGeoItem suiteGeo = _AbstractView.getSuiteGeoItem(theGeoItem.getId());
		_AbstractView.Push(new PanoramicView(suiteGeo));
	}

//	String floorPlanHTML = null;

	private void ShowFloorPlan() throws RequestException {
		SuiteType type = suiteGeo.suite.getSuiteType();
		String floorPlanUrl = type.getFloorPlanUrl();
		if (floorPlanUrl != null)
			Window.open(floorPlanUrl, "_blank", null);
	}
	
	private void ShowMoreInfo() throws RequestException {
	/*	Suite suite = suiteGeo.suite;
		String moreInfoUrl = suite.getInfoUrl();
		if (moreInfoUrl != null)
			Window.open(moreInfoUrl, "_blank", null);*/
//		frame.getStyle().setZIndex(1000);
//		frame.setAttribute("src", "http://www.google.com");
//		RootPanel.getBodyElement().appendChild(frame);
	}
	
	private native void open(String html) /*-{
		var wnd = window.open("", "_blank", "");
		wnd.document.write(html);
	}-*/;

	public void Update(double speed) {
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
