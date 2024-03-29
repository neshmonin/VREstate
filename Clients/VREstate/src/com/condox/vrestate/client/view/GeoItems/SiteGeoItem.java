package com.condox.vrestate.client.view.GeoItems;

import com.condox.clientshared.abstractview.IGeoItem;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.document.Position;
import com.condox.clientshared.document.Site;

public class SiteGeoItem implements IGeoItem {
	private final double initialRange_m = 300;
	private final double initialTilt_d = 60;

	private Site site = null;

	public SiteGeoItem(Site site){
		this.site = site;
		Position position = site.getPosition();
		if (position != null) {
			position.setTilt(initialTilt_d);
			position.setRange(initialRange_m);
		}
	}
	
	@Override
	public Position getPosition() {
		return site.getPosition();
	}

	@Override
	public String getName() {
		return site.getName();
	}

	@Override
	public int getParent_id() {
		return site.getParent_id();
	}

	@Override
	public int getId() {
		return site.getId();
	}

	@Override
	public String getCaption() {
		if (!Options.isViewOrder())
			return site.getName();

		return "";
	}

	@Override
	public String getType() {
		return "site";
	}

	@Override
	public String getInitialViewKml() {
		// TODO Auto-generated method stub
		return null;
	}
}
