package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Building;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Site;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.SiteInteractor;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlLookAt;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.KmlVec2;
import com.nitrous.gwt.earth.client.api.event.KmlLoadCallback;

public class SiteView extends GEView {

	private SiteInteractor interactor = null;
	private Site selection = null;
	private KmlLookAt look_at = null;

	public SiteView(Site site) {
		super();
		selection = site;
		interactor = new SiteInteractor(this);
		look_at = GE.getView().copyAsLookAt(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		look_at.setLatitude(site.getPosition().getLatitude());
		look_at.setLongitude(site.getPosition().getLongitude());
		look_at.setAltitude(site.getPosition().getAltitude());
		look_at.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		look_at.setHeading(heading);
		look_at.setTilt(tilt);
		if (range == 0)
			range = site.getPosition().getRange();
		look_at.setRange(range);
		// TODO Сделать проверку DisplayModelUrl
		Log.write(Options.HOME_URL + selection.getDisplayModelUrl());
		GE.getPlugin().fetchKml(Options.HOME_URL + selection.getDisplayModelUrl(), new KmlLoadCallback() {

			@Override
			public void onLoaded(KmlObject feature) {
				// TODO Auto-generated method stub
				if (feature != null)
					GE.getPlugin().getFeatures().appendChild(feature);
			}});
		setEnabled(true);
	}

	@Override
	public void setEnabled(boolean value) {
		super.setEnabled(value);
		if (value) {
			look_at.setHeading(heading);
			look_at.setTilt(tilt);
			look_at.setRange(range);
			Update(0.5);
		}
		interactor.setEnabled(value);
		Draw();
	}

	@Override
	public boolean isSelected(Object item) {
		if (selection.equals(item))
			return true;
		return false;
	}

	@Override
	public void Select(String type, int id) {
		if (type.equals("building"))
			for (Building building : Document.get().getBuildings()) {
				if (building.getId() == id)
					new BuildingView(building);
			}
		if (type.equals("suite"))
			for (Suite suite : Document.get().getSuites())
				if (suite.getId() == id)
					for (Building building : Document.get().getBuildings())
						if (building.getId() == suite.getParent_id()) {
							new BuildingView(building);
							new SuiteView(suite);
						}
	}

	public void Move(double dH, double dT, double dR) {
////		Log.write("Move: " + dH + " " + dT + " " + dR);
//		heading += dH / 8;
//		tilt -= dT / 12;
//		range += dR;
//
//		// if (Math.abs(heading - selection.getPosition().getHeading()) < 45)
//		// look_at.setHeading(heading);
//		// if (Math.abs(heading - selection.getPosition().getHeading()) > 315)
//		// look_at.setHeading(heading);
//		while (heading < 0)
//			heading += 360;
//		while (heading > 360)
//			heading -= 360;
//		
//		tilt = Math.max(tilt, 10);
//		tilt = Math.min(tilt, 80);
//
//		range = Math.max(range, 100);
//
//		look_at.setHeading(heading);
//		look_at.setTilt(tilt);
//		look_at.setRange(range);
//		Update(0);

		this.dH += dH;
		this.dT += dT;
	}

	public void Update(double speed) {
		Log.write("Update:start");
		double old_speed = GE.getPlugin().getOptions().getFlyToSpeed();
		double new_speed = speed;
		if (new_speed == 0)
			new_speed = GE.getPlugin().getFlyToSpeedTeleport();
		GE.getPlugin().getOptions().setFlyToSpeed(new_speed);
		Log.write("look_at.heading: " + look_at.getHeading());
		Log.write("look_at.tilt: " + look_at.getTilt());
		Log.write("look_at.range: " + look_at.getRange());
		GE.getView().setAbstractView(look_at);
		Log.write(look_at.toString());
		GE.getPlugin().getOptions().setFlyToSpeed(old_speed);
		Log.write("Update:end");
	}

	@Override
	public String getCaption() {
		// TODO Auto-generated method stub
		return selection.getName();
	}

	@Override
	public void Update() {
		KmlVec2 coords = GE.getView().project(selection.getPosition().getLatitude(),
				selection.getPosition().getLongitude(), selection.getPosition().getAltitude(),
				KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		if (coords == null)
			Update(1);
		else {
			double screen_center_X = GE.getEarth().getOffsetWidth() / 2;
			double screen_center_Y = GE.getEarth().getOffsetHeight() / 2;
			double dX = Math.abs(screen_center_X - coords.getX());
			double dY = Math.abs(screen_center_Y - coords.getY());
			if ((dX > 10) || (dY > 10))
				Update(0.5);
			else
				Update(0);
		}
	}
	
	private double dH = 0;
	private double dT = 0;
	
	public void onFrameEnd() {
		heading += dH / 2;
		dH *= 1 / 2; 
		tilt -= dT / 2;
		dT *= 1 / 2;
		
		while (heading < 0)
			heading += 360;
		while (heading > 360)
			heading -= 360;
		
		tilt = Math.max(tilt, 10);
		tilt = Math.min(tilt, 80);


		look_at.setHeading(heading);
		look_at.setTilt(tilt);
		look_at.setRange(range);
		Update(0);
	}
	
	
}
