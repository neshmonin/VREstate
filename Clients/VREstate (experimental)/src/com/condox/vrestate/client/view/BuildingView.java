package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Building;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Site;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.BuildingInteractor;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlLookAt;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;
import com.nitrous.gwt.earth.client.api.KmlVec2;

public class BuildingView extends GEView {

	private BuildingInteractor interactor = null;
	private Building selection = null;
	private KmlLookAt look_at = GE.getPlugin().createLookAt("");

	public BuildingView(Building building) {
		super();
		selection = building;
		interactor = new BuildingInteractor(this);
		setEnabled(true);

	}

	@Override
	public void setEnabled(boolean value) {
		Log.write("BuildingView::setActive()" + value);
		super.setEnabled(value);
		if (value) {
			// // setPosition(selection.getPosition());
			// // UpdateLookAt();
			look_at.setLatitude(selection.getPosition().getLatitude());
			look_at.setLongitude(selection.getPosition().getLongitude());
			look_at.setAltitude(selection.getPosition().getAltitude());
			look_at.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
			heading = GE.getView()
					.copyAsLookAt(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND)
					.getHeading();
			look_at.setHeading(heading);
			tilt = GE.getView()
					.copyAsLookAt(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND)
					.getTilt();
			look_at.setTilt(tilt);
			// if (range == 0)
			// range = building.getPosition().getRange();
			range = 300;
			look_at.setRange(range);
			Update(1);
		}
		// address_overlay.setVisibility(value);
		interactor.setEnabled(value);
		// Draw();
	}

	@Override
	public boolean isSelected(Object item) {
		if (selection.equals(item))
			return true;
		for (Site site : Document.get().getSites())
			if (site.getId() == selection.getParent_id())
				if (site.equals(item))
					return true;
		return false;
	}

	@Override
	public void Select(String type, int id) {
		if (type.equals("building"))
			if (selection.getId() == id) {
				if (Document.get().getBuildings().size() > 1)
					Pop();
			} else
				for (Building building : Document.get().getBuildings())
					if (building.getId() == id) {
						selection = building;
						setEnabled(true);
					}
		if (type.equals("suite"))
			for (Suite suite : Document.get().getSuites())
				if (suite.getId() == id)
					new SuiteView(suite);
	}

	@Override
	public String getCaption() {
		return selection.getAddress();
	}

	public void Update(double speed) {
		double old_speed = GE.getPlugin().getOptions().getFlyToSpeed();
		double new_speed = speed;
		if (new_speed < 0)
			new_speed = GE.getPlugin().getFlyToSpeedTeleport();
		GE.getPlugin().getOptions().setFlyToSpeed(new_speed);
//		Log.write("look_at.heading: " + look_at.getHeading());
//		Log.write("look_at.tilt: " + look_at.getTilt());
//		Log.write("look_at.range: " + look_at.getRange());
		GE.getView().setAbstractView(look_at);
		GE.getPlugin().getOptions().setFlyToSpeed(old_speed);
	}

	private double tilt = 0;
	private double range = 100;

	public void Move(double dH, double dT, double dR) {
		Log.write("Move: " + dH + " " + dT + " " + dR);
		heading += dH / 8;
		tilt -= dT / 12;
		range += dR;

		// if (Math.abs(heading - selection.getPosition().getHeading()) < 45)
		// look_at.setHeading(heading);
		// if (Math.abs(heading - selection.getPosition().getHeading()) > 315)
		// look_at.setHeading(heading);
		while (heading < 0)
			heading += 360;
		while (heading > 360)
			heading -= 360;
		tilt = Math.max(tilt, 10);
		tilt = Math.min(tilt, 170);

		range = Math.max(range, 100);

		look_at.setHeading(heading);
		look_at.setTilt(tilt);
		look_at.setRange(range);
		// Update(0);
	}

	@Override
	public void Update() {
		KmlVec2 coords = GE.getView().project(
				selection.getPosition().getLatitude(),
				selection.getPosition().getLongitude(),
				selection.getPosition().getAltitude(),
				KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		if (coords == null)
			Update(1);
		else {
			double screen_center_X = GE.getEarth().getOffsetWidth() / 2;
			double screen_center_Y = GE.getEarth().getOffsetHeight() / 2;
			double dX = Math.abs(screen_center_X - coords.getX());
			double dY = Math.abs(screen_center_Y - coords.getY());
			if ((dX > 10) || (dY > 10))
				Update(1);
			else
				Update(-1);
		}
	}
	
	private double heading_diff = 0;
	private double tilt_diff = 0;
	private double range_diff = 0;
	
	public void addHeadingDiff(double value) {
		heading_diff += value;
	}
	
	public void addTiltDiff(double value) {
		tilt_diff += value;
	}
	
	public void addRangeDiff(double value) {
		range_diff += value;
	}
	
	public void onFrameEnd() {
		heading += heading_diff;
		heading_diff = 0;
		tilt -= tilt_diff;
		tilt_diff = 0;
		range += range_diff;
		range_diff = 0;
		
		while (heading < 0)
			heading += 360;
		while (heading > 360)
			heading -= 360;
		
		tilt = Math.max(tilt, 10);
		tilt = Math.min(tilt, 170);

		range = Math.max(range, 100);
		
		look_at.setHeading(heading);
		look_at.setTilt(tilt);
		look_at.setRange(range);
		Update(-1);
	}
	
}
