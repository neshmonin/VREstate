package com.condox.vrestate.client.view.GeoItems;

import com.condox.vrestate.client.Position;

public interface IGeoItem {

	public abstract Position getPosition();

	public abstract String getName();
	
	public abstract int getParent_id();

	public abstract int getId();

	public abstract String getCaption();
}