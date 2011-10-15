package my.vrestate.client.core.GEPlugin;

import java.util.ArrayList;

import my.vrestate.client.Drawables.IDrawable;
import my.vrestate.client.core.Camera;
import my.vrestate.client.core.ClientPosition;
import my.vrestate.client.core.LookAt;
import my.vrestate.client.core.Point;

public class GEPlugin implements IGEPlugin
{
//	События, которые будет выплёвывать сам GEPlugin
//	GEPluginReady - готовность GEPluigin'а
	private ArrayList<IPluginReadyListener> PluginReadyListeners = new ArrayList<IPluginReadyListener>();
	
	public void addPluginReadyListener(IPluginReadyListener listener) {
		PluginReadyListeners.add(listener);
	}
	public void removePluginReadyListener(IPluginReadyListener listener) {
		PluginReadyListeners.remove(listener);
	}
	private void firePluginReady() {
		for (IPluginReadyListener listener : PluginReadyListeners)
			listener.onPluginReady();
	}
//	MouseEvent - событие мышки
	private static ArrayList<IMouseEventListener> MouseEventListeners = new ArrayList<IMouseEventListener>();
	
	public void addMouseEventListener(IMouseEventListener listener) {
		GEPlugin.MouseEventListeners.add(listener);
	}
	public void removeMouseEventListener(IMouseEventListener listener) {
		GEPlugin.MouseEventListeners.remove(listener);
	}
	private void fireMouseEvent(MouseEventData mouse_event_data) {
//		GWT.log("Rising plugin ready");
		for (IMouseEventListener listener : MouseEventListeners)
			listener.onMouseEvent(mouse_event_data);
	}
//	ViewChanged - вид изменился
	private ArrayList<IViewChangedListener> ViewChangedListeners = new ArrayList<IViewChangedListener>();
	
