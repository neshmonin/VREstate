package my.vrestate.client.Interactors;

import java.util.ArrayList;

import com.google.gwt.core.client.GWT;

import my.vrestate.client.Drawables.Button;
import my.vrestate.client.core.GEPlugin.IMouseEventListener;
import my.vrestate.client.core.GEPlugin.MouseEventData;

public class PanoramicViewInteractor extends Interactor implements IMouseEventListener{
	
	private Button ButtonPanoramicView;
	
	public PanoramicViewInteractor() {
		GWT.log("creating panoramic");
		ButtonPanoramicView = new Button(100,-20,164,23,"PanoramicViewButton.png");
		ButtonPanoramicView.setVisible(false);
	}
	
	@Override
	public void setVisible(boolean visible) {
		super.setVisible(visible);
		ButtonPanoramicView.setVisible(isEnabled() && isVisible());
	};

	@Override
	public void setEnabled(boolean enabled) {
		super.setEnabled(enabled);
		ButtonPanoramicView.setVisible(isEnabled() && isVisible());
	};
	
	@Override
	public void onMouseEvent(MouseEventData mouse_event_data) {
		if(isEnabled() && isVisible()) {
			switch(mouse_event_data.Type) {
			case ME_CLICK: {
				int x = mouse_event_data.X;
				int y = mouse_event_data.Y;
				if (ButtonPanoramicView.isProceed(x, y)) {
					firePanoramicView();
//					GWT.log("Proceed");
				}
				break;
			}
			}
		}
	}
	
	private ArrayList<IPanoramicViewListener> PanoramicViewListeners = new ArrayList<IPanoramicViewListener>();
	
	public void addPanoramicViewListener(IPanoramicViewListener listener) {
		this.PanoramicViewListeners.add(listener);
	}
	public void removePanoramicViewListener(IPanoramicViewListener listener) {
		this.PanoramicViewListeners.remove(listener);
	}
	private void firePanoramicView() {
		for (IPanoramicViewListener listener : PanoramicViewListeners)
			if (isEnabled() && isVisible()) listener.onPanoramicView();
	}


}
