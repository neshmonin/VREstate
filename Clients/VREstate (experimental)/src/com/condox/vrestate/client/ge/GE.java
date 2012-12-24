package com.condox.vrestate.client.ge;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.VRE;
import com.google.gwt.event.logical.shared.ResizeEvent;
import com.google.gwt.event.logical.shared.ResizeHandler;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.RootLayoutPanel;
import com.nitrous.gwt.earth.client.api.GEHtmlStringBalloon;
import com.nitrous.gwt.earth.client.api.GELayerId;
import com.nitrous.gwt.earth.client.api.GEPlugin;
import com.nitrous.gwt.earth.client.api.GEPluginReadyListener;
import com.nitrous.gwt.earth.client.api.GEView;
import com.nitrous.gwt.earth.client.api.GEVisibility;
import com.nitrous.gwt.earth.client.api.GoogleEarth;
import com.nitrous.gwt.earth.client.api.GoogleEarthWidget;
import com.nitrous.gwt.earth.client.api.KmlLookAt;
import com.nitrous.gwt.earth.client.api.KmlMouseEvent;
import com.nitrous.gwt.earth.client.api.KmlObject;
import com.nitrous.gwt.earth.client.api.KmlPlacemark;
import com.nitrous.gwt.earth.client.api.event.FrameEndListener;
import com.nitrous.gwt.earth.client.api.event.MouseListener;
import com.nitrous.gwt.earth.client.api.event.ViewChangeListener;

public class GE {

	private static GoogleEarthWidget earth = null;
	private static GEPlugin plugin = null;
	
	private static boolean isReady = false;
	
	public static boolean isReady() {
		return isReady;
	}

	// =======================================================
	public static GoogleEarthWidget getEarth() {
		return earth;
	};

	public static GEPlugin getPlugin() {
		return plugin;
	};

	public static GEView getView() {
		if (getPlugin() != null)
			return getPlugin().getView();
		else
			return null;
	};

	public static KmlObject parseKml(String kml) {
		if (getPlugin() != null)
			return getPlugin().parseKml(kml);
		else
			return null;
	};
//=====================================================
	public static double getRange(KmlLookAt LookAt) {
		try {
			return LookAt.getRange();
		} catch (Exception e) {
			return getRange(LookAt);
			// e.printStackTrace();
		}
	};

	public static double getTilt(KmlLookAt LookAt) {
		try {
			return LookAt.getTilt();
		} catch (Exception e) {
			return getTilt(LookAt);
			// e.printStackTrace();
		}
	};

	public static double getHeading(KmlLookAt LookAt) {
		try {
			return LookAt.getHeading();
		} catch (Exception e) {
			return getHeading(LookAt);
			// e.printStackTrace();
		}
	};

	public static void appendChild(KmlObject object) {
		try {
			getPlugin().getFeatures().appendChild(object);
		} catch (Exception e) {
			appendChild(object);
			// e.printStackTrace();
		}
	};

	public static void replaceChild(KmlObject newChild, KmlObject oldChild) {
		try {
			getPlugin().getFeatures().replaceChild(newChild, oldChild);
		} catch (Exception e) {
			replaceChild(newChild, oldChild);
			// e.printStackTrace();
		}
	};

	// =====================================================

	final private String EARTH_API_KEY = "ABQIAAAAm7LIvLNR-PkJLewH4qmS7hREGtQZq9OFJfHndXhPP8gxXzlLARQtA_EfZjc9zs77WO25FrLcaZ4ZVA";

	public void Init() {
		GoogleEarth.loadApi(EARTH_API_KEY, new Runnable() {

			@Override
			public void run() {
				earth = new GoogleEarthWidget();

				earth.addPluginReadyListener(new GEPluginReadyListener() {
					public void pluginReady(GEPlugin ge) {
						// show map content once the plug-in has loaded
						plugin = earth.getGEPlugin();
						plugin.getWindow().setVisibility(true);

						// add a navigation control
//						if (VRE.OPTIONS_NAVIGATION_CONTROLS)
							plugin.getNavigationControl().setVisibility(
									GEVisibility.VISIBILITY_AUTO);
//						else
//							plugin.getNavigationControl().setVisibility(
//									GEVisibility.VISIBILITY_HIDE);

						plugin.getOptions().setMouseNavigationEnabled(false);
						// show some layers
						
						plugin.enableLayer(GELayerId.LAYER_BUILDINGS, true);
						plugin.enableLayer(GELayerId.LAYER_BORDERS, true);
						plugin.enableLayer(GELayerId.LAYER_ROADS, true);
						plugin.enableLayer(GELayerId.LAYER_TERRAIN, true);
						plugin.enableLayer(GELayerId.LAYER_TREES, true);
						// ========================
//						setupEvents();
						isReady = true;
					}

					public void pluginInitFailure() {
						// failure!
						Window.alert("Failed to initialize Google Earth Plug-in");
					}
				});

				earth.setStyleName("map3d");
				earth.setSize("100%", "100%");
				 RootLayoutPanel.get().add(earth);
				// VREstate.spEarthContainer.add(elevator);
//				VREstate.spEarthContainer.add(earth);
				// earth.setSize("50%", "100px");
				earth.init();
			};
		});
	};

