package com.condox.vrestate.shared;

public interface I_VRObject {
	public enum VRObjectType {
		None,
		Suite,
		Building,
		Site
	};

	public VRObjectType getType();

	public int getId();

	public int getParent_id();

	public void setInfoUrl(String infoUrl);

	public String getInfoUrl();
}
