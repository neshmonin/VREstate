package com.condox.vrestate.client.view;

import com.condox.vrestate.client.view.Camera.Camera;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;

public interface I_AbstractView extends I_UpdatableView {
	public void Select(String type, int id);
	public void onHeadingChanged();
	abstract void setEnabled(boolean value);
	abstract void setupCamera(I_AbstractView poppedView); // first time must create a camera and assign it to _camera
	IGeoItem getGeoItem(); // access function, implemented in _AbstractView
	double getTransitionSpeed(); // access function, implemented in _AbstractView
	double getRegularSpeed(); // access function, implemented in _AbstractView
	Camera getCamera(); // access function, implemented in _AbstractView

	abstract void onDestroy();
	
	// the following three are implemented in _AbstractView
	public boolean isSetEnabledScheduled(); 
	public void scheduleSetEnabled();

	public abstract void doViewChanged(); // call this to force onViewChanged() in a tread-safe manner
}
