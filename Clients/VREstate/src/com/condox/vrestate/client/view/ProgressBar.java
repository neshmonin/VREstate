package com.condox.vrestate.client.view;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.OverlayHelpers;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;

public class ProgressBar extends OverlayHelpers
						 implements I_UpdatableView 
{

	private OvlRectangle progressRect = null;
	private OvlRectangle labelRect = null;
	private KmlScreenOverlay labelOvl = null;
	private KmlScreenOverlay progressOvl = null;
	private String displayedProgress = null;
	private String displayedLabel = "<none>";
	
	public enum ProgressLabel
	{
		Loading,
		Processing,
		Executing,
		Wait,
		None
	}
	
	public ProgressBar()
	{
		progressRect = new OvlRectangle(
				new OvlPoint(new OvlDimension(0.5f), 
							 new OvlDimension(0.5f)),
						new OvlDimension(299),
						new OvlDimension(16)
			);
	}
	
	private String getUpdatedDisplayUrl(double percent)
	{
		String progressUrl = null;
		if (percent < 5.0) progressUrl = "0p.jpg";
		else if (percent < 15.0) progressUrl = "10p.jpg";
		else if (percent < 25.0) progressUrl = "20p.jpg";
		else if (percent < 35.0) progressUrl = "30p.jpg";
		else if (percent < 45.0) progressUrl = "40p.jpg";
		else if (percent < 55.0) progressUrl = "50p.jpg";
		else if (percent < 65.0) progressUrl = "60p.jpg";
		else if (percent < 75.0) progressUrl = "70p.jpg";
		else if (percent < 85.0) progressUrl = "80p.jpg";
		else if (percent < 95.0) progressUrl = "90p.jpg";
		else progressUrl = "100p.jpg";
		
		if (progressUrl.equals(displayedProgress))
			return null;
		
		displayedProgress = progressUrl;
		return progressUrl;
	}
	
	private String getLabelHref(ProgressLabel label)
	{
		String hrefLabel = null;
		switch (label)
		{
		case Loading:
			hrefLabel = "Loading.png";
			break;
		case Processing:
			hrefLabel = "Processing.png";
			break;
		case Executing:
			hrefLabel = "Executing.png";
			break;
		case Wait:
			hrefLabel = "WaitMessage.png";
			break;
		default:
			return null;
		}

		OvlDimension dimX, dimY;
		if (label == ProgressLabel.Wait)
		{
			dimX = new OvlDimension(0.8f);
			dimY = new OvlDimension(0.8f);
		}
		else
		{
			dimX = new OvlDimension(226);
			dimY = new OvlDimension(36);
		}

		labelRect = new OvlRectangle(
				new OvlPoint(new OvlDimension(0.5f), 
							 new OvlDimension(0.55f)),
						dimX,
						dimY
			);

		if (hrefLabel.equals(displayedLabel))
			return null;
		
		displayedLabel = hrefLabel;
		
		return hrefLabel;
	}
	
	double percent = 0.0;
	
	public void Update(double percent)
	{
		this.percent = percent;
		onViewChanged();
	}

	ProgressLabel label = ProgressLabel.None;
	
	public void Update(ProgressLabel label)
	{
		this.label = label;
		onViewChanged();
	}

	public void Cleanup()
	{
		if (progressOvl != null)
		{
			GE.getPlugin().getFeatures().removeChild(progressOvl);
			progressOvl = null;
		}
		
		if (labelOvl != null)
		{
			GE.getPlugin().getFeatures().removeChild(labelOvl);
			labelOvl = null;
		}
	}

	@Override
	public void onViewChanged() {
		
		String hrefLabel = getLabelHref(label);
		if (hrefLabel != null)
		{
			hrefLabel = Options.HOME_URL + "buttons/" + hrefLabel; 
			KmlIcon iconLabel = GE.getPlugin().createIcon("");
			iconLabel.setHref(hrefLabel);

			KmlScreenOverlay newOverlay = GE.getPlugin().createScreenOverlay("");
			labelRect.InitScreenOverlay(newOverlay);

			newOverlay.setIcon(iconLabel);
			
			if (labelOvl == null)
				GE.getPlugin().getFeatures().appendChild(newOverlay);
			else
				GE.getPlugin().getFeatures().replaceChild(newOverlay, labelOvl);
			
			labelOvl = newOverlay;
		}
		
		String progressUrl = getUpdatedDisplayUrl(percent);
		if (progressUrl != null)
		{
			progressUrl = Options.HOME_URL + "buttons/" + progressUrl;
			KmlIcon iconLabel = GE.getPlugin().createIcon("");
			iconLabel.setHref(progressUrl);

			KmlScreenOverlay newOverlay = GE.getPlugin().createScreenOverlay("");
			progressRect.InitScreenOverlay(newOverlay);

			newOverlay.setIcon(iconLabel);
			
			if (progressOvl == null)
				GE.getPlugin().getFeatures().appendChild(newOverlay);
			else
				GE.getPlugin().getFeatures().replaceChild(newOverlay, progressOvl);
			
			progressOvl = newOverlay;
		}
	}
	/*
	private void updateOverlay( String newHref, 
								KmlScreenOverlay overlayToUpdate, 
								OvlRectangle rectangleToUpdate)
	{
		if (newHref != null)
		{
			KmlIcon iconLabel = GE.getPlugin().createIcon("");
			iconLabel.setHref(newHref);

			KmlScreenOverlay newOverlay = GE.getPlugin().createScreenOverlay("");
			rectangleToUpdate.InitScreenOverlay(newOverlay);

			newOverlay.setIcon(iconLabel);
			
			if (overlayToUpdate == null)
				GE.getPlugin().getFeatures().appendChild(newOverlay);
			else
				GE.getPlugin().getFeatures().replaceChild(newOverlay, overlayToUpdate);
			
			overlayToUpdate = newOverlay;
		}
	}
*/
	@Override public void onTransitionStopped() {}
	
	boolean updateViewAfterCameraStopFlag = false;
	public boolean isSetEnabledScheduled(){return updateViewAfterCameraStopFlag;}; 
	public void setUpdateViewAfterCameraStopFlag(boolean on){updateViewAfterCameraStopFlag = on;};
}
