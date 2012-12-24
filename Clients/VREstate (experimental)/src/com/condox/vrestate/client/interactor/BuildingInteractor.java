package com.condox.vrestate.client.interactor;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Options;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.BuildingView;
import com.condox.vrestate.client.view.SiteView;
import com.google.gwt.event.shared.HandlerRegistration;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Timer;
import com.nitrous.gwt.earth.client.api.KmlIcon;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;
import com.nitrous.gwt.earth.client.api.event.FrameEndListener;
import com.nitrous.gwt.earth.client.api.event.MouseListener;
import com.nitrous.gwt.earth.client.api.event.ViewChangeListener;

public class BuildingInteractor extends Interactor {

	private BuildingView view = null;
	private HandlerRegistration mouse_listener = null;
	private HandlerRegistration view_listener = null;
	private HandlerRegistration frame_listener = null;
	KmlScreenOverlay zoom_overlay = null;
	KmlScreenOverlay unzoom_overlay = null;

	public BuildingInteractor(BuildingView view) {
		super(view.getCaption());
		this.view = view;
	}

	@Override
	public void setEnabled(boolean value) {
		Log.write("BuildingInteractor:" + value);
		if (value) {
			if (mouse_listener == null)
				mouse_listener = GE.getPlugin().getWindow()
						.addMouseListener(new MouseListener() {

							private int x = 0;
							private int y = 0;
							private int z = 0;
							boolean action = false;
							Timer zoomer = new Timer() {

								@Override
								public void run() {
									// TODO Auto-generated method stub
									ChangeRange(10);
								}
							};

							Timer unzoomer = new Timer() {

								@Override
								public void run() {
									ChangeRange(-10);
								}
							};

							@Override
							public void onClick(KmlMouseEvent event) {
								event.preventDefault();
								Log.write("BuildingInteractor::onClick()");

								if (event.getTarget().getType()
										.equals("KmlPlacemark")) {
									KmlPlacemark placemark = (KmlPlacemark) event
											.getTarget();
									String json = placemark.getSnippet();
									JSONObject obj = JSONParser.parseLenient(
											json).isObject();
									// TODO Добавить проверку obj на null
									String type = obj.get("type").isString()
											.stringValue();
									int id = (int) obj.get("id").isNumber()
											.doubleValue();
									Log.write("" + obj.toString());
									 view.Select(type, id);
								}
								// Изменить!!
								// view.Select(null, 0);
							}

							@Override
							public void onDoubleClick(KmlMouseEvent event) {
							}

							@Override
							public void onMouseDown(KmlMouseEvent event) {
								event.preventDefault();
								x = event.getClientX();
								y = event.getClientY();
								z = event.getClientY();
								action = true;

								if ((50 < x)
										&& (x < 150)
										&& (GE.getEarth().getOffsetHeight() - 150 < y)
										&& (y < GE.getEarth().getOffsetHeight() - 50))
									zoomer.scheduleRepeating(1);
								if ((50 < x)
										&& (x < 150)
										&& (GE.getEarth().getOffsetHeight() - 300 < y)
										&& (y < GE.getEarth().getOffsetHeight() - 200))
									unzoomer.scheduleRepeating(1);
//								Log.write(x + " " + y);

							}

							@Override
							public void onMouseUp(KmlMouseEvent event) {
								event.preventDefault();
								action = false;
								zoomer.cancel();
								unzoomer.cancel();
								view.Draw();
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
								if (action) {
									switch (event.getButton()) {
									case 0: // LEFT
										double dX = event.getClientX() - x;
										double dY = event.getClientY() - y;
//										view.Move(dX, dY, 0);
//										view.Update();
										view.addHeadingDiff(dX);
										view.addTiltDiff(dY);
										x += dX;
										y += dY;
										break;
									case 2: // RIGHT
										double dZ = event.getClientY() - z;
//										ChangeRange(-dZ);
										view.addRangeDiff(-dZ);
										z += dZ;
										break;
									}
								}
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
								// TODO Auto-generated method stub

							}

							@Override
							public void onViewChange() {
								// TODO Auto-generated method stub

							}

							// private double centerX =
							// GE.getEarth().getOffsetWidth() / 2;
							// private double centerY = GE.getEarth()
							// .getOffsetHeight() / 2;

							@Override
							public void onViewChangeEnd() {
							}
						});
		} else {
			view_listener.removeHandler();
			view_listener = null;
		}
		
		if (value) {
			if (frame_listener == null)
				frame_listener = GE.getPlugin().addFrameEndListener(
						new FrameEndListener() {

							@Override
							public void onFrameEnd() {
								// TODO Auto-generated method stub
								frame_ended = true;
								view.onFrameEnd();
							}
						});
		} else {
			frame_listener.removeHandler();
			frame_listener = null;
		}
		

		if (zoom_overlay == null) {
			KmlIcon icon = GE.getPlugin().createIcon("");
			String href = Options.ZOOM_IN_URL;
			icon.setHref(href);
			zoom_overlay = GE.getPlugin().createScreenOverlay("");
			zoom_overlay.setIcon(icon);
			zoom_overlay.getOverlayXY().set(100, KmlUnits.UNITS_PIXELS, 100,
					KmlUnits.UNITS_PIXELS);
			zoom_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
					KmlUnits.UNITS_FRACTION);
			zoom_overlay.getSize().set(100, KmlUnits.UNITS_PIXELS, 100,
					KmlUnits.UNITS_PIXELS);
			GE.getPlugin().getFeatures().appendChild(zoom_overlay);
		}
		zoom_overlay.setVisibility(value);

		if (unzoom_overlay == null) {
			KmlIcon icon = GE.getPlugin().createIcon("");
			String href = Options.ZOOM_OUT_URL;
			icon.setHref(href);
			unzoom_overlay = GE.getPlugin().createScreenOverlay("");
			unzoom_overlay.setIcon(icon);
			unzoom_overlay.getOverlayXY().set(100, KmlUnits.UNITS_PIXELS, 250,
					KmlUnits.UNITS_PIXELS);
			unzoom_overlay.getScreenXY().set(0.5, KmlUnits.UNITS_FRACTION, 0.5,
					KmlUnits.UNITS_FRACTION);
			unzoom_overlay.getSize().set(100, KmlUnits.UNITS_PIXELS, 100,
					KmlUnits.UNITS_PIXELS);
			GE.getPlugin().getFeatures().appendChild(unzoom_overlay);
		}
		unzoom_overlay.setVisibility(value);

	}
	
	private boolean frame_ended = false;
	private void ChangeRange(double dR) {
		if (frame_ended) {
			view.Move(0, 0, dR);
			view.Update();
			frame_ended = false;
		}
	}
}
