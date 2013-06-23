package com.condox.vrestate.shared;


public interface IGeoItem {

	public abstract Position getPosition();

	public abstract String getName();
	
	public abstract int getParent_id();

	public abstract int getId();

	public abstract String getType();

	public abstract String getCaption();
	
	public abstract String getInitialViewKml();
}