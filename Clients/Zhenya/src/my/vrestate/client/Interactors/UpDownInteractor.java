package my.vrestate.client.Interactors;

import java.util.ArrayList;

import my.vrestate.client.core.GEPlugin.IMouseEventListener;
import my.vrestate.client.core.GEPlugin.MouseEventData;

public class UpDownInteractor extends Interactor  implements IMouseEventListener{
	private boolean moving_started = false;
	private int last_y = 0;
	@Override
	public void onMouseEvent(MouseEventData mouse_event_data) {
		switch(mouse_event_data.Type) {
		case ME_DOWN: {
			this.last_y = mouse_event_data.Y;
			this.moving_started = true;
			break;
		}
		case ME_MOVE: {
			int y = mouse_event_data.Y;
			if (this.moving_started) {
				int dy = y - last_y;
				if (dy != 0)
					this.fireUpDown(dy);
				last_y = y;
			}
			break;
		}
		case ME_UP: {
			this.moving_started = false;
			break;
		}
		}
	}
	
	private ArrayList<IUpDownListener> UpDownListeners = new ArrayList<IUpDownListener>();
	
	public void addUpDownListener(IUpDownListener listener) {
		this.UpDownListeners.add(listener);
	}
	public void removeUpDownListener(IUpDownListener listener) {
		this.UpDownListeners.remove(listener);
	}
	private void fireUpDown(double delta) {
		for (IUpDownListener listener : UpDownListeners)
			listener.onUpDown(delta);
	}

}
