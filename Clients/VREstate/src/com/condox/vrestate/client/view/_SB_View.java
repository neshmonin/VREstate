package com.condox.vrestate.client.view;


import com.condox.vrestate.shared.Document;
import com.condox.vrestate.shared.IGeoItem;
import com.condox.vrestate.shared.I_AbstractView;
import com.condox.vrestate.shared.Options;
import com.condox.vrestate.shared.ViewOrder.ProductType;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.SB_Interactor;
import com.condox.vrestate.client.interactor.SB_Kiosk_Interactor;
import com.nitrous.gwt.earth.client.api.GEVisibility;

public abstract class _SB_View extends _GEView implements I_SB_View {

	protected double dH = 0;
	protected double dT = 0;
	protected double dR = 0;
	protected double dP = 0;
	private static Filter filter = null;
	
	protected _SB_View(IGeoItem geoItem) {
		super( geoItem );
		if (filter == null) {
			filter = Filter.get();
			filter.Reset();
		}
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
			if (_interactor == null) {
				if (Options.ROLE == Options.ROLES.KIOSK)
					_interactor = new SB_Kiosk_Interactor(this);
				else
					_interactor = new SB_Interactor(this);
			}
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
	public void Pan(double dP) {
        _camera.attributes.Alt_m += dP/10;
		_camera.MoveLookAt(0, 0, 0);
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
		setupStandardLookAtCamera((_GEView)poppedView);
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
				default:
					break;
				}
			}

			Document.progressBar.CleanupProgress();
			Document.progressBar = null;
		}
		else
			onHeadingChanged();
	}
}