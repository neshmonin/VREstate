package com.condox.vrestate.client.view;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.ViewOrder.ProductType;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.SB_Interactor;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;
import com.nitrous.gwt.earth.client.api.GEVisibility;

public abstract class _SB_View extends _GEView implements I_SB_View {

	protected double dH = 0;
	protected double dT = 0;
	protected double dR = 0;
	
	protected _SB_View(IGeoItem geoItem) {
		super( geoItem );
	}

	@Override
	public void setEnabled(boolean enabling) {
		super.setEnabled(enabling);
		if(Document.targetViewOrder == null) 
			Filter.get().setVisible(enabling);
		else if (Document.targetViewOrder.getProductType() == ProductType.PublicListing ||
				 Document.targetViewOrder.getProductType() == ProductType.Building3DLayout)
			Filter.get().setVisible(enabling);

		if (enabling) {
			GE.getPlugin().getNavigationControl().setVisibility(GEVisibility.VISIBILITY_AUTO);
			if (_interactor == null)
				_interactor = new SB_Interactor(this);
			_interactor.setEnabled(true);
		}
		else
		{
			if (_interactor != null)
			{
				_interactor.setEnabled(false);
				_interactor = null;
			}
		}
	}

	@Override
	public void Move(double dH, double dT, double dR) {
		this.dH += dH;
		this.dT += dT;
		this.dR += dR;
		doViewChanged();
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
	public void setupCamera(I_AbstractView poppedView) {
		setupStandardLookAtCamera(poppedView);
    }

	@Override
	public void onTransitionStopped() {
		if (Document.progressBar != null){
			// This is initial loading
			onHeadingChanged();
			if(Document.targetViewOrder != null) {
				int targetId = Document.targetViewOrder.getTargetObject().getId();
				switch (Document.targetViewOrder.getTargetObjectType())
				{
				case Suite:
					Select("suite", targetId);
					break;
				case Building:
					Select("building", targetId);
					break;
				}
			}

			Document.progressBar = null;
		}
		else
			onHeadingChanged();
	}
}