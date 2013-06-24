package com.condox.vrestate.client.ge;


import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.Options;
import com.condox.vrestate.client.VREstate;
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
import com.nitrous.gwt.earth.client.api.KmlObject;

public class GE extends GoogleEarthWidget{

	private static GoogleEarthWidget earth = null;
	
	// =======================================================
	public static GoogleEarthWidget getEarth() {
		return earth;
	};
	//********************************************************
//	private static int width = 0;
//	private static int height = 0;
//	private static int factor = 1;
	
//	private static native int getFactor() /*-{
//        // always return 1, except at non-default zoom levels in IE before version 8
////        function GetZoomFactor () {
//            var factor = 1;
////            if ($doc.body.getBoundingClientRect) {
////                    // rect is only in physical pixel size in IE before version 8 
////                var rect = $doc.body.getBoundingClientRect ();
////                var physicalW = rect.right - rect.left;
//////                alert(physicalW);
////                var logicalW = $doc.body.offsetWidth;
//////                alert(logicalW);
////                    // the zoom level is always an integer percent value
////                factor = Math.round ((physicalW / logicalW) * 100) / 100;
////            }
//            return factor;
////        }
//        
//	}-*/;
	
	private native static float getZoomLevel() /*-{
		return $wnd["ZOOM_LEVEL"];
	}-*/;
	
	public static int getEarthWidth() {
		return (int) (earth.getOffsetWidth() * getZoomLevel());
	};
	
	public static int getEarthHeight() {
		return (int) (earth.getOffsetHeight() * getZoomLevel());
	};
	
	
	//********************************************************

	public static GEPlugin getPlugin() {
		return earth.getGEPlugin();
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
			Log.write("Exception! GE->getRange failed: " + e.getMessage());
			return getRange(LookAt); // try again
		}
	};

	public static double getTilt(KmlLookAt LookAt) {
		try {
			return LookAt.getTilt();
		} catch (Exception e) {
			Log.write("Exception! GE->getTilt failed: " + e.getMessage());
			return getTilt(LookAt); // try again
		}
	};

	public static double getHeading(KmlLookAt LookAt) {
		try {
			return LookAt.getHeading();
		} catch (Exception e) {
			Log.write("Exception! GE->getHeading failed: " + e.getMessage());
			return getHeading(LookAt); // try again
		}
	};

	public static void appendChild(KmlObject object) {
		try {
			getPlugin().getFeatures().appendChild(object);
		} catch (Exception e) {
			Log.write("Exception! GE->appendChild failed: " + e.getMessage());
			appendChild(object); // try again
		}
	};

	public static void replaceChild(KmlObject newChild, KmlObject oldChild) {
		try {
			getPlugin().getFeatures().replaceChild(newChild, oldChild);
		} catch (Exception e) {
			Log.write("Exception! GE->replaceChild failed: " + e.getMessage());
			replaceChild(newChild, oldChild); // try again
		}
	};

	// =====================================================

	private String EARTH_API_KEY = "ABQIAAAAm7LIvLNR-PkJLewH4qmS7hREGtQZq9OFJfHndXhPP8gxXzlLARQtA_EfZjc9zs77WO25FrLcaZ4ZVA";
	VREstate vrEstate = null;

	@SuppressWarnings("deprecation")
	public void Init(VREstate vrEstate) {
		this.vrEstate = vrEstate;
		//GoogleEarth.loadApi(new Runnable(){ // for latest versions of gwt-earth
		GoogleEarth.loadApi(EARTH_API_KEY, new Runnable(){
            @Override
            public void run() {
               onApiLoaded(); // start the application                          
            }               
		});
	};

	public void onApiLoaded() {
		earth = this;
		addPluginReadyListener(new GEPluginReadyListener() {
            public void pluginReady(GEPlugin ge) {
                loadEarthContent(); // show Earth content once the plugin has loaded
            }

            public void pluginInitFailure() {
                Window.alert("Failed to initialize Google Earth Plug-in");
            }
        });
		
		RootLayoutPanel.get().add(earth);
		earth.init();
	};
	
	public void loadEarthContent() {
		// show map content once the plug-in has loaded
		GEPlugin plgn = getGEPlugin();
		plgn.getWindow().setVisibility(true);
		if (Options.USE_HISTORICAL) {
			plgn.getTime().setHistoricalImageryEnabled(true);
			plgn.getTime().getControl().setVisibility(GEVisibility.VISIBILITY_HIDE);
		}

		plgn.getOptions().setMouseNavigationEnabled(false);
		
		// show some layers
		plgn.enableLayer(GELayerId.LAYER_BUILDINGS, true);
		plgn.enableLayer(GELayerId.LAYER_BORDERS, true);
		plgn.enableLayer(GELayerId.LAYER_ROADS, true);
		plgn.enableLayer(GELayerId.LAYER_TERRAIN, true);
		plgn.enableLayer(GELayerId.LAYER_TREES, true);
		// ========================

		vrEstate.LoadView();
	}

	//private boolean HelpVisible = false;

	public void ToggleHelpVisible(String content) {

		GEPlugin plugin = getGEPlugin();
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
