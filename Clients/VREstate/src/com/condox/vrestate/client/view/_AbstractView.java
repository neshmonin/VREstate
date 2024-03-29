package com.condox.vrestate.client.view;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Stack;
import com.condox.clientshared.abstractview.IGeoItem;
import com.condox.clientshared.abstractview.I_AbstractView;
import com.condox.clientshared.abstractview.I_Progress;
import com.condox.clientshared.abstractview.I_UpdatableView;
import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.document.Building;
import com.condox.clientshared.document.Document;
import com.condox.clientshared.document.I_VRObject;
import com.condox.clientshared.document.Site;
import com.condox.clientshared.document.Suite;
import com.condox.clientshared.document.ViewOrder;
import com.condox.clientshared.utils.StringFormatter;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.I_AbstractInteractor;
import com.condox.vrestate.client.view.Camera.Camera;
import com.condox.vrestate.client.view.GeoItems.BuildingGeoItem;
import com.condox.vrestate.client.view.GeoItems.SiteGeoItem;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem.GeoStatus;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.shared.HandlerRegistration;
import com.google.gwt.user.client.Timer;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;
import com.nitrous.gwt.earth.client.api.event.FrameEndListener;
import com.nitrous.gwt.earth.client.api.event.ViewChangeListener;

public abstract class _AbstractView implements I_AbstractView {

	private static Stack<I_AbstractView> views = new Stack<I_AbstractView>();
	public static ViewMessages i18n = (ViewMessages)GWT.create(ViewMessages.class);;

	protected I_AbstractInteractor _interactor = null;
	protected IGeoItem theGeoItem = null;
	public Camera _camera;
	protected double _transitionSpeed;
	protected double _regularSpeed;
	protected String _title;
	private static boolean m_timeoutTimerDisabled = false;
	protected static int TIMEOUTINTERVAL = 2 * 60 * 1000;
	static protected Timer m_timeoutTimer = new Timer() {

		@Override
		public void run() {
			_AbstractView.PopToTheBottom();
			_AbstractView.ResetTimeOut();
		}
	};

	// Timer to return to the first FullScreenView (Helicopter or Video) after a
	// timeout
	public static void ResetTimeOut() {
//		ScreenSaver.get().reset();
		m_timeoutTimer.cancel();
		if (!m_timeoutTimerDisabled) {
			if (Options.SERVER_MODE.equals(Options.MODE.TEST))
				m_timeoutTimer.schedule(30 * 1000);
			else
				m_timeoutTimer.schedule(TIMEOUTINTERVAL);
		}
	}
	
	protected boolean isViewChangedInProgress = false;

	@Override
	public void doViewChanged() {
		if (!isViewChangedInProgress) {
			isViewChangedInProgress = true;
			onViewChanged();
			isViewChangedInProgress = false;
		}
	}

	@SuppressWarnings("unused")
	private static HandlerRegistration frameend_listener = GE.getPlugin()
			.addFrameEndListener(new FrameEndListener() {

				@Override
				public void onFrameEnd() {
					if (views.isEmpty())
						return;

					_AbstractView currentView = (_AbstractView) views.peek();
				}
			});

	@SuppressWarnings("unused")
	private static HandlerRegistration view_listener = GE.getPlugin().getView()
			.addViewChangeListener(new ViewChangeListener() {

				@Override
				public void onViewChangeBegin() {
				}

				// As onViewChange fired, we redirect it to the onViewChanged
				// virtual function
				// of the current view.
				@Override
				public void onViewChange() {
					// if (views.isEmpty())
					// return;
					//
					// _AbstractView currentView = (_AbstractView) views.peek();
					// if(!currentView.isViewChangedInProgress())
					// {
					// currentView.setViewChangedInProgress(true);
					// currentView.onViewChanged();
					// currentView.setViewChangedInProgress(false);
					// }
				}

				// As onViewChangeEnd fired, we redirect it to the
				// onTransitionStopped
				// virtual function of the current view.
				@Override
				public void onViewChangeEnd() {
					if (views.isEmpty())
						return;

					I_AbstractView currentView = views.peek();
					if (currentView.isSetEnabledScheduled()) {
						currentView.setEnabled(true);
						currentView.onTransitionStopped();
						NextSelection();
					}
				}
			});

