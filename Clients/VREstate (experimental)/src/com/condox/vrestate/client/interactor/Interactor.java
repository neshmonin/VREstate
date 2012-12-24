package com.condox.vrestate.client.interactor;

import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.View;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;

public abstract class Interactor implements IInteractor {

	// private View view = null;
	private String caption;
	private KmlScreenOverlay info_overlay = null;

	public Interactor(String caption) {
		// this.view = view;
		this.caption = caption;
	}

//	@Override
//	public void setEnabled(boolean value) {
//		if (value) {
//			KmlIcon icon = GE.getPlugin().createIcon("");
//			String href = Options.HOME_URL + "gen/txt?height=20&shadow=2&text="
//			// + view.getCaption()
//			+ caption 
//			+ "&txtClr=1677721565280&shdClr=0&frame=0";
//			icon.setHref(href);
//			info_overlay = GE.getPlugin().createScreenOverlay("");
//			info_overlay.setIcon(icon);
//			info_overlay.getOverlayXY().set(0.5, KmlUnits.UNITS_FRACTION, 75,
//					KmlUnits.UNITS_INSET_PIXELS);
//			info_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
//					KmlUnits.UNITS_FRACTION);
//			// info_overlay.getSize().set(100, KmlUnits.UNITS_PIXELS, 50,
//			// KmlUnits.UNITS_PIXELS);
//			GE.getPlugin().getFeatures().appendChild(info_overlay);
//		} else {
//			GE.getPlugin().getFeatures().removeChild(info_overlay);
//		}
//	}
}
