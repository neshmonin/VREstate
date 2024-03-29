package com.condox.vrestate.client.interactor;

import com.condox.clientshared.communication.Options;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.PanoramicView;
import com.condox.vrestate.client.view._AbstractView;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.shared.HandlerRegistration;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.event.MouseListener;

public class PanoramicInteractor extends OverlayHelpers implements
		MouseListener, I_AbstractInteractor {

	private HandlerRegistration mouse_listener = null;
	private PanoramicView view = null;

	KmlScreenOverlay back_overlay = null;
	KmlScreenOverlay center_overlay = null;
	private OvlRectangle backRect = null;
	private OvlRectangle centerRect = null;
	private InteractorMessages i18n;

	public PanoramicInteractor(PanoramicView view) {
		this.view = view;
		i18n = (InteractorMessages) GWT.create(InteractorMessages.class);
		backRect = new OvlRectangle(new OvlPoint(new OvlDimension(0.07f),
				new OvlDimension(0.9f)), new OvlDimension(0.1f),
				new OvlDimension(0.1f));

		centerRect = new OvlRectangle(new OvlPoint(new OvlDimension(0.93f),
				new OvlDimension(0.9f)), new OvlDimension(0.1f),
				new OvlDimension(0.1f));
	}

	private enum WhereIsMouse {
		OnBackButton, OnCenterButton, AnywhereElse
	}

	private WhereIsMouse HitTest(int x, int y) {
		if (backRect.ContainsPixel(x, y))
			return WhereIsMouse.OnBackButton;
		if (centerRect.ContainsPixel(x, y))
			return WhereIsMouse.OnCenterButton;

		return WhereIsMouse.AnywhereElse;
	}

	@Override
	public void setEnabled(boolean enabling) {
		// Log.write("PanoramicInteractor: setEnabled = " + enabling);
		if (enabling) {
			if (mouse_listener == null)
				mouse_listener = GE.getPlugin().getWindow()
						.addMouseListener(this);
		} else {
			mouse_listener.removeHandler();
			mouse_listener = null;
		}

		if (enabling) {
			if (back_overlay == null) {
				KmlIcon icon = GE.getPlugin().createIcon("");
				String href = i18n
						.URL_BUTTON_EXIT_PANORAMIC_VIEW(Options.URL_BUTTONS);
				icon.setHref(href);
				back_overlay = GE.getPlugin().createScreenOverlay("");
				back_overlay.setIcon(icon);

				// Set the ScreenOverlay's position and size
				backRect.InitScreenOverlay(back_overlay);

				GE.getPlugin().getFeatures().appendChild(back_overlay);
			}
			back_overlay.setVisibility(enabling);

			if (center_overlay == null) {
				KmlIcon icon = GE.getPlugin().createIcon("");
				String href = i18n
						.URL_BUTTON_CENTER_PANORAMIC_VIEW(Options.URL_BUTTONS);
				icon.setHref(href);
				center_overlay = GE.getPlugin().createScreenOverlay("");
				center_overlay.setIcon(icon);

				// Set the ScreenOverlay's position and size
				centerRect.InitScreenOverlay(center_overlay);

				GE.getPlugin().getFeatures().appendChild(center_overlay);
			}
			center_overlay.setVisibility(enabling);
		} else // disabling
		{
			if (back_overlay != null) {
				back_overlay.setVisibility(enabling);
				GE.getPlugin().getFeatures().removeChild(back_overlay);
			}

			if (center_overlay != null) {
				center_overlay.setVisibility(enabling);
				GE.getPlugin().getFeatures().removeChild(center_overlay);
			}
		}
	}

	/* ================================================ */
	private int x = 0;
	private int y = 0;
	boolean moving = false;

	@Override
	public void onClick(KmlMouseEvent event) {
		event.preventDefault();

		switch (HitTest(x, y)) {
		case OnBackButton:
			_AbstractView.Pop();
			break;
		case OnCenterButton:
			view.Center();
			break;
		default:
			break;
		}
	}

	@Override
	public void onDoubleClick(KmlMouseEvent event) {
	}

	@Override
	public void onMouseDown(KmlMouseEvent event) {
		_AbstractView.ResetTimeOut();
		event.preventDefault();
		x = event.getClientX();
		y = event.getClientY();
		moving = true;
	}

	@Override
	public void onMouseUp(KmlMouseEvent event) {
		_AbstractView.ResetTimeOut();
		event.preventDefault();
		moving = false;
	}

	@Override
	public void onMouseOver(KmlMouseEvent event) {
	}

	@Override
	public void onMouseOut(KmlMouseEvent event) {
	}

	@Override
	public void onMouseMove(KmlMouseEvent event) {
		event.preventDefault();
		if (moving) {
			switch (event.getButton()) {
			case 0: // LEFT
				double dX = event.getClientX() - x;
				double dY = event.getClientY() - y;
				view.Move(dX, dY);
				x += dX;
				y += dY;
				break;
			}
		}
	}
}
