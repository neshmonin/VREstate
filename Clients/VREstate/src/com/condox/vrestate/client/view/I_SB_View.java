package com.condox.vrestate.client.view;

import com.condox.clientshared.abstractview.I_AbstractView;

public interface I_SB_View extends I_AbstractView {
	public abstract void Move(double dH, double dT, double dR);

	public abstract void Pan(double d);
}