	public void addViewChangedListener(IViewChangedListener listener) {
		this.ViewChangedListeners.add(listener);
	}
	public void removeViewChangedListener(IViewChangedListener listener) {
		this.ViewChangedListeners.remove(listener);
	}
	private void fireViewChanged() {
//		GWT.log("Rising viewchanged ready");
		for (IViewChangedListener listener : ViewChangedListeners)
			listener.onViewChanged();
	}
//	С событиями покончено, теперь собственно методы.
//	Init - инициализация GEPlugin'а
	public native void Init()/*-{
		$wnd.Instance = this;
		//=============================================
		$wnd.Drawables = new Array();
														
														
														
		//=============================================
		//		Глобальные переменные, используемые в JavaScript'овской
		//     	части GEWrapper'а.
		//    	ВАЖНО: префикс $wnd. является JSNI-синонимом window. Если его не 
		//    		использовать, то переменная ge будет видна только в данной функции.
		//    		Из других native-функций GEWrapper'а переменная ge видна не будет.
		//    		Сам очень долго ломал над этим голову.
		//  	$wnd.ge  - глобальный экземпляр GEPlugin'а.
		$wnd.ge = null;
		//		Собственно инициализация.
		//		OKCallback, ErrorCallback - функции, вызываемые при успехе или ошибке
		//		соотвественно.
		$wnd.google.earth.createInstance('map3d', OKCallback, ErrorCallback);

		function OKCallback(instance) {
			$wnd.ge = instance;
//						        	$wnd.ge.getOptions().setMouseNavigationEnabled(false);
			$wnd.ge.getOptions().setFlyToSpeed($wnd.ge.SPEED_TELEPORT);
			//			$wnd.ge.getOptions().setScrollWheelZoomSpeed(0);
			$wnd.ge.getWindow().setVisibility(true);
			
			
			
			
			$wnd.google.earth.addEventListener(
				$wnd.ge.getWindow(),
				'mousedown',
				function(event) {
					event.preventDefault();
					var MouseEventData = @my.vrestate.client.core.GEPlugin.MouseEventData::new()();
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Type = 
						@my.vrestate.client.core.GEPlugin.MouseEventData.MouseEventType::ME_DOWN;
					switch(event.getButton()) {
					case 0:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_LEFT;
						break;
					case 1:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_MIDDLE;
						break;
					case 2:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_RIGHT;
						break;
					};
					var X = event.getClientX();
					var Y = event.getClientY();
//					Y = $doc.getElementById('map3d').offsetHeight - Y;
					var Tag = '';
					
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::X = X;
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Y = Y;
					$wnd.Instance.@my.vrestate.client.core.GEPlugin.GEPlugin::fireMouseEvent(Lmy/vrestate/client/core/GEPlugin/MouseEventData;)
						(MouseEventData);
					});
					
			$wnd.google.earth.addEventListener(
				$wnd.ge.getWindow(),
				'mousemove',
				function(event) {
					event.preventDefault();
					var MouseEventData = @my.vrestate.client.core.GEPlugin.MouseEventData::new()();
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Type = 
						@my.vrestate.client.core.GEPlugin.MouseEventData.MouseEventType::ME_MOVE;
					switch(event.getButton()) {
					case 0:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_LEFT;
						break;
					case 1:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_MIDDLE;
						break;
					case 2:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_RIGHT;
						break;
					};
					var X = event.getClientX();
					var Y = event.getClientY();
//					Y = $doc.getElementById('map3d').offsetHeight - Y;
					
					var Tag = '';
					var target = event.getTarget();
					if (target != $wnd.ge.getWindow())
						Tag = target.getId();
					
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::X = X;
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Y = Y;
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Tag = Tag;
					$wnd.Instance.@my.vrestate.client.core.GEPlugin.GEPlugin::fireMouseEvent(Lmy/vrestate/client/core/GEPlugin/MouseEventData;)
						(MouseEventData);
					});
			$wnd.google.earth.addEventListener(
				$wnd.ge.getWindow(),
				'mouseup',
				function(event) {
					event.preventDefault();
					var MouseEventData = @my.vrestate.client.core.GEPlugin.MouseEventData::new()();
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Type = 
						@my.vrestate.client.core.GEPlugin.MouseEventData.MouseEventType::ME_UP;
					switch(event.getButton()) {
					case 0:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_LEFT;
						break;
					case 1:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_MIDDLE;
						break;
					case 2:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_RIGHT;
						break;
					};
					var X = event.getClientX();
					var Y = event.getClientY();
//					Y = $doc.getElementById('map3d').offsetHeight - Y;
					var Tag = '';
					
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::X = X;
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Y = Y;
					$wnd.Instance.@my.vrestate.client.core.GEPlugin.GEPlugin::fireMouseEvent(Lmy/vrestate/client/core/GEPlugin/MouseEventData;)
						(MouseEventData);
					});
					
			$wnd.google.earth.addEventListener(
				$wnd.ge.getWindow(),
				'click',
				function(event) {
					event.preventDefault();
					var MouseEventData = @my.vrestate.client.core.GEPlugin.MouseEventData::new()();
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Type = 
						@my.vrestate.client.core.GEPlugin.MouseEventData.MouseEventType::ME_CLICK;
					switch(event.getButton()) {
					case 0:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_LEFT;
						break;
					case 1:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_MIDDLE;
						break;
					case 2:
						MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Button = 
							@my.vrestate.client.core.GEPlugin.MouseEventData.ButtonType::MB_RIGHT;
						break;
					};
					var X = event.getClientX();
					var Y = event.getClientY();
//					Y = $doc.getElementById('map3d').offsetHeight - Y;
					
					var Tag = '';
					var target = event.getTarget();
					if(target.getType() == 'KmlPlacemark') {
						point = target.getGeometry();
						var lat = point.getLatitude();
						var lon = point.getLongitude();
						var alt = point.getAltitude();
						var xy = $wnd.ge.getView().project(lat, lon, alt, $wnd.ge.ALTITUDE_RELATIVE_TO_GROUND);
						X = Math.floor(xy.getX());
						Y = Math.floor(xy.getY());
					}
//					if (target != $wnd.ge.getWindow())
//						Tag = target.getId();
					
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::X = X;
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Y = Y;
					MouseEventData.@my.vrestate.client.core.GEPlugin.MouseEventData::Tag = Tag;
					$wnd.Instance.@my.vrestate.client.core.GEPlugin.GEPlugin::fireMouseEvent(Lmy/vrestate/client/core/GEPlugin/MouseEventData;)
						(MouseEventData);
					});
//
			$wnd.google.earth
					.addEventListener(
							$wnd.ge.getView(),
							'viewchangeend',
							function() {
//								var ViewEvent = @my.vrestate.client.events.Event::new(Lmy/vrestate/client/events/Events$Type;Ljava/lang/Object;)
//									(@my.vrestate.client.events.Events.Type::VIEW_CHANGED, null);
//								@my.vrestate.client.events.Events::FireEvent(Lmy/vrestate/client/events/Event;)(ViewEvent);
								$wnd.Instance.@my.vrestate.client.core.GEPlugin.GEPlugin::fireViewChanged()();
							});
							
//							
//							
////			$wnd.google.earth
////					.addEventListener(
////							$wnd.ge,
////							'frameend',
////							function() {
////								var ViewEvent = @my.vrestate.client.events.EventFrameEnd::new()();
////								@my.vrestate.client.events.Events::FireEvent(Lmy/vrestate/client/events/AbstractEvent;)(ViewEvent);
////							});
//
//			//			$wnd.google.earth.addEventListener($wnd.ge.getWindow(),'mouseup',
//			//				function() {
//			//					alert('UP!');
//			//				});
//			//			$wnd.onGoPanoramicView = function(suite) {
//			////				alert('ongo');
//			//				var event = @my.vrestate.client.events.EventOpenPanoramicView::new(Lmy/vrestate/client/core/Suite;)(suite)
//			//				@my.vrestate.client.events.Events::FireEvent(Lmy/vrestate/client/events/AbstractEvent;)(event);
//			//			}
//			//			alert(1);	
//			// Если все ОК
//			//===========================================================
//			
//
//			//===========================================================
//			var Event = @my.vrestate.client.events.Event::new(Lmy/vrestate/client/events/Events$Type;Ljava/lang/Object;)
//				(@my.vrestate.client.events.Events.Type::PLUGIN_READY);
//			@my.vrestate.client.events.Events::FireEvent(Lmy/vrestate/client/events/Event;)(Event);
//			//*************************************************
//			alert('OK');
			$wnd.Instance.@my.vrestate.client.core.GEPlugin.GEPlugin::firePluginReady()();
			
						
		}

		function ErrorCallback(errorCode) {
			//alert("Error loading plugin.");
		}

	}-*/;
	


//	public static native void UpdateSite(Site Site)/*-{
////				var TAG = Site.@my.vrestate.client.core.Site::GE_ID;
//		//		var NAME = Site.@my.vrestate.client.core.Site::Name;
////		var ModelUrl = Site.@my.vrestate.client.core.Site::ModelUrl;
//
//		var OLD = $wnd.ge.getElementById(TAG);
//		//		alert(OLD);
//		if (OLD != null)
//			$wnd.ge.getFeatures().removeChild(OLD);
//
//		var S = '<?xml version=\"1.0\" encoding=\"utf-8\"?>'
//				+ '<kml xmlns=\"http://www.opengis.net/kml/2.2\">'
//				+ '<Folder id=\"' + TAG + '\">' + '<NetworkLink>' + '<Link>'
//				+ '<href>' + ModelUrl + '</href>' + '</Link>'
//				+ '</NetworkLink>' + '</Folder>' + '</kml>';
//		//		alert(S);
//		var Object = $wnd.ge.parseKml(S);
//		$wnd.ge.getFeatures().appendChild(Object);
//
//	}-*/;
//
//	public static native void DrawBuilding(Building building)/*-{
////		var tag = building.@my.vrestate.client.core.Building::getTag()();
////		var existing = $wnd.ge.getElementById(tag);
////		if (existing != null)
////			$wnd.getFeatures().removeChild(existing);
////		var building_placemark = $wnd.ge.createPlacemark(tag);
////		
//	}-*/;
//
//	public static native void UpdateBuilding(Building Building)/*-{
//		var TAG = Building.@my.vrestate.client.core.Building::Tag;
//		var NAME = Building.@my.vrestate.client.core.Building::Name;
//		var ModelUrl = Building.@my.vrestate.client.core.Building::ModelUrl;
//
//		var OLD = $wnd.ge.getElementById(TAG);
//		alert(TAG);
//		if (OLD != null)
//			$wnd.ge.getFeatures().removeChild(OLD);
//
//		var S = '<?xml version=\"1.0\" encoding=\"utf-8\"?>'
//				+ '<kml xmlns=\"http://www.opengis.net/kml/2.2\">'
//				+ '<Folder id=\"' + TAG + '\">' + '<NetworkLink>' + '<Link>'
//				+ '<href>' + ModelUrl + '</href>' + '</Link>'
//				+ '</NetworkLink>' + '</Folder>' + '</kml>';
//		//		alert(S);
//		var Object = $wnd.ge.parseKml(S);
//		$wnd.ge.getFeatures().appendChild(Object);
//	}-*/;
//
//	public static native void UpdateSuite(Suite Suite)/*-{
//		var TAG = Suite.@my.vrestate.client.core.Suite::Tag;
//		var NAME = Suite.@my.vrestate.client.core.Suite::Name;
//
//		var Status = Suite.@my.vrestate.client.core.Suite::Status;
//
//		var EXISTING = $wnd.ge.getElementById(TAG);
//		if (EXISTING != null)
//			$wnd.ge.getFeatures().removeChild(EXISTING);
//		//		alert(1);
//
//		var S = '<?xml version=\"1.0\" encoding=\"utf-8\"?>'
//				+ '<kml xmlns=\"http://www.opengis.net/kml/2.2\">'
//				+ '<Placemark id=\"' + TAG + '\">' + '<Style>' + '<LineStyle>';
//		switch (Status) {
//		case 2: {
//			S = S + '<color>ff0000ff</color>';
//			break;
//		}
//		case 0: {
//			S = S + '<color>ff00ff00</color>';
//			break;
//		}
//		case 1: {
//			S = S + '<color>ff00ffff</color>';
//			break;
//		}
//		}
//		;
//		S += '<width>2</width>' + '</LineStyle>' + '</Style>' +
//		//				'<LineString>' +
//		//					'<extrude>0</extrude>' +
//		//					'<altitudeMode>absolute</altitudeMode>' +
//		//					'<coordinates>';
//		//					var Point0 = Suite.@my.vrestate.client.core.Suite::Position.@my.vrestate.client.core.XYZ::Clone()();
//		//					function moveXYZ(X,Y,Z){
//		//						Point0.@my.vrestate.client.core.XYZ::setXYZ(DDD)(
//		//							Point0.@my.vrestate.client.core.XYZ::getX()() + X,
//		//							Point0.@my.vrestate.client.core.XYZ::getY()() + Y,
//		//							Point0.@my.vrestate.client.core.XYZ::getZ()() + Z);
//		//						S +=
//		//							Point0.@my.vrestate.client.core.XYZ::getLon()() + ',' +
//		//							Point0.@my.vrestate.client.core.XYZ::getLat()() + ',' +
//		//							Point0.@my.vrestate.client.core.XYZ::getAlt()() + ' ';
//		//					};
//		//					
//		//					var SuiteType = Suite.@my.vrestate.client.core.Suite::Type;
//		//					var PointsCount = SuiteType.@my.vrestate.client.core.SuiteType::getPointsCount()();
//		//					for (index = 0; index < PointsCount; index++) {
//		//						var Point = SuiteType.@my.vrestate.client.core.SuiteType::getPoint(I)(index);
//		//							var X = Point.@my.vrestate.client.core.XYZ::getX()();
//		//							var Y = Point.@my.vrestate.client.core.XYZ::getY()();
//		//							var Z = Point.@my.vrestate.client.core.XYZ::getZ()();
//		//						moveXYZ(X,Y,Z);
//		//					}
//		//					S +=					
//		//					'</coordinates>' +
//		//				'</LineString>' +
//		'</Placemark>' + '</kml>';
//		//	alert(S);
//		var Object = $wnd.ge.parseKml(S);
//		$wnd.ge.getFeatures().appendChild(Object);
//	}-*/;
//
//	public static native void UpdateBalloon(Balloon Balloon)/*-{
//
//		var TARGET = $wnd.ge
//				.getElementById(Balloon.@my.vrestate.client.core.Balloon::TARGET_ID);
//		var balloon = $wnd.ge.createHtmlStringBalloon('');
//		balloon.setFeature(TARGET);
//		balloon
//				.setContentString(Balloon.@my.vrestate.client.core.Balloon::TEXT);
//		$wnd.ge.setBalloon(balloon);
//	}-*/;
//
//	public static native void UpdatePlacemark(Placemark Placemark)/*-{
//		var TAG = Placemark.@my.vrestate.client.core.Placemark::Id;
//
//		var NAME = Placemark.@my.vrestate.client.core.Placemark::getName()();
//		var VISIBILITY = Placemark.@my.vrestate.client.core.Placemark::Visible;
//		var POSITION = Placemark.@my.vrestate.client.core.Placemark::Position;
//		var LON = POSITION.@my.vrestate.client.core.Point::getLongitude()();
//		var LAT = POSITION.@my.vrestate.client.core.Point::getLatitude()();
//		var ALT = POSITION.@my.vrestate.client.core.Point::getAltitude()();
//
//		var ALREADY_EXISTING = $wnd.ge.getElementById(TAG);
//		if (ALREADY_EXISTING != null)
//			$wnd.ge.getFeatures().removeChild(ALREADY_EXISTING);
//
////		//		alert(1);
////		var S = '<?xml version=\"1.0\" encoding=\"utf-8\"?>'
////				+ '<kml xmlns=\"http://www.opengis.net/kml/2.2\">'
////				+ '<Placemark id=\"' + TAG + '\">' + '<name>' + NAME
////				+ '</name>';
////		if (VISIBILITY == true)
////			S += '<visibility>1</visibility>';
////		else
////			S += '<visibility>0</visibility>';
////		S += '<Point>' + '<altitudeMode>relativeToGround</altitudeMode>'
////				+ '<coordinates>' + LON + ',' + LAT + ',' + ALT
////				+ '</coordinates>' + '</Point>' + '</Placemark>' + '</kml>';
////		//		alert(S);
////		var Object = $wnd.ge.parseKml(S);
////		$wnd.ge.getFeatures().appendChild(Object);
//		
//		// Create the placemark.
//		var placemark = $wnd.ge.createPlacemark(TAG);
//		placemark.setName(NAME);
////		$wnd.ge.addEventListener(placemark,'mousemove', alert('move!!'));
//
//		// Define a custom icon.
////		var icon = $wnd.ge.createIcon('http://127.0.0.1:8026/empty_icon.png');
////		icon.setHref('');
////		var style = $wnd.ge.createStyle(''); //create a new style
////		style.getIconStyle().setIcon(icon); //apply the icon to the style
////		placemark.setStyleSelector(style); //apply the style to the placemark
//
//		// Set the placemark's location.  
//		var point = $wnd.ge.createPoint('');
//		point.set(LAT, LON, ALT,$wnd.ge.ALTITUDE_RELATIVE_TO_GROUND,false,false);
//		placemark.setGeometry(point);
//
//		// Add the placemark to Earth.
//		$wnd.ge.getFeatures().appendChild(placemark);
//		
//
//	}-*/;
//
//	public static native String getSelectedSuiteName() /*-{
//		var CurrentBalloon = $wnd.ge.getBalloon();
//		alert(CurrentBalloon);
//		if (CurrentBalloon != null)
//			var CurrentFeature = CurrentBalloon.getFeature();
//		var Name = null;
//		if (CurrentFeature != null)
//			var Name = CurrentFeature.getName();
//		return Name;
//	}-*/;
//
	public static native int getPluginHeight()/*-{
		return $doc.getElementById('map3d').offsetHeight;
	}-*/;

