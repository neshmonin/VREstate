package my.vrestate.client.Interactors;

import java.util.ArrayList;

import com.google.gwt.core.client.GWT;

import my.vrestate.client.Drawables.Button;
import my.vrestate.client.core.GEPlugin.IMouseEventListener;
import my.vrestate.client.core.GEPlugin.MouseEventData;

public class BackViewInteractor extends Interactor implements IMouseEventListener{
	
	private Button ButtonBack;
	
	public BackViewInteractor() {
		ButtonBack = new Button(40,-20,59,23,"BackButton.png");
		ButtonBack.setVisible(true);
	}
	@Override
	public void setVisible(boolean visible) {
		super.setVisible(visible);
		ButtonBack.setVisible(isEnabled() && isVisible());
	}
	
	@Override
	public void setEnabled(boolean enabled) {
		super.setEnabled(enabled);
		ButtonBack.setVisible(isEnabled() && isVisible());
	};
	
	@Override
	public void onMouseEvent(MouseEventData mouse_event_data) {
		if(isEnabled() && isVisible()) {
			switch(mouse_event_data.Type) {
			case ME_CLICK: {
				int x = mouse_event_data.X;
				int y = mouse_event_data.Y;
				if (ButtonBack.isProceed(x, y)) {
					fireBackView();
				}
				break;
			}
			}
		}
	}
	
	private ArrayList<IBackViewListener> BackViewListeners = new ArrayList<IBackViewListener>();
	
	public void addBackViewListener(IBackViewListener listener) {
		this.BackViewListeners.add(listener);
	}
	public void removeBackViewListener(IBackViewListener listener) {
		this.BackViewListeners.remove(listener);
	}
	private void fireBackView() {
		for (IBackViewListener listener : BackViewListeners)
			if (isEnabled() && isVisible()) listener.onBackView();
	}


}
