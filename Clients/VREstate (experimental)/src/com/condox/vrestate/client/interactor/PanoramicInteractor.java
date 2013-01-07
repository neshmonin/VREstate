package com.condox.vrestate.client.interactor;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.PanoramicView;
import com.google.gwt.event.shared.HandlerRegistration;
import com.google.gwt.user.client.Window;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;
import com.nitrous.gwt.earth.client.api.event.FrameEndListener;
import com.nitrous.gwt.earth.client.api.event.MouseListener;
import com.nitrous.gwt.earth.client.api.event.ViewChangeListener;

public class PanoramicInteractor extends Interactor {

	private PanoramicView view = null;

	private KmlScreenOverlay back_overlay = GE.getPlugin().createScreenOverlay(
			"");
	private KmlScreenOverlay center_overlay = GE.getPlugin()
			.createScreenOverlay("");

	public PanoramicInteractor(PanoramicView view) {
		super(view.getCaption());
		this.view = view;
		//
		KmlIcon icon = GE.getPlugin().createIcon("");
		String href = Options.URL_BUTTON_EXIT_PANORAMIC_VIEW;
		icon.setHref(href);
		back_overlay = GE.getPlugin().createScreenOverlay("");
		back_overlay.setIcon(icon);
		back_overlay.getOverlayXY().set(100, KmlUnits.UNITS_PIXELS, 75,
				KmlUnits.UNITS_INSET_PIXELS);
		back_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
				KmlUnits.UNITS_FRACTION);
		back_overlay.getSize().set(150, KmlUnits.UNITS_PIXELS, 75,
				KmlUnits.UNITS_PIXELS);
		GE.getPlugin().getFeatures().appendChild(back_overlay);

