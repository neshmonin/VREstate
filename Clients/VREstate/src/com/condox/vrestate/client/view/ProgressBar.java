package com.condox.vrestate.client.view;


import com.condox.clientshared.abstractview.I_Progress;
import com.condox.clientshared.abstractview.I_UpdatableView;
import com.condox.clientshared.communication.Options;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.interactor.OverlayHelpers;
import com.google.gwt.core.client.GWT;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;

public class ProgressBar extends OverlayHelpers implements I_UpdatableView, I_Progress {

	private OvlRectangle progressRect = null;
	private OvlRectangle labelRect = null;
	private KmlScreenOverlay labelOvl = null;
	private KmlScreenOverlay progressOvl = null;
	private String displayedProgress = null;
	private String displayedLabel = "<none>";
	private ViewMessages i18n;

	public ProgressBar() {
		progressRect = new OvlRectangle(new OvlPoint(new OvlDimension(0.5f),
				new OvlDimension(0.5f)), new OvlDimension(299),
				new OvlDimension(16));
		i18n = (ViewMessages)GWT.create(ViewMessages.class);
	}

	private String getUpdatedDisplayUrl(double percent) {
		String progressUrl = null;
		if (percent < 0)
			return null;
		if (percent < 5.0)
			progressUrl = "0p.jpg";
		else if (percent < 15.0)
			progressUrl = "10p.jpg";
		else if (percent < 25.0)
			progressUrl = "20p.jpg";
		else if (percent < 35.0)
			progressUrl = "30p.jpg";
		else if (percent < 45.0)
			progressUrl = "40p.jpg";
		else if (percent < 55.0)
			progressUrl = "50p.jpg";
		else if (percent < 65.0)
			progressUrl = "60p.jpg";
		else if (percent < 75.0)
			progressUrl = "70p.jpg";
		else if (percent < 85.0)
			progressUrl = "80p.jpg";
		else if (percent < 95.0)
			progressUrl = "90p.jpg";
		else
			progressUrl = "100p.jpg";

		if (progressUrl.equals(displayedProgress))
			return null;

		displayedProgress = progressUrl;
		return progressUrl;
	}

	private String getLabelHref(ProgressType label) {
		String hrefLabel = null;
		switch (label) {
		case Error:
			hrefLabel = i18n.Error();
			break;
		case Loading:
			hrefLabel = i18n.Loading();
			break;
		case Processing:
			hrefLabel = i18n.Processing();
			break;
		case Executing:
			hrefLabel = i18n.Executing();
			break;
		case Wait:
			hrefLabel = i18n.WaitMessage();
			break;
		default:
			return null;
		}

		OvlDimension dimX, dimY;
		if (label == ProgressType.Error) {
			dimX = new OvlDimension(0.5f);
			dimY = new OvlDimension(0.1f);
		} else if (label == ProgressType.Wait) {
			dimX = new OvlDimension(0.8f);
			dimY = new OvlDimension(0.8f);
		} else {
			dimX = new OvlDimension(226);
			dimY = new OvlDimension(36);
		}

		labelRect = new OvlRectangle(new OvlPoint(new OvlDimension(0.5f),
				new OvlDimension(0.55f)), dimX, dimY);

		if (hrefLabel.equals(displayedLabel))
			return null;

		displayedLabel = hrefLabel;

		return hrefLabel;
	}

	double percent = 0.0;

	/* (non-Javadoc)
	 * @see com.condox.vrestate.client.view.I_Progress#Update(double)
	 */
	@Override
	public void UpdateProgress(double percent) {
		this.percent = percent;
		onViewChanged();
	}

	ProgressType label = ProgressType.None;

	/* (non-Javadoc)
	 * @see com.condox.vrestate.client.view.I_Progress#Update(com.condox.vrestate.client.view.ProgressBar.ProgressLabel)
	 */
	@Override
	public void SetupProgress(ProgressType label) {
		UpdateProgress(-1.0);
		this.label = label;
		onViewChanged();
	}

	/* (non-Javadoc)
	 * @see com.condox.vrestate.client.view.I_Progress#Cleanup()
	 */
	@Override
	public void CleanupProgress() {
		if (progressOvl != null) {
			GE.getPlugin().getFeatures().removeChild(progressOvl);
			progressOvl = null;
		}

		if (labelOvl != null) {
			GE.getPlugin().getFeatures().removeChild(labelOvl);
			labelOvl = null;
		}
	}

    @Override
	public void onViewChanged() {
		String hrefLabel = getLabelHref(label);
		if (hrefLabel != null) {
			hrefLabel = Options.URL_BUTTONS + hrefLabel;
			KmlIcon iconLabel = GE.getPlugin().createIcon("");
			iconLabel.setHref(hrefLabel);

			KmlScreenOverlay newOverlay = GE.getPlugin()
					.createScreenOverlay("");
			labelRect.InitScreenOverlay(newOverlay);

			newOverlay.setIcon(iconLabel);

			if (labelOvl == null)
				GE.getPlugin().getFeatures().appendChild(newOverlay);
			else
				GE.getPlugin().getFeatures().replaceChild(newOverlay, labelOvl);

			labelOvl = newOverlay;
		}

		String progressUrl = getUpdatedDisplayUrl(percent);
		if (progressUrl != null) {
			progressUrl = Options.URL_VRT + "buttons/" + progressUrl;
			KmlIcon iconLabel = GE.getPlugin().createIcon("");
			iconLabel.setHref(progressUrl);

			KmlScreenOverlay newOverlay = GE.getPlugin()
					.createScreenOverlay("");
			progressRect.InitScreenOverlay(newOverlay);

			newOverlay.setIcon(iconLabel);

			if (progressOvl == null)
				GE.getPlugin().getFeatures().appendChild(newOverlay);
			else
				GE.getPlugin().getFeatures()
						.replaceChild(newOverlay, progressOvl);

			progressOvl = newOverlay;
		}
	}

	/*
	 * private void updateOverlay( String newHref, KmlScreenOverlay
	 * overlayToUpdate, OvlRectangle rectangleToUpdate) { if (newHref != null) {
	 * KmlIcon iconLabel = GE.getPlugin().createIcon("");
	 * iconLabel.setHref(newHref);
	 * 
	 * KmlScreenOverlay newOverlay = GE.getPlugin().createScreenOverlay("");
	 * rectangleToUpdate.InitScreenOverlay(newOverlay);
	 * 
	 * newOverlay.setIcon(iconLabel);
	 * 
	 * if (overlayToUpdate == null)
	 * GE.getPlugin().getFeatures().appendChild(newOverlay); else
	 * GE.getPlugin().getFeatures().replaceChild(newOverlay, overlayToUpdate);
	 * 
	 * overlayToUpdate = newOverlay; } }
	 */
	@Override
	public void onTransitionStopped() {
	}

	boolean updateViewAfterCameraStopFlag = false;

	public boolean isSetEnabledScheduled() {
		return updateViewAfterCameraStopFlag;
	};

	public void setUpdateViewAfterCameraStopFlag(boolean on) {
		updateViewAfterCameraStopFlag = on;
	};
}
