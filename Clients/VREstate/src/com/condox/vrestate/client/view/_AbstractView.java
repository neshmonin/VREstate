package com.condox.vrestate.client.view;

import java.util.Collection;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Stack;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Building;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Site;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.I_AbstractInteractor;
import com.condox.vrestate.client.view.Camera.Camera;
import com.condox.vrestate.client.view.GeoItems.BuildingGeoItem;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;
import com.condox.vrestate.client.view.GeoItems.SiteGeoItem;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.event.shared.HandlerRegistration;
import com.google.gwt.user.client.Timer;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;
import com.nitrous.gwt.earth.client.api.event.FrameEndListener;
import com.nitrous.gwt.earth.client.api.event.ViewChangeListener;

public abstract class _AbstractView implements I_AbstractView {

	private static Stack<I_AbstractView> views = new Stack<I_AbstractView>();

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
			if (Options.DEBUG_MODE)
				m_timeoutTimer.schedule(2 * 60 * 1000);
			else
				m_timeoutTimer.schedule(TIMEOUTINTERVAL);
	}
	}
	
//	public static void onTimerReset() {
//		_AbstractView.ResetTimeOut();
//		// Window.alert("VR:onTimerReset");
//	}
//
//	public static void onTimerTimeout() {
//		_AbstractView.PopToTheBottom();
//		// Window.alert("VR:onTimerTimeout");
//	}

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
		newView.getCamera().Apply();
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
		newView.getCamera().Apply();
		GE.getPlugin().getOptions().setFlyToSpeed(newView.getRegularSpeed());

		return newView;
	}

	public static void PopToTheBottom() {
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
			newView.getCamera().Apply();
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
		newView.getCamera().Apply();
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
		newView.getCamera().Apply();
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
		String href = Options.HOME_URL + "gen/txt?height=40&shadow=2&text="
				+ getTitleText() + "&txtClr=16777215&shdClr=0&frame=0";
		icon.setHref(href);
		info_overlay.setVisibility(value);
	};

	public static void ApplyFilter() {
		Document.progressBar = new ProgressBar();
		Document.progressBar.Update(ProgressBar.ProgressLabel.Processing);

		int howMany = getSuiteGeoItems().size();
		int count = 0;
		for (SuiteGeoItem suiteGeo : getSuiteGeoItems()) {
			Document.progressBar.Update(count * 100 / howMany);
			suiteGeo.Redraw();
			count++;
		}
		Document.progressBar.Cleanup();
		Document.progressBar = null;
	}

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

	@Override
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

	protected void setupStandardLookAtCamera(I_AbstractView poppedView) {
		if (_camera != null) {
			_camera.attributes.SetLonLatAlt(theGeoItem);
			if (poppedView != null) {
				Camera poppedCamera = poppedView.getCamera();
				if (poppedCamera.CameraType == _camera.CameraType)
					_camera.attributes.Heading_d = Camera
							.NormalizeHeading_d(poppedCamera.attributes.Heading_d);
			}
		} else {
			I_AbstractView curView = _AbstractView.getCurrentView();
			if (curView != null) {
				_camera = new Camera(curView.getCamera());
				_camera.attributes.SetLonLatAlt(theGeoItem);
				_camera.attributes.Tilt_d = theGeoItem.getPosition().getTilt();
				_camera.attributes.Range_m = getStartingRange();
			} else {
				_camera = new Camera(Camera.Type.LookAt,
						theGeoItem.getPosition().getHeading(),
						theGeoItem.getPosition().getTilt(),
						0,
						theGeoItem.getPosition().getLatitude(),
						theGeoItem.getPosition().getLongitude(),
						theGeoItem.getPosition().getAltitude(), 
						getStartingRange());
			}
		}
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

	// This static function creates all the GeoItems for the given
	// Site and all obtained hierarchy of elements (BuildingGeo-s, SuiteGeo-s,
	// etc.)
	public static void CreateAllGeoItems() {
		Document.progressBar.Update(ProgressBar.ProgressLabel.Processing);
		for (Site site : Document.get().getSites()) {
			SiteGeoItem siteGeo = new SiteGeoItem(site);
			siteGeoItems.put(site.getId(), siteGeo);
		}

		for (Building building : Document.get().getBuildings()) {
			BuildingGeoItem buildingGeo = new BuildingGeoItem(building);
			buildingGeoItems.put(building.getId(), buildingGeo);
		}

		int count = 0;
		int howMany = Document.get().getSuites().size();
		for (Suite suite : Document.get().getSuites()) {
			addSiteGeoItem(suite, false);

			count++;
			Document.progressBar.Update(count * 100.0 / howMany);
		}

		Document.progressBar.Cleanup();
	}

	public static void addSiteGeoItem(Suite suite, boolean redraw) {
		SuiteGeoItem suiteGeo = new SuiteGeoItem(suite);
		suiteGeoItems.put(suite.getId(), suiteGeo);
		if (redraw)
			suiteGeo.Redraw();
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

	public static Collection<SiteGeoItem> getSiteGeoItems() {
		return siteGeoItems.values();
	}

	public static Collection<BuildingGeoItem> getBuildingGeoItems() {
		return buildingGeoItems.values();
	}

	public static Collection<SuiteGeoItem> getSuiteGeoItems() {
		return suiteGeoItems.values();
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
