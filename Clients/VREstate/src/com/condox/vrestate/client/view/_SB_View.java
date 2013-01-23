package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.SB_Interactor;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;
import com.nitrous.gwt.earth.client.api.GEVisibility;

public abstract class _SB_View extends _GEView implements I_SB_View {

	private double dH = 0;
	private double dT = 0;
	private double dR = 0;
	
	protected _SB_View(IGeoItem geoItem) {
		super( geoItem );
	}
	
	@Override
	public void setEnabled(boolean enabling) {
		super.setEnabled(enabling);
		if (enabling) {
			GE.getPlugin().getNavigationControl().setVisibility(GEVisibility.VISIBILITY_AUTO);
			if (_interactor == null)
				_interactor = new SB_Interactor(this);
			_interactor.setEnabled(true);
		}
		else
		{
			_interactor.setEnabled(false);
			_interactor = null;
		}

		if (!Options.isViewOrder())
			Filter.get().setVisible(enabling);
	}

	@Override
	public void Move(double dH, double dT, double dR) {
		this.dH += dH;
		this.dT += dT;
		this.dR += dR;
		onViewChanged();
	}

	@Override
	public void onViewChanged() {
		double deltaHeading = dH / 6;
		double deltaTilt = dT / 6;
		double deltaRange = dR / 2;
		if (deltaHeading == 0.0 && deltaTilt == 0.0 && deltaRange == 0.0)
			return;
		
		dH *= 1 / 6; 
		dT *= 1 / 6;
		dR *= 1 / 2;
		
		_camera.MoveLookAt(deltaHeading, -deltaTilt, deltaRange);
	}

	@Override
	public void setupCamera() {
		setupStandardLookAtCamera();
    }

	@Override
	public void onTransitionStopped() {
		if (Document.progressBar != null){
			// This is initial loading
			onHeadingChanged();
			if(Document.targetSuite != null)
				Select("suite", Document.targetSuite.getId());

			Document.progressBar = null;
		}
		else
			onHeadingChanged();
	}
}