	public static native int getPluginWidth()/*-{
		return PluginWidth = $doc.getElementById('map3d').offsetWidth;
	}-*/;
//
////	public static native void UpdateButton(AbstractButton Button) /*-{
//////		//				alert('Updating button');
//////		var ID = Button.@my.vrestate.client.buttons.AbstractButton::ID;
//////		//		alert('Button ID=' + ID);
//////		var screenOverlay = $wnd.ge.getElementById(ID);
//////		if (screenOverlay == null) {
//////			screenOverlay = $wnd.ge.createScreenOverlay(ID);
//////			$wnd.ge.getFeatures().appendChild(screenOverlay);
//////		}
//////		if (Button == null) {
//////			screenOverlay.setVisibility(false);
//////		} else {
//////			var X = Button.@my.vrestate.client.buttons.AbstractButton::X;
//////			var Y = Button.@my.vrestate.client.buttons.AbstractButton::Y;
//////			var SIZE_X = Button.@my.vrestate.client.buttons.AbstractButton::SIZE_X;
//////			var SIZE_Y = Button.@my.vrestate.client.buttons.AbstractButton::SIZE_Y;
//////
//////			var ICON_URL = Button.@my.vrestate.client.buttons.AbstractButton::ICON_URL
//////					+ Button.@my.vrestate.client.buttons.AbstractButton::STATE
//////					+ '.png';
//////			var icon = $wnd.ge.createIcon('');
//////			//			alert(ICON_URL);
//////			icon.setHref(ICON_URL);
//////			screenOverlay.setIcon(icon);
//////
//////			screenOverlay.getOverlayXY().setXUnits($wnd.ge.UNITS_PIXELS);
//////			screenOverlay.getOverlayXY().setX(X);
//////			screenOverlay.getOverlayXY().setYUnits($wnd.ge.UNITS_PIXELS);
//////			screenOverlay.getOverlayXY().setY(Y);
//////			screenOverlay.getScreenXY().setX(0);
//////			screenOverlay.getScreenXY().setY(0);
//////			screenOverlay.getSize().setXUnits($wnd.ge.UNITS_PIXELS);
//////			screenOverlay.getSize().setYUnits($wnd.ge.UNITS_PIXELS);
//////			screenOverlay.getSize().setX(SIZE_X);
//////			screenOverlay.getSize().setY(SIZE_Y);
//////			screenOverlay.setVisibility(true);
//////			//=================================================================
//////			$wnd.google.earth.addEventListener(screenOverlay, 'mousemove',
//////					function(event) {
//////						//					alert(1);
//////						//					event.stopPropagation();
//////						//					@my.vrestate.client.events.Events::FireEvent(Lmy/vrestate/client/core/Constants$EventTypes;Ljava/lang/Object;)
//////						//					(@my.vrestate.client.core.Constants.EventTypes::BUTTON_MOUSE_MOVE, Button);
//////					});
//////			$wnd.google.earth.addEventListener(screenOverlay, 'click',
//////					function(event) {
//////						alert(1);
//////						event.stopPropagation();
//////						//					@my.vrestate.client.events.Events::FireEvent(Lmy/vrestate/client/core/Constants$EventTypes;Ljava/lang/Object;)
//////						//					(@my.vrestate.client.core.Constants.EventTypes::BUTTON_MOUSE_CLICK, Button);
//////					});
//////			//=================================================================
//////		}
////
////	}-*/;
//
//
	public native Camera getCamera() /*-{
//		alert(1);
		var CurrCamera = $wnd.ge.getView().copyAsCamera($wnd.ge.ALTITUDE_RELATIVE_TO_GROUND);
		var Camera = @my.vrestate.client.core.Camera::new()();
		Camera.@my.vrestate.client.core.Camera::setLatitude(D)(CurrCamera.getLatitude());
		Camera.@my.vrestate.client.core.Camera::setLongitude(D)(CurrCamera.getLongitude());
		Camera.@my.vrestate.client.core.Camera::setAltitude(D)(CurrCamera.getAltitude());
		Camera.@my.vrestate.client.core.Camera::setHeading(D)(CurrCamera.getHeading());
		Camera.@my.vrestate.client.core.Camera::setTilt(D)(CurrCamera.getTilt());
		Camera.@my.vrestate.client.core.Camera::setRoll(D)(CurrCamera.getRoll());
//		alert(2);
		return Camera;
	}-*/;
//
	public native LookAt getLookAt() /*-{
		var CurrLookAt = $wnd.ge.getView().copyAsLookAt($wnd.ge.ALTITUDE_RELATIVE_TO_GROUND);
		var LookAt = @my.vrestate.client.core.LookAt::new()();
		LookAt.@my.vrestate.client.core.LookAt::setLatitude(D)(CurrLookAt.getLatitude());
		LookAt.@my.vrestate.client.core.LookAt::setLongitude(D)(CurrLookAt.getLongitude());
		LookAt.@my.vrestate.client.core.LookAt::setAltitude(D)(CurrLookAt.getAltitude());
		LookAt.@my.vrestate.client.core.LookAt::setHeading(D)(CurrLookAt.getHeading());
		LookAt.@my.vrestate.client.core.LookAt::setTilt(D)(CurrLookAt.getTilt());
		LookAt.@my.vrestate.client.core.LookAt::setRange(D)(CurrLookAt.getRange());
		return LookAt;
	}-*/;
//
	public native void setCamera(Camera Src) /*-{
		var Camera = $wnd.ge.createCamera('');
		Camera
				.setLatitude(Src.@my.vrestate.client.core.Camera::getLatitude()());
		Camera
				.setLongitude(Src.@my.vrestate.client.core.Camera::getLongitude()());
		Camera
				.setAltitude(Src.@my.vrestate.client.core.Camera::getAltitude()());
		Camera.setHeading(Src.@my.vrestate.client.core.Camera::getHeading()());
		Camera.setTilt(Src.@my.vrestate.client.core.Camera::getTilt()());
		Camera.setRoll(Src.@my.vrestate.client.core.Camera::getRoll()());
		$wnd.ge.getView().setAbstractView(Camera);
	}-*/;

