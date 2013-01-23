package com.condox.vrestate.client.view;

import java.util.Collection;
import java.util.HashMap;
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
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;
import com.nitrous.gwt.earth.client.api.event.ViewChangeListener;

public abstract class _AbstractView implements I_AbstractView {

	private static Stack<I_AbstractView> views = new Stack<I_AbstractView>();

	protected I_AbstractInteractor _interactor = null;
	protected IGeoItem theGeoItem = null;
    public Camera _camera;
    protected double _transitionSpeed;
    protected double _regularSpeed;
    protected String _title;

	/*==========================================================*/
	// A single point of handling view-changing events
	
	@SuppressWarnings("unused")
	private static HandlerRegistration view_listener = 
		GE.getPlugin().getView().addViewChangeListener(new ViewChangeListener(){

			@Override public void onViewChangeBegin() { }

			// As onViewChange fired, we redirect it to the onViewChanged virtual function
			// of the current view.
			@Override public void onViewChange() {
				if (views.isEmpty())
					return;
				
				I_UpdatableView currentView = views.peek();
				currentView.onViewChanged();
			}

			// As onViewChangeEnd fired, we redirect it to the onTransitionStopped
			// virtual function of the current view.
			@Override
			public void onViewChangeEnd() {
				if (views.isEmpty())
					return;
				
				I_AbstractView currentView = views.peek();
				if (currentView.isSetEnabledScheduled())
				{
					currentView.setEnabled(true);
					currentView.onTransitionStopped();
				}
			}
		});
	
	/*==========================================================*/
	
	protected _AbstractView() {
		_regularSpeed = GE.getPlugin().getFlyToSpeedTeleport();
		_transitionSpeed = 0.5;
	}

	private static String getPrintableViews(String title)
	{
		String stackMsg = title + ": the views (";
		for (I_UpdatableView item : views)
			stackMsg += item.getClass().getName() + "->"; 
		return stackMsg + "<none>)";
	}

	public static void Push(I_AbstractView newView) {
		newView.scheduleSetEnabled();
		newView.setupCamera();
		
		if (!views.isEmpty())
		{
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

		views.pop();
		I_AbstractView newView = views.peek(); 
		newView.setupCamera();

		//newView.setEnabled(true);
		newView.scheduleSetEnabled();
		
		Log.write(getPrintableViews("Pop"));

        GE.getPlugin().getOptions().setFlyToSpeed(newView.getTransitionSpeed());
        newView.getCamera().Apply();
        GE.getPlugin().getOptions().setFlyToSpeed(newView.getRegularSpeed());

        return newView;
	}

	public static void Pop_Push(I_AbstractView newView) {
		I_AbstractView currView = views.peek();
		if (views.size() < 2)
			return;
		
		currView.setEnabled(false);

		views.pop();

		newView.scheduleSetEnabled();
		newView.setupCamera();
		
		views.push(newView);

		Log.write(getPrintableViews("Pop-Push"));

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
		for(SuiteGeoItem suiteGeo : getSuiteGeoItems())
			suiteGeo.applyFilter();
	}

	@Override
	public IGeoItem getGeoItem() {
		return theGeoItem;
	}

	@Override
	public double getTransitionSpeed(){
		return _transitionSpeed;
	}
	@Override
	public double getRegularSpeed(){
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
	
	protected void setupStandardLookAtCamera()
	{
        if (_camera != null)
        {
        	_camera.attributes.SetLonLatAlt(theGeoItem);
        }
        else
        {
	        I_AbstractView curView = _AbstractView.getCurrentView();
	        if (curView != null)
	        {
	        	_camera = new Camera(curView.getCamera());
		        _camera.attributes.SetLonLatAlt(theGeoItem);
		        _camera.attributes.Tilt_d = theGeoItem.getPosition().getTilt();
		        _camera.attributes.Range_m = theGeoItem.getPosition().getRange();
	        }
	        else
	        {
	        	_camera = new Camera(Camera.Type.LookAt,
	        			theGeoItem.getPosition().getHeading(),
	        			theGeoItem.getPosition().getTilt(),
	        			0,
	                    theGeoItem.getPosition().getLatitude(),
	                    theGeoItem.getPosition().getLongitude(),
	                    theGeoItem.getPosition().getAltitude(),
	                    theGeoItem.getPosition().getRange());
	        }
        }
	}

	boolean setEnabledScheduled = false;
	public boolean isSetEnabledScheduled()
	{
		boolean isScheduled = setEnabledScheduled; 
		setEnabledScheduled = false;
		return isScheduled;
	}; 
	public void scheduleSetEnabled(){setEnabledScheduled = true;};
	
	/*-----------------------------------------------------------------------*/
	private static Map<Integer, SiteGeoItem> siteGeoItems = new HashMap<Integer, SiteGeoItem>();
	private static Map<Integer, BuildingGeoItem> buildingGeoItems = new HashMap<Integer, BuildingGeoItem>();
	private static Map<Integer, SuiteGeoItem> suiteGeoItems = new HashMap<Integer, SuiteGeoItem>();

	// This static function creates all the GeoItems for the given
    // Site and all obtained hierarchy of elements (BuildingGeo-s, SuiteGeo-s, etc.)
    public static void CreateAllGeoItems()
    {
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
			SuiteGeoItem suiteGeo = new SuiteGeoItem(suite);
			suiteGeoItems.put(suite.getId(), suiteGeo);
			
			count++;
			Document.progressBar.Update(count*100.0/howMany);
		}

		Document.progressBar.Cleanup();
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
}