		KmlIcon icon2 = GE.getPlugin().createIcon("");
		String href2 = Options.URL_BUTTON_CENTER_PANORAMIC_VIEW;
		icon2.setHref(href2);
		center_overlay = GE.getPlugin().createScreenOverlay("");
		center_overlay.setIcon(icon2);
		center_overlay.getOverlayXY().set(100, KmlUnits.UNITS_INSET_PIXELS, 75,
				KmlUnits.UNITS_INSET_PIXELS);
		center_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
				KmlUnits.UNITS_FRACTION);
		center_overlay.getSize().set(150, KmlUnits.UNITS_PIXELS, 75,
				KmlUnits.UNITS_PIXELS);
		GE.getPlugin().getFeatures().appendChild(center_overlay);
	}

	private double dH = 0;
	private double dT = 0;

	private HandlerRegistration mouse_listener = null;
	private HandlerRegistration view_listener = null;
	private HandlerRegistration frame_listener = null;

	@Override
	public void setEnabled(boolean value) {
		Log.write("PanoramicInteractor:" + value);
		back_overlay.setVisibility(value);
		center_overlay.setVisibility(value);
		if (value) {
			if (mouse_listener == null)
				mouse_listener = GE.getPlugin().getWindow()
						.addMouseListener(new MouseListener() {

							private int x = 0;
							private int y = 0;
							boolean moving = false;
							boolean clicking = false;

							@Override
							public void onClick(KmlMouseEvent event) {
								event.preventDefault();
								Log.write("PanoramicInteractor::onClick()");
								// Log.write("1");
								if ((50 < event.getClientX())
										&& (event.getClientX() < 200)
										&& (50 < event.getClientY())
										&& (event.getClientY() < 125))
									view.Pop();
								else if ((GE.getEarth().getOffsetWidth() - 200 < event
										.getClientX())
										&& (event.getClientX() < GE.getEarth()
												.getOffsetWidth() - 50)
										&& (50 < event.getClientY())
										&& (event.getClientY() < 125))
									view.Center();
							}

							@Override
							public void onDoubleClick(KmlMouseEvent event) {
								// Log.write("2");
							}

							@Override
							public void onMouseDown(KmlMouseEvent event) {
								event.preventDefault();
								// Log.write("3");
								x = event.getClientX();
								y = event.getClientY();
								moving = true;
							}

							@Override
							public void onMouseUp(KmlMouseEvent event) {
								event.preventDefault();
								// Log.write("4");
								moving = false;
								view.Draw();

								// Click(event.getClientX(),
								// event.getClientY());

							}

							@Override
							public void onMouseOver(KmlMouseEvent event) {
								// Log.write("5");
							}

							@Override
							public void onMouseOut(KmlMouseEvent event) {
								// Log.write("6");
							}

							@Override
							public void onMouseMove(KmlMouseEvent event) {
								event.preventDefault();
								Log.write("7");
								if (moving) {
									switch (event.getButton()) {
									case 0: // LEFT
										double dX = event.getClientX() - x;
										double dY = event.getClientY() - y;
										// view.ChangeHeading(dX);
										// view.ChangeTilt(dY);
										view.Move(dX, dY);
										// dH += dX;
										// dT += dY;
										// Log.write("dH: " + dH);
										// Log.write("dT: " + dT);
										x += dX;
										y += dY;
										break;
									}
								}
								clicking = false;
							}
						});
		} else {
			// Log.write("1");
			mouse_listener.removeHandler();
			mouse_listener = null;
			// Log.write("2");
		}

		if (value) {
			if (view_listener == null)
				view_listener = GE.getView().addViewChangeListener(
						new ViewChangeListener() {
							
							@Override
							public void onViewChangeBegin() {
							}

							@Override
							public void onViewChange() {
							}

							@Override
							public void onViewChangeEnd() {
							}
						});
		} else {
			view_listener.removeHandler();
			view_listener = null;
		}
		//
		// if (value) {
		// if (frame_listener == null)
		// frame_listener = GE.getPlugin().addFrameEndListener(
		// new FrameEndListener() {
		//
		// @Override
		// public void onFrameEnd() {
		// Log.write("A");
		// // // dH *= 0.8;
		// // // dT /= 2;
		// // if ((dH > 0.05)||(dT > 0.05))
		// // if ((Math.abs(dH) > 0.5)||(Math.abs(dH) >
		// // 0.5))
		// // {
		// // //// dH -= 1;
		// // //// dT -= 1;
		// // view.Move(dH, dT);
		// // dH = 0;
		// // dT = 0;
		// // }
		// }
		// });
		// } else {
		// frame_listener.removeHandler();
		// frame_listener = null;
		// }
	}

	// void Click(int x, int y) {
	// // Window.alert("x: " + x + " y: " + y);
	// if ((50 < x) && (x < 150) && (50 < y) && (y < 100))
	// view.Pop();
	// if ((GE.getEarth().getOffsetWidth() - 150 < x)
	// && (x < GE.getEarth().getOffsetWidth() - 50) && (50 < y)
	// && (y < 100)) {
	// view.getCamera().setHeading(
	// view.selection.getPosition().getHeading());
	// view.getCamera().setTilt(90);
	// view.UpdateCamera();
	// }
	// };
	//
	// void Update(double h, double t) {
	// double heading = view.getCamera().getHeading();
	// heading += dH;
	// view.getCamera().setHeading(heading);
	// view.UpdateCamera(-1);
	// }
	//
	// back_overlay = GE.getPlugin().createScreenOverlay("");
	// KmlIcon back_icon = GE.getPlugin().createIcon("");
	// back_icon.setHref(Options.URL_BUTTON_EXIT_PANORAMIC_VIEW);
	// back_overlay.setIcon(back_icon);
	// back_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
	// KmlUnits.UNITS_FRACTION);
	//
	// back_overlay.getOverlayXY().setXUnits(KmlUnits.UNITS_PIXELS);
	// back_overlay.getOverlayXY().setX(100);
	//
	// back_overlay.getOverlayXY().setYUnits(KmlUnits.UNITS_INSET_PIXELS);
	// back_overlay.getOverlayXY().setY(75);
	//
	// back_overlay.getSize().setX(100);
	// back_overlay.getSize().setY(50);
	// GE.getPlugin().getFeatures().appendChild(back_overlay);
	//
	// center_overlay = GE.getPlugin().createScreenOverlay("");
	// KmlIcon suite_name_icon = GE.getPlugin().createIcon("");
	// suite_name_icon.setHref(Options.URL_BUTTON_CENTER_PANORAMIC_VIEW);
	// center_overlay.setIcon(suite_name_icon);
	// center_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
	// KmlUnits.UNITS_FRACTION);
	//
	// center_overlay.getOverlayXY().setXUnits(KmlUnits.UNITS_INSET_PIXELS);
	// center_overlay.getOverlayXY().setX(100);
	//
	// center_overlay.getOverlayXY().setYUnits(KmlUnits.UNITS_INSET_PIXELS);
	// center_overlay.getOverlayXY().setY(75);
	//
	// center_overlay.getSize().setX(100);
	// center_overlay.getSize().setY(50);
	// GE.getPlugin().getFeatures().appendChild(center_overlay);
}