	public native void setLookAt(LookAt Src) /*-{
		var LookAt = $wnd.ge.createLookAt('');
		LookAt
				.setLatitude(Src.@my.vrestate.client.core.LookAt::getLatitude()());
		LookAt
				.setLongitude(Src.@my.vrestate.client.core.LookAt::getLongitude()());
		LookAt
				.setAltitude(Src.@my.vrestate.client.core.LookAt::getAltitude()());
		LookAt.setHeading(Src.@my.vrestate.client.core.LookAt::getHeading()());
		LookAt.setTilt(Src.@my.vrestate.client.core.LookAt::getTilt()());
		LookAt.setRange(Src.@my.vrestate.client.core.LookAt::getRange()());
		$wnd.ge.getView().setAbstractView(LookAt);
	}-*/;
//
	public native void ShowBalloon(int id, String data)/*-{
//		var object = $wnd.ge.getElementById(id);
		var object = $wnd.Drawables[id];
		var balloon = $wnd.ge.createHtmlStringBalloon('');
		balloon.setFeature(object); // optional
		balloon.setCloseButtonEnabled(false);
		balloon.setContentString(data);
//		  balloon.setContentString(' <object width="200" height="150"></object>');
//		  balloon.setContentString(' abdsffffffffffffffffffff<br><br>fffffffffffffffffffffffffffffffffffffffc');
		$wnd.ge.setBalloon(balloon);
	}-*/;

