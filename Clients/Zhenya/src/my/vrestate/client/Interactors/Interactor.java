package my.vrestate.client.Interactors;

import com.google.gwt.core.client.GWT;


public class Interactor implements IInteractor {
	
	private boolean Enabled = false;
	private boolean Visible = false;

	public void setEnabled(boolean enabled) {
		Enabled = enabled;
	}

	public boolean isEnabled() {
		return Enabled;
	};

	@Override
	public void setVisible(boolean visible) {
		Visible = visible;
	}

	public boolean isVisible() {
		return Visible;
	};
}
