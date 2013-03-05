package com.condox.vrestate.client.interactor;

import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.HelicopterView;
import com.google.gwt.event.shared.HandlerRegistration;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.event.MouseListener;

public class SingleTouchInteractor implements MouseListener,
	I_AbstractInteractor {

	private HelicopterView view = null;

	public SingleTouchInteractor(HelicopterView view) {
		this.view = view;
	}

	@Override
	public void onClick(KmlMouseEvent event) {
		event.preventDefault();
		view.pushNextView();
	}

	@Override	public void onDoubleClick(KmlMouseEvent event) {}
	@Override	public void onMouseDown(KmlMouseEvent event) {}
	@Override	public void onMouseUp(KmlMouseEvent event) {}
	@Override	public void onMouseOver(KmlMouseEvent event) {}
	@Override	public void onMouseOut(KmlMouseEvent event) {}
	@Override	public void onMouseMove(KmlMouseEvent event) {}

	private HandlerRegistration mouse_listener = null;

	@Override
	public void setEnabled(boolean enabling) {
		if (enabling) {
			if (mouse_listener == null)
				mouse_listener = GE.getPlugin().getWindow()
						.addMouseListener(this);
		} else {
			mouse_listener.removeHandler();
			mouse_listener = null;
		}
	}
}
