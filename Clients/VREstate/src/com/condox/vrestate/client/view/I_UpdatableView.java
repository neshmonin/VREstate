package com.condox.vrestate.client.view;

public interface I_UpdatableView {
	public abstract void onViewChanged(); // called at the end of each frame
	public abstract void onTransitionStopped(); // called at the end-of-all-changes
}