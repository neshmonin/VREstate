package com.condox.vrestate.maps.client;

import com.condox.clientshared.abstractview.I_Progress;
import com.google.gwt.user.client.Window;

public class MyProgress implements I_Progress {

	@Override
	public void SetupProgress(ProgressType label) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void UpdateProgress(double percent) {
		// TODO Auto-generated method stub
		Window.alert("Progress for " + percent + "percents.");
	}

	@Override
	public void CleanupProgress() {
		// TODO Auto-generated method stub
		
	}

}