	private Timer zoomTimer = new Timer() {

		@Override
		public void run() {
//			if (View.getCurrView() != null)
//				View.getCurrView().onZoom(0.1);
		}
	};

	private Timer unzoomTimer = new Timer() {

		@Override
		public void run() {
//			if (View.getCurrView() != null)
//				View.getCurrView().onZoom(-0.1);
		}
	};

	private MouseListener mouseListener = new MouseListener() {
		int x = 0;
		int y = 0;
		boolean moving = false;
		boolean zooming = false;
		boolean clicking = false;

		@Override
		public void onClick(KmlMouseEvent event) {
//			Log.write("GE::MouseClick");
//			// event.preventDefault();
//			Log.write("Interactor: onClick()");

			x = event.getClientX();
			y = event.getClientY();
//			int button = event.getButton();
//			if (View.getCurrView() != null) {
//				Log.write("View.getCurrView()!=null");
//				if (!View.getCurrView().onClick(x, y, button))
//					if (event.getTarget().getType().equals("KmlPlacemark")) {
//						Log.write("event.getTarget().getType().equals(\"KmlPlacemark\")");
//						KmlPlacemark placemark = (KmlPlacemark) event
//								.getTarget();
//						View.getCurrView().onClick(placemark.getId());
//					}
//			}
//			;
		}

		@Override
		public void onDoubleClick(KmlMouseEvent event) {
		}

		@Override
		public void onMouseDown(KmlMouseEvent event) {
			moving = true;
			clicking = true;
//			Log.write("MouseDown");
//			// event.preventDefault();
			x = event.getClientX();
			y = event.getClientY();
//			Viewable.Down(x, y);
//			Region.getSelected().view.onDown(x, y);
//			if (View.btnZoomIn.check(x, y)) {
//				zoomTimer.scheduleRepeating(100);
//				clicking = false;
//			} else if (View.btnZoomOut.check(x, y)) {
//				unzoomTimer.scheduleRepeating(100);
//				clicking = false;
//			} else {
//				zoomTimer.cancel();
//				unzoomTimer.cancel();
//				if (event.getButton() == 0) {
//					moving = true;
//					zooming = false;
//				} else if (event.getButton() == 2) {
//					moving = false;
//					zooming = true;
//				}
//				;
//			}
			// if (View.getCurrView() != null)
			// View.getCurrView().step();
		}

		@Override
		public void onMouseUp(KmlMouseEvent event) {
//			Suite.DrawPlacemarks();
			if (clicking) {
				if (event.getTarget().getType().equals("KmlPlacemark")) {
					KmlPlacemark placemark = (KmlPlacemark) event.getTarget();
//					Viewable.Get(placemark.getId()).get(0).Select();
				};
			}
				zoomTimer.cancel();
				unzoomTimer.cancel();
				moving = false;
				clicking = false;
				zooming = false;
//				Viewable.Up();
				//			Region.getSelected().view.onUp();
				//			if (!clicking)
				//				User.get().Draw();
		}

		@Override
		public void onMouseOver(KmlMouseEvent event) {
		}

		@Override
		public void onMouseOut(KmlMouseEvent event) {
		}

		@Override
		public void onMouseMove(KmlMouseEvent event) {
			clicking = false;
			event.preventDefault();
			if (moving) {
				double dx = (double) (event.getClientX() - x);
				double dy = (double) (event.getClientY() - y);
				// TODO В будущем обратить сюда внимание
				if (Math.abs(dx) > 0) {
					double width = (double) earth.getOffsetWidth();
					dx = dx / width;
					x = event.getClientX();
				}
				if (Math.abs(dy) > 0) {
					double height = (double) earth.getOffsetHeight();
					dy = dy / height;
					y = event.getClientY();
				}
//				Viewable.Drag(dx, dy);
//				Region.getSelected().view.onDrag(dx,dy);

			}
			;
			if (zooming) {
				double dy = (double) (event.getClientY() - y);
				if (Math.abs(dy) > 0) {
					double height = (double) earth.getOffsetHeight();
					dy = dy / height;
					y = event.getClientY();
//					if (View.getCurrView() != null)
//						View.getCurrView().onZoom(dy);
				}
				;

			}
			;
		};
	};

	private boolean eventsRegistered = false;