	/* ========================================================== */

	protected _AbstractView() {
		_regularSpeed = GE.getPlugin().getFlyToSpeedTeleport();
		_transitionSpeed = 0.5;
	}

	private static String getPrintableViews(String title) {
		String stackMsg = title + ": the views (";
		for (I_UpdatableView item : views)
			stackMsg += item.getClass().getName() + "->";
		return stackMsg + "<none>)";
	}

	@Override
	public void onDestroy() {
	}

//	@Override
//	public boolean isCameraMoved() {
//		return getCamera().isMoved();
//	}
	
	@Override
	public void ApplyCamera() {
		getCamera().Apply();
	}
	
	public static void Push(I_AbstractView newView) {
		newView.scheduleSetEnabled();
		newView.setupCamera(null);

		if (!views.isEmpty()) {
			I_AbstractView oldView = views.peek();
			oldView.setEnabled(false);
		}

		views.push(newView);

		Log.write(getPrintableViews("Push"));

		GE.getPlugin().getOptions().setFlyToSpeed(newView.getTransitionSpeed());
		newView.ApplyCamera();
		GE.getPlugin().getOptions().setFlyToSpeed(newView.getRegularSpeed());
	}

	public static I_AbstractView Pop() {
		I_AbstractView currView = views.peek();
		if (views.size() < 2)
			return currView;

		currView.setEnabled(false);
		currView.onDestroy();

		I_AbstractView poppedView = views.pop();
		I_AbstractView newView = views.peek();
		newView.setupCamera(poppedView);

		// newView.setEnabled(true);
		newView.scheduleSetEnabled();

		Log.write(getPrintableViews("Pop"));

		GE.getPlugin().getOptions().setFlyToSpeed(newView.getTransitionSpeed());
		newView.ApplyCamera();
		GE.getPlugin().getOptions().setFlyToSpeed(newView.getRegularSpeed());

		return newView;
	}

	public static void PopToTheBottom() {
		Filter.get().Reset();
		while (views.size() > 1) {
			I_AbstractView currView = views.peek();
			currView.setEnabled(false);
			currView.onDestroy();

			I_AbstractView poppedView = views.pop();
			I_AbstractView newView = views.peek();
			newView.setupCamera(poppedView);

			newView.setEnabled(true);

			Log.write(getPrintableViews("PopToTheBottom"));

			GE.getPlugin().getOptions()
					.setFlyToSpeed(newView.getTransitionSpeed());
			newView.ApplyCamera();
			GE.getPlugin().getOptions()
					.setFlyToSpeed(newView.getRegularSpeed());
		}
	}

	public static void Pop_Push(I_AbstractView newView) {
		I_AbstractView currView = views.peek();
		if (views.size() < 2)
			return;

		currView.setEnabled(false);
		currView.onDestroy();

		I_AbstractView poppedView = views.pop();

		newView.scheduleSetEnabled();
		newView.setupCamera(poppedView);

		views.push(newView);

		Log.write(getPrintableViews("Pop-Push"));

		GE.getPlugin().getOptions().setFlyToSpeed(newView.getTransitionSpeed());
		newView.ApplyCamera();
		GE.getPlugin().getOptions().setFlyToSpeed(newView.getRegularSpeed());

	}

	public static void Pop_Pop_Push(I_AbstractView newView) {
		I_AbstractView currView = views.peek();
		if (views.size() < 3)
			return;

		currView.setEnabled(false);
		currView.onDestroy();

		views.pop();
		I_AbstractView interimView = views.peek();
		interimView.onDestroy();
		I_AbstractView poppedView = views.pop();

		newView.scheduleSetEnabled();
		newView.setupCamera(poppedView);

		views.push(newView);

		Log.write(getPrintableViews("Pop-Pop-Push"));

		GE.getPlugin().getOptions().setFlyToSpeed(newView.getTransitionSpeed());
		newView.ApplyCamera();
		GE.getPlugin().getOptions().setFlyToSpeed(newView.getRegularSpeed());

	}

