package com.condox.vrestate.client.view;

import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.Camera.Camera;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.condox.vrestate.shared.IGeoItem;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlLookAt;

public abstract class _GEView extends _AbstractView {

	public _GEView(IGeoItem item) {
		theGeoItem = item;
	}

	@Override
	public void onHeadingChanged() {
		double heading_d = getHeading();
		for (SuiteGeoItem suiteGeo : _AbstractView.getSuiteGeoItems().values())
			suiteGeo.onHeadingChanged(heading_d);
	}

	private double getHeading() {
		KmlLookAt look_at = GE.getView().copyAsLookAt(
				KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		if (look_at != null)
			return look_at.getHeading();
		return 0;
	}
	
	protected void setupStandardLookAtCamera(_GEView poppedView) {
		if (_camera != null) {
			_camera.attributes.SetLonLatAlt(theGeoItem);
			if (poppedView != null) {
				Camera poppedCamera = poppedView.getCamera();
				if (poppedCamera.CameraType == _camera.CameraType)
					_camera.attributes.Heading_d = Camera
							.NormalizeHeading_d(poppedCamera.attributes.Heading_d);
			}
		} else {
			_GEView curView = (_GEView)_AbstractView.getCurrentView();
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

}
