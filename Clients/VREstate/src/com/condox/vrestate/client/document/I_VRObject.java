package com.condox.vrestate.client.document;

public interface I_VRObject {
	public enum VRObjectType
	{
		None,
		Suite,
		Building,
		Site
	};
	
	public VRObjectType getType();
	public int getId();
	public int getParent_id();
}
