package com.condox.vrestate.client.view;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.ViewOrder.ProductType;
import com.condox.vrestate.client.filter.Filter;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlLookAt;

public abstract class _GEView extends _AbstractView {

	public _GEView(IGeoItem item) {
		theGeoItem = item;
	}

	@Override
	public void onHeadingChanged() {
		double heading_d = getHeading();
		for (SuiteGeoItem suiteGeo : _AbstractView.getSuiteGeoItems())
			suiteGeo.onHeadingChanged(heading_d);
	}

	private double getHeading() {
		KmlLookAt look_at = GE.getView().copyAsLookAt(
				KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
		if (look_at != null)
			return look_at.getHeading();
		return 0;
	};
}
