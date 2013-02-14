package com.condox.vrestate.client.view.GeoItems;

import com.condox.vrestate.client.Position;
import com.condox.vrestate.client.document.Site;

public class SiteGeoItem implements IGeoItem {

	private Site site = null;

	public SiteGeoItem(Site site){
		this.site = site;
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
		return site.getName();
	}

	@Override
	public String getType() {
		return "site";
	}
}
