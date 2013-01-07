package com.condox.vrestate.client.interactor;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.ge.GE;
import com.condox.vrestate.client.view.SuiteView;
import com.google.gwt.event.shared.HandlerRegistration;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.event.MouseListener;
import com.nitrous.gwt.earth.client.api.event.ViewChangeListener;

public class SuiteInteractor extends Interactor {

	private SuiteView view = null;

	public SuiteInteractor(SuiteView view) {
		super(view.getCaption());
		this.view = view;
	}

	private HandlerRegistration mouse_listener = null;
	private HandlerRegistration view_listener = null;

	@Override
	public void setEnabled(boolean value) {
		Log.write("SuiteInteractor:" + value);
		if (value) {
			if (mouse_listener == null)
			mouse_listener = GE.getPlugin().getWindow()
					.addMouseListener(new MouseListener() {

						private int x = 0;
						private int y = 0;
						private int z = 0;
						boolean action = false;

						@Override
						public void onClick(KmlMouseEvent event) {
							event.preventDefault();
							Log.write("SuiteInteractor::onClick()");
							
							if (event.getTarget().getType().equals("KmlPlacemark")) {
								KmlPlacemark placemark = (KmlPlacemark) event.getTarget();
								String json = placemark.getSnippet();
								Log.write(json);
								JSONObject obj = JSONParser.parseLenient(json).isObject();
								// TODO Добавить проверку obj на null
								String type = obj.get("type").isString().stringValue();
								int id = (int) obj.get("id").isNumber().doubleValue();
								view.Select(type, id);
							} else
							view.Select(null, 0);
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
						}

						@Override
						public void onMouseUp(KmlMouseEvent event) {
							event.preventDefault();
							action = false;
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
//									view.UpdateLookAt(dX, dY, 0);
									x += dX;
									y += dY;
									break;
								case 2: // RIGHT
									double dZ = event.getClientY() - z;
//									view.UpdateLookAt(0, 0, dZ);
									z += dZ;
									break;
								}
							}
						}
					});
		} else {
			Log.write("1");
			mouse_listener.removeHandler();
			mouse_listener = null;
			Log.write("2");
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

//						private double centerX = GE.getEarth().getOffsetWidth() / 2;
//						private double centerY = GE.getEarth()
//								.getOffsetHeight() / 2;

						@Override
						public void onViewChangeEnd() {
//							view.Update();
							Log.write("Here!");
							if (view_listener != null) {
								view.Draw();
								((SuiteView)view).ShowMoreInfo();
							}
						}
					});
		} else {
			view_listener.removeHandler();
			view_listener = null;
		}
	}
}
