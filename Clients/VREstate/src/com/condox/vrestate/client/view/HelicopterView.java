package com.condox.vrestate.client.view;

import com.condox.vrestate.client.interactor.SingleTouchInteractor;
import com.condox.vrestate.client.view.GeoItems.BuildingGeoItem;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;
import com.google.gwt.user.client.Timer;

public class HelicopterView extends SiteView {
	protected int tickCounter = 0; 
    protected Timer updateViewTimer = new Timer(){

		@Override
		public void run() {
			if (isFlying) {
				dH = 6;
				doViewChanged();
				tickCounter++;
				if (tickCounter >= 10){
					tickCounter = 0;
					onHeadingChanged();
				}
			}
			else
				updateViewTimer.cancel();
		}
	};

	private boolean isFlying = false;
	public HelicopterView(IGeoItem geoItem) {
		super(geoItem);
		isFlying = false;
	}

	@Override
	public void setEnabled(boolean enabling) {
		if (enabling) {
			if (_interactor == null)
				_interactor = new SingleTouchInteractor(this);
			_interactor.setEnabled(true);
			isFlying = true;;
			updateViewTimer.schedule(200);
			
//			GEHtmlStringBalloon balloon = GE.getPlugin().createHtmlStringBalloon("");
//			Login login = new Login();
//			balloon.setContentString(login.toString());
//			GE.getPlugin().setBalloon(balloon);
		}
		else {
			_interactor.setEnabled(false);
			_interactor = null;
			isFlying = false;
		}
	}

	@Override
	public void onViewChanged() {
		super.onViewChanged();
		updateViewTimer.schedule(200);
	}

	@Override
	public double getStartingRange() {
		return 200.0;
	}
	
	
	public void pushNextView() {
		isFlying = false;
		SiteView siteView = new SiteView(theGeoItem);
		_AbstractView.Push(siteView);
		if (_AbstractView.getBuildingGeoItems().size() == 1) {
			for (BuildingGeoItem buildingGeo : _AbstractView.getBuildingGeoItems().values())
				_AbstractView.AddSelection(buildingGeo);
		}
	}
}