	public void setupEvents() {
		if (!eventsRegistered) {
//			earth.getGEPlugin().getWindow().addMouseListener(mouseListener);

			earth.getGEPlugin().addFrameEndListener(new FrameEndListener() {
				@Override
				public void onFrameEnd() {
//					Region.getSelected().UpdateViewPosition();
//					View.onFrame();
//					Filter.get().UpdateSize();
				};
			});
			earth.getGEPlugin().getView().addViewChangeListener(new ViewChangeListener() {
				Timer CheckViewEnd = new Timer() {
					@Override
					public void run() {
//						Viewable.ViewEnd();
						
					}};
				@Override
				public void onViewChangeBegin() {
					// TODO Auto-generated method stub
					CheckViewEnd.cancel();
				}

				@Override
				public void onViewChange() {
					CheckViewEnd.cancel();
				}

				@Override
				public void onViewChangeEnd() {
					// TODO Auto-generated method stub
//					CheckViewEnd.cancel();
//					CheckViewEnd.schedule(50);
				}});

			Window.addResizeHandler(new ResizeHandler() {

				Timer resizeTimer = new Timer() {
					@Override
					public void run() {
//						if (View.getCurrView() != null)
//							View.getCurrView().onResize();
					}
				};

				@Override
				public void onResize(ResizeEvent event) {
//					Region.getSelected().view.onResize();
//					resizeTimer.cancel();
//					resizeTimer.schedule(250);
				}
			});
			eventsRegistered = true;
		}
		;
	};

//	public void ShowSuiteInfo(Suite suite) {
//		Log.write("Interactor: ShowSuiteInfo()");
//		long time = System.currentTimeMillis();
//		GEPlugin plugin = earth.getGEPlugin();
//
//		// Оригинальный баббл
//		 GEHtmlStringBalloon balloon = plugin.createHtmlStringBalloon("");
////		 balloon.setFeature((KmlFeature)plugin.getElementById(suite.getId())); // optional
////		 Log.write("feature: " + balloon.getFeature().getKml());
//		 String content = suite.getContent();
////		 balloon.setContentString(suite.getContent());
////		 Window.alert(content);
//		 balloon.setContentString(content);
//		 balloon.setCloseButtonEnabled(false);
//
//		// Опасный баббл
////		GEHtmlDivBalloon balloon = plugin.createHtmlDivBalloon("");
////		
////		Frame frame = new Frame();
////		frame.setSize("1000px", "1000px");
////		frame.setUrl("http://unity3d.com/gallery/demos/live-demos#butterfly");
////		balloon.setContentDiv("");
////		((Element)balloon.getContentDiv()).appendChild(frame.getElement());
////		balloon.setMinHeight(1000);
////		balloon.setMinWidth(1000);
//		plugin.setBalloon(balloon);
//	};

	public void HideSuiteInfo() {
		long time = System.currentTimeMillis();
		GEPlugin plugin = earth.getGEPlugin();
		plugin.setBalloon(null);
		time = System.currentTimeMillis() - time;
		// Log.write("Hide suite info in "+time+" ms.");
	};

	private boolean HelpVisible = false;

	public void ToggleHelpVisible(String content) {

		GEPlugin plugin = earth.getGEPlugin();
		// HelpVisible = !HelpVisible;
		// if (HelpVisible) {
		GEHtmlStringBalloon balloon = plugin.createHtmlStringBalloon("");
		balloon.setCloseButtonEnabled(false);
		balloon.setContentString(content);
		plugin.setBalloon(balloon);
		// Window.alert(content);
		// } else
		// plugin.setBalloon(null);
	};

	/*
	 * public void SetWaitVisible(boolean visible) { GEPlugin plugin =
	 * earth.getGEPlugin(); GEHtmlStringBalloon baloon; if (visible) { baloon =
	 * plugin.createHtmlStringBalloon(""); baloon.setCloseButtonEnabled(false);
	 * String content = "" +
	 * "<IMG src=\"clock.gif\" WIDTH=\"200\" HEIGHT=\"200\" ALIGN=\"right\"><br>"
	 * + "    While every effort is made to ensure that the content of " +
	 * "the presented 3D model is accurate, the images are provided \"as is\" "
	 * + "and {0} makes no representations or warranties in relation " +
	 * "to the accuracy or completeness of the information found on it." +
	 * "While the content of this application is provided in good faith, " +
	 * "we do not warrant that the information will be kept up to date, be " +
	 * "true and not misleading, or that the 3D models used in the " +
	 * "application will always (or ever) be available for use." +
	 * "Nothing on this application should be taken to constitute " +
	 * "professional advice or a formal recommendation and we exclude " +
	 * "all representations and warranties relating to the content" +
	 * " and use of this application.In no event will {0} be " +
	 * "liable for any incidental, indirect, consequential or special " +
	 * "damages of any kind, or any damages whatsoever, including," +
	 * " without limitation, those resulting from loss of profit, " +
	 * "loss of contracts, goodwill, data, information, income, " +
	 * "anticipated savings or business relationships, " +
	 * "whether or not advised of the possibility of such damage, " +
	 * "arising out of or in connection with the use of this application" +
	 * " or any data sources used in this application.";
	 * baloon.setContentString(content); } else baloon = null;
	 * plugin.setBalloon(baloon); };
	 */
};
