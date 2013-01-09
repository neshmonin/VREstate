package com.condox.vrestate.client.view;

import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.PanoramicInteractor;
import com.condox.vrestate.client.view.Camera.Camera;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;
import com.nitrous.gwt.earth.client.api.GEVisibility;

public class PanoramicView extends _GEView {

	public PanoramicView(IGeoItem geoItem) {
		super( geoItem );
	}

	@Override
	public void setEnabled(boolean enabling) {
		super.setEnabled(enabling);
		if (enabling) {
			GE.getPlugin().getNavigationControl().setVisibility(GEVisibility.VISIBILITY_HIDE);
			if (_interactor == null)
				_interactor = new PanoramicInteractor(this);
			_interactor.setEnabled(true);
		} else {
			_interactor.setEnabled(false);
			_interactor = null;
		}
	}

	// @Override
	public boolean isSelected(Object item) {
		return false;
	}


	public void Move(double dH, double dT) {
		//this.dH -= dH;
		//this.dT += dT;
		//onViewChanged();

		_camera.MoveCamera(-dH / 10, dT / 10);
	}
	
	public void Center() {
        _camera.attributes.Heading_d = theGeoItem.getPosition().getHeading();
        _camera.attributes.Tilt_d = 90; 
        GE.getPlugin().getOptions().setFlyToSpeed(getTransitionSpeed());
        _camera.Apply();
        GE.getPlugin().getOptions().setFlyToSpeed(getRegularSpeed());
	}

	@Override
	public String getTitleText() {
		return "Suite " + theGeoItem.getName() + " - Out-of-Window View";
	}

	@Override
	public void onViewChanged() {
		/*double deltaHeading = dH / 10;
		double deltaTilt = dT / 10;
		if (deltaHeading == 0.0 && deltaTilt == 0.0)
			return;
		
		dH *= 1 / 10; 
		dT *= 1 / 10;

		_camera.MoveCamera(deltaHeading, deltaTilt);*/
	}

	@Override
    public void setupCamera()
    {
		double METERS_PER_DEGREES = 111111;
		double lat = theGeoItem.getPosition().getLatitude()
				+ 50 / 2.54 * 0.0254
				* Math.cos(Math.toRadians(theGeoItem.getPosition()
						.getHeading())) / METERS_PER_DEGREES;
		double lon = theGeoItem.getPosition().getLongitude()
				+ 50 / 2.54 * 0.0254
				* Math.sin(Math.toRadians(theGeoItem.getPosition()
						.getHeading()))
				/ METERS_PER_DEGREES
				/ Math.cos(Math.toRadians(theGeoItem.getPosition()
						.getLatitude()));
		
    	_camera = new Camera(Camera.Type.Camera,
    			theGeoItem.getPosition().getHeading(),
    			90,
    			0,
    			lat,
    			lon,
                theGeoItem.getPosition().getAltitude() + 1.5,
                0);
    }

	@Override
	public void Select(String type, int id) {
	}

	@Override
	public void onTransitionStopped() { }

}
