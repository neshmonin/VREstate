package com.condox.clientshared.abstractview;

public interface I_UpdatableView {
	public abstract void onViewChanged(); // called at the end of each frame, called in tread-safe manner
	public abstract void onTransitionStopped(); // called at the end-of-all-changes
}