	public String getTitleText() {
		return theGeoItem.getCaption();
	}

	KmlScreenOverlay info_overlay = null;
	KmlIcon icon = null;

	public void setEnabled(boolean value) {
		if (info_overlay == null) {
			icon = GE.getPlugin().createIcon("");
			info_overlay = GE.getPlugin().createScreenOverlay("");
			info_overlay.setIcon(icon);
			info_overlay.getOverlayXY().set(0.5, KmlUnits.UNITS_FRACTION, 30,
					KmlUnits.UNITS_INSET_PIXELS);
			info_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
					KmlUnits.UNITS_FRACTION);
			GE.getPlugin().getFeatures().appendChild(info_overlay);
		}
		String href = Options.URL_VRT + "gen/txt?height=40&shadow=2&text="
				+ getTitleText() + "&txtClr=16777215&shdClr=0&frame=0";
		icon.setHref(href);
		info_overlay.setVisibility(value);
	};

	@Override
	public IGeoItem getGeoItem() {
		return theGeoItem;
	}

	@Override
	public double getTransitionSpeed() {
		return _transitionSpeed;
	}

	@Override
	public double getRegularSpeed() {
		return _regularSpeed;
	}

	public Camera getCamera() {
		return _camera;
	}

	@Override
	public abstract void onViewChanged();

	public static IGeoItem getCurrentGeoItem() {
		if (views.isEmpty())
			return null;

		I_AbstractView currentView = views.peek();
		return currentView.getGeoItem();
	}

	public static I_AbstractView getCurrentView() {
		if (views.isEmpty())
			return null;

		I_AbstractView currentView = views.peek();
		return currentView;
	}

	boolean setEnabledScheduled = false;

	public boolean isSetEnabledScheduled() {
		boolean isScheduled = setEnabledScheduled;
		setEnabledScheduled = false;
		return isScheduled;
	};

	public void scheduleSetEnabled() {
		setEnabledScheduled = true;
	};

	private static List<IGeoItem> SelectionsQueue = new LinkedList<IGeoItem>();

	public static void AddSelection(IGeoItem geoItemToSelect) {
		SelectionsQueue.add(geoItemToSelect);
	}

	public static void NextSelection() {
		if (SelectionsQueue.size() <= 0)
			return;

		IGeoItem geoItem = SelectionsQueue.get(0);
		if (geoItem == null)
			return;

		I_AbstractView currentView = views.peek();
		currentView.Select(geoItem.getType(), geoItem.getId());
		SelectionsQueue.remove(0);
	}

	/*-----------------------------------------------------------------------*/
	private static Map<Integer, SiteGeoItem> siteGeoItems = new HashMap<Integer, SiteGeoItem>();
	private static Map<Integer, BuildingGeoItem> buildingGeoItems = new HashMap<Integer, BuildingGeoItem>();
	private static Map<Integer, SuiteGeoItem> suiteGeoItems = new HashMap<Integer, SuiteGeoItem>();
	public static KmlObject wiresKmlObject = null;

	// This static function creates all the GeoItems for the given
	// Site and all obtained hierarchy of elements (BuildingGeo-s, SuiteGeo-s,
	// etc.)
	public static void CreateAllGeoItems() {
		I_Progress progressBar = new ProgressBar();
		progressBar.SetupProgress(I_Progress.ProgressType.Processing);
		for (Site site : Document.get().getSites().values()) {
			SiteGeoItem siteGeo = new SiteGeoItem(site);
			siteGeoItems.put(site.getId(), siteGeo);
		}

		for (Building building : Document.get().getBuildings().values()) {
			BuildingGeoItem buildingGeo = new BuildingGeoItem(building);
			buildingGeoItems.put(building.getId(), buildingGeo);
		}

		int count = 0;
		int howMany = Document.get().getSuites().size();
		String kmlGeoItems = "";
		for (Suite suite : Document.get().getSuites().values()) {
			kmlGeoItems += addSuiteGeoItem(suite, false);

			count++;
			progressBar.UpdateProgress(count * 100.0 / howMany);
		}

		wiresKmlObject = GE_addKml(kmlGeoItems);

		if (Document.targetViewOrder != null) {
			if (Document.targetViewOrder.getProductType() == ViewOrder.ProductType.PrivateListing ||
				Document.targetViewOrder.getProductType() == ViewOrder.ProductType.PublicListing) {
				Suite targetSuite = (Suite) Document.targetViewOrder.getTargetObject();
				suiteGeoItems.get(targetSuite.getId()).setGeoStatus(GeoStatus.Selected);
				
				String vTourUrl =  Document.targetViewOrder.getVTourUrl();
				if (vTourUrl != null)
					targetSuite.setVTourUrl(vTourUrl);
			}
		}
		
		progressBar.CleanupProgress();
	}
	
	private static KmlObject GE_addKml(String kmlGeoItems) {
		String kmlWires = StringFormatter.format(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
				"<kml xmlns=\"http://www.opengis.net/kml/2.2\">" +
					"<Document>" +
						"{0}" +
					"</Document>" +
				"</kml>",
				kmlGeoItems);

		KmlObject wires = GE.getPlugin().parseKml(kmlWires);
		GE.getPlugin().getFeatures().appendChild(wires);
		
		return wires;
	}
	
	@Override
	public void UpdateChangedGeoItems(Map<Integer, I_VRObject> changedVRObjects) {
		if (changedVRObjects == null)
			return;
		
		for (I_VRObject vrObject : changedVRObjects.values()) {
			int id = vrObject.getId();
			switch (vrObject.getType()) {
			case None:
				break;
			case Site:
				break;
			case Building:
				break;
			case Suite:
				Suite suite = (Suite)vrObject;
				if (suiteGeoItems.containsKey(id)) {
					// updating the existing SuiteGeoItem
					SuiteGeoItem suiteGeo = getSuiteGeoItem(id);
					suiteGeo.Init(suite);
					suiteGeo.ShowIfFilteredIn();
				}
				else {
					// creating new SuiteGeoItem
					suite.CalcLineCoords();
					GE_addKml(_AbstractView.addSuiteGeoItem(suite, true));
				}
				break;
			}
		}
	}

	public static String addSuiteGeoItem(Suite suite, boolean redraw) {
		SuiteGeoItem suiteGeo = new SuiteGeoItem(suite);
		if (suiteGeoItems.containsKey(suite.getId()))
			Log.write("_AbstractView.addSiteGeoItem -> duplicate suiteGeoItem: suiteId="
					+suite.getId()+
					" suiteName="+suite.getName()+
					" suiteType="+suite.getSuiteType().toString());

		suiteGeoItems.put(suite.getId(), suiteGeo);
		if (redraw)
			suiteGeo.ShowIfFilteredIn();
		
		return suiteGeo.kmlStyle + suiteGeo.kmlPlacemark;
	}

	public static SiteGeoItem getSiteGeoItem(int id) {
		return siteGeoItems.get(id);
	}

	public static BuildingGeoItem getBuildingGeoItem(int id) {
		return buildingGeoItems.get(id);
	}

	public static SuiteGeoItem getSuiteGeoItem(int id) {
		return suiteGeoItems.get(id);
	}

	public static Map<Integer, SiteGeoItem> getSiteGeoItems() {
		return siteGeoItems;
	}

	public static Map<Integer, BuildingGeoItem> getBuildingGeoItems() {
		return buildingGeoItems;
	}

	public static Map<Integer, SuiteGeoItem> getSuiteGeoItems() {
		return suiteGeoItems;
	}

	public static void enableTimeout(boolean enable) {
		_AbstractView.m_timeoutTimerDisabled = !enable;
	}

	public static boolean isTimeoutEnabled() {
		return !m_timeoutTimerDisabled;
	}

	@Override
	public double getStartingRange() {
		return theGeoItem.getPosition().getRange();
	}
	
}
