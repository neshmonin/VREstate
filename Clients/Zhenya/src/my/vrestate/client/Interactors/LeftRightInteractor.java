package my.vrestate.client.Interactors;

import java.util.ArrayList;

import my.vrestate.client.core.GEPlugin.IMouseEventListener;
import my.vrestate.client.core.GEPlugin.IPluginReadyListener;
import my.vrestate.client.core.GEPlugin.MouseEventData;

public class LeftRightInteractor extends Interactor implements IMouseEventListener{
	private boolean moving_started = false;
	private int last_x = 0;
	@Override
	public void onMouseEvent(MouseEventData mouse_event_data) {
		switch(mouse_event_data.Type) {
		case ME_DOWN: {
			this.last_x = mouse_event_data.X;
			this.moving_started = true;
			break;
		}
		case ME_MOVE: {
			int x = mouse_event_data.X;
			if (this.moving_started) {
				int dx = x - last_x;
				if (dx != 0)
					this.fireLeftRight(dx);
				last_x = x;
			}
			break;
		}
		case ME_UP: {
			this.moving_started = false;
			break;
		}
		}
	}
	
	private ArrayList<ILeftRightListener> LeftRightListeners = new ArrayList<ILeftRightListener>();
	
	public void addLeftRightListener(ILeftRightListener listener) {
		this.LeftRightListeners.add(listener);
	}
	public void removeLeftRightListener(ILeftRightListener listener) {
		this.LeftRightListeners.remove(listener);
	}
	private void fireLeftRight(double delta) {
		for (ILeftRightListener listener : LeftRightListeners)
			listener.onLeftRight(delta);
	}
}