	public native void HideBalloon()/*-{
//		alert('Hiding balloon');
		$wnd.ge.setBalloon(null);
	}-*/;
//
//
//	public native static void UpdateButton(AbstractButton button) /*-{
////		alert('Updating button');
//		var id = button.@my.vrestate.client.buttons.AbstractButton::getDrawId()();
//		var visible = button.@my.vrestate.client.buttons.AbstractButton::isVisible()();
//		var active = button.@my.vrestate.client.buttons.AbstractButton::isActive()();
//		var screenOverlay = $wnd.ge.getElementById(id);
////		alert(active);
//		
//		if ((screenOverlay)&&(!active)) {
//			$wnd.ge.getFeatures().removeChild(screenOverlay);
//			screenOverlay.release();
////			screenOverlay.setVisibility(false);
//		}
//		if ((!screenOverlay)&&(active)) {
////			var new_id = 0;
////			var t = $wnd.ge.getElementById(new_id);
////			while(t){
////				alert(t);
////				new_id++;
////				t = $wnd.ge.getElementById(new_id);
////			alert('+');
////			}
////			id = '' + new_id;
//
//			screenOverlay = $wnd.ge.createScreenOverlay(id);
//			$wnd.ge.getFeatures().appendChild(screenOverlay);
//			button.@my.vrestate.client.buttons.AbstractButton::setDrawId(Ljava/lang/String;)(id);
//		}
//		
//		if ((screenOverlay)&&(active)) {
//			
//			var x = button.@my.vrestate.client.buttons.AbstractButton::getLeft()();
//			var y = button.@my.vrestate.client.buttons.AbstractButton::getTop()();
//			// Здесь надо переделать красиво----------------------------------
//			if (x < 0) x += $doc.getElementById('map3d').offsetWidth;
//			if (y < 0) y += $doc.getElementById('map3d').offsetHeight;
//			//---------------------------------------------------------------
//			var width = button.@my.vrestate.client.buttons.AbstractButton::getWidth()();
//			var height = button.@my.vrestate.client.buttons.AbstractButton::getHeight()(); 
////
//			var ICON_URL = button.@my.vrestate.client.buttons.AbstractButton::ICON_URL;
//			var icon = $wnd.ge.createIcon('');
//			icon.setHref(ICON_URL);
//			screenOverlay.setIcon(icon);
////
//			screenOverlay.getOverlayXY().setXUnits($wnd.ge.UNITS_PIXELS);
//			screenOverlay.getOverlayXY().setYUnits($wnd.ge.UNITS_PIXELS);
//			screenOverlay.getOverlayXY().setX(x);
//			screenOverlay.getOverlayXY().setY(y);
//			screenOverlay.getScreenXY().setX(0);
//			screenOverlay.getScreenXY().setY(0);
//			
//			screenOverlay.getSize().setXUnits($wnd.ge.UNITS_PIXELS);
//			screenOverlay.getSize().setYUnits($wnd.ge.UNITS_PIXELS);
//			screenOverlay.getSize().setX(width);
//			screenOverlay.getSize().setY(height);
//			screenOverlay.setVisibility(visible);
//		}		
//	}-*/;
//
	public native static void UpdateDrawable(IDrawable drawable)/*-{
		var visible = drawable.@my.vrestate.client.Drawables.IDrawable::isVisible()();
//		visible = true;
		var draw_id = drawable.@my.vrestate.client.Drawables.IDrawable::getDrawId()();
		var kml_data = drawable.@my.vrestate.client.Drawables.IDrawable::getKML()();

		if (visible && (draw_id >= 0)) {
			$wnd.Drawables[draw_id].setVisibility(true);
		}

		if (visible && !(draw_id >= 0)) {
			var i = 0;
			while($wnd.Drawables[i]) i++;
  			var new_object = $wnd.ge.parseKml(kml_data);
  			$wnd.Drawables[i] = new_object; 
         	$wnd.ge.getFeatures().appendChild(new_object);
         	drawable.@my.vrestate.client.Drawables.IDrawable::setDrawId(I)(i);
		}
		if (!visible && (draw_id >= 0)) {
			draw_id = parseInt(draw_id);
			if (draw_id) {
				$wnd.Drawables[draw_id].setVisibility(false);
				$wnd.ge.getFeatures().removeChild($wnd.Drawables[draw_id]);
				$wnd.Drawables[draw_id] = null;
	         	drawable.@my.vrestate.client.Drawables.IDrawable::setDrawId(I)(-1);
			}
		}
		
		if (!visible && !(draw_id >= 0));
		
	}-*/;
	
	public native static ClientPosition getClientPosition(Point point)/*-{
		var lat = point.@my.vrestate.client.core.Point::getLatitude()();
		var lon = point.@my.vrestate.client.core.Point::getLongitude()();
		var alt = point.@my.vrestate.client.core.Point::getAltitude()();
		var xy = $wnd.ge.getView().project(lat, lon, alt, $wnd.ge.ALTITUDE_RELATIVE_TO_GROUND);
//		xy.setXUnits($wnd.ge.UNITS_PIXELS);
		var X = xy.getX();
		var Y = xy.getY();
//		var X = 0;
//		var Y = 0;
		var res = @my.vrestate.client.core.ClientPosition::new(DD)(X,Y);
		return res;
	}-*/;

}