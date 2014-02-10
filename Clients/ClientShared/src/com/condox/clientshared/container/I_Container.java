package com.condox.clientshared.container;

import com.google.gwt.user.client.ui.Composite;

public interface I_Container {
	public void add(I_Contained child);
	public void add(Composite child);
	public void clear();
}
