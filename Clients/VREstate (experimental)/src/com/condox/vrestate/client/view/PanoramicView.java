package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Filter;
import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Position;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.PanoramicInteractor;
import com.google.gwt.user.client.Window;
import com.nitrous.gwt.earth.client.api.GEVisibility;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlCamera;
import com.nitrous.gwt.earth.client.api.KmlLookAt;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;

public class PanoramicView extends GEView {

//	private KmlLookAt camera = GE.getPlugin().createLookAt("");
	private KmlCamera camera = null;
	private PanoramicInteractor interactor = null;
	private Suite selection = null;
	private double heading = 0;
	private double tilt = 90;

	private KmlScreenOverlay address_overlay = GE.getPlugin()
	.createScreenOverlay("");

	public PanoramicView(Suite suite) {
		this.selection = suite;
		Position position = suite.getPosition();
		double lat = position.getLatitude();
		double lon = position.getLongitude();
		double alt = position.getAltitude();
		heading = position.getHeading();
		tilt = 90;
		
//		Log.write("Suite position:");
//		Log.write(" lat:" + lat);
//		Log.write(" lon:" + lon);
//		Log.write(" alt:" + alt);
//		Log.write(" heading:" + heading);
//		Log.write(" tilt:" + tilt);
//		Log.write(" roll:" + roll);
//		
//		camera = GE.getView().copyAsCamera(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
//		
//		lat = camera.getLatitude();
//		lon = camera.getLongitude();
//		alt = camera.getAltitude();
//		heading = camera.getHeading();
//		tilt = camera.getTilt();
//		roll = camera.getRoll();
//		
//		Log.write("Camera position:");
//		Log.write(" lat:" + lat);
//		Log.write(" lon:" + lon);
//		Log.write(" alt:" + alt);
//		Log.write(" heading:" + heading);
//		Log.write(" tilt:" + tilt);
//		Log.write(" roll:" + roll);
		
		camera = GE.getView().copyAsCamera(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		camera.setLatitude(lat);
		camera.setLongitude(lon);
		camera.setAltitude(alt);
		camera.setHeading(heading);
		camera.setTilt(90);
//		tilt = camera.getTilt();
		
		
		
		
		
		interactor = new PanoramicInteractor(this);
//		camera.setLatitude(suite.getPosition().getLatitude());
//		camera.setLongitude(suite.getPosition().getLongitude());
//		camera.setAltitude(suite.getPosition().getAltitude());
//		camera.setAltitudeMode(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
//		camera.setHeading(suite.getPosition().getHeading());
//		camera.setTilt(90);
////		camera.setRange(500);
//		camera.setRoll(0);
		
//		heading = camera.getHeading();
//		tilt = camera.getTilt();
//		camera.setAltitude(camera.getAltitude() + 100);
//		GE.getView().setAbstractView(camera);
		
		setEnabled(true);
	}

	@Override
	public void setEnabled(boolean value) {
		super.setEnabled(value);
		if (value) {
			GE.getPlugin().getNavigationControl().setVisibility(GEVisibility.VISIBILITY_HIDE);
			// setPosition(selection.getPosition());
			// UpdateCamera();
			Update(0.5);
		} else {
			GE.getPlugin().getNavigationControl().setVisibility(GEVisibility.VISIBILITY_AUTO);
		}
//		address_overlay.setVisibility(value);
		if (value)
			Filter.get().setVisible(false);
		interactor.setEnabled(value);
	}

	// TODO Доработать
	// @Override
	public boolean isSelected(Object item) {
		// if (selection.equals(item))
		// return true;
		// for (Site site : Document.get().getSites())
		// if (site.getId() == selection.getParent_id())
		// if (site.equals(item))
		// return true;
		return false;
	}

	//
	// @Override
	// public void Select(String type, int id) {
	// // if (type.equals("building"))
	// // if (selection.getId() == id)
	// // Pop();
	// // else
	// // for (Building building : Document.get().getBuildings())
	// // if (building.getId() == id) {
	// // selection = building;
	// // setActive(true);
	// // }
	// // if (type.equals("suite"))
	// // for (Suite suite : Document.get().getSuites())
	// // if (suite.getId() == id)
	// // Push(new SuiteView(suite));
	// }
	//
	// @Override
	// public void UpdateCamera() {
	// // center_overlay.setVisibility(back_overlay.getVisibility());
	// super.UpdateCamera();
	// }
	//
	// @Override
	// public void UpdateCamera(double dX, double dY, double dZ) {
	// KmlCamera camera =
	// GE.getPlugin().getView().copyAsCamera(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
	// double heading = camera.getHeading() + dX;
	// double tilt = camera.getTilt() + dY;
	// double dH = 0;
	// double dT = 0;
	// // if (heading)
	// // TODO Auto-generated method stub
	// super.UpdateCamera(dX, dY, dZ);
	// }

	public void Move(double dH, double dT) {
//		Log.write("view.Move " + dH + " " + dT);
		heading -= dH / 8;
		tilt += dT / 12;
		if ((Math.abs(heading - selection.getPosition().getHeading()) > 60)&&
				(Math.abs(heading - selection.getPosition().getHeading()) < 300)) {
			heading += dH / 8;
			tilt -= dT / 12;
			return;
		}
		
		while(heading < 0)
			heading += 360;
		while(heading > 360)
			heading -= 360;
		camera.setHeading(heading);
		
		tilt = Math.max(tilt, 10);
		tilt = Math.min(tilt, 170);
		// сопли
//		if ((tilt > 85)&&(tilt < 95))
//			tilt = 85;
			
		camera.setTilt(tilt);
		
		
		Update();
	}
	
	public void Center() {
		heading = selection.getPosition().getHeading();
		tilt = 90;
		camera.setHeading(heading);
		camera.setTilt(tilt);
		Update();
	}

	@Override
	public String getCaption() {
		// TODO Auto-generated method stub
		return selection.getName() + " - Panoramic View";
	}

	public void Update(double speed) {
//		Log.write("Update:start");
		double old_speed = GE.getPlugin().getOptions().getFlyToSpeed();
		double new_speed = speed;
		if (new_speed < 0)
			new_speed = GE.getPlugin().getFlyToSpeedTeleport();
		GE.getPlugin().getOptions().setFlyToSpeed(new_speed);
		Log.write("camera.heading: " + camera.getHeading());
		Log.write("camera.tilt: " + camera.getTilt());
		Log.write("camera.roll: " + camera.getRoll());
		GE.getView().setAbstractView(camera);
//		Log.write(camera.toString());
		GE.getPlugin().getOptions().setFlyToSpeed(old_speed);
//		Log.write("Update:end");
	}

	@Override
	public void Select(String type, int id) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void Update() {
		Update(-1);
	}

}
