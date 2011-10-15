package my.vrestate.client.Drawables;

import com.google.gwt.core.client.GWT;
import com.google.gwt.user.client.Window;

import my.vrestate.client.Options;
import my.vrestate.client.core.GEPlugin.GEPlugin;

public class Button implements IDrawable {

	enum States {
		BS_NORMAL, BS_ACTIVE, BS_PRESSED
	}
	
	private States State = States.BS_NORMAL;
	private boolean Visible = false;
	private int DrawId = -1;
	private int Left = 0;
	private int Top = 0;
	private int Width = 0;
	private int Height = 0;
	private String IconUrl = "";

	
	public Button(int left, int top, int width, int height, String icon_url) {
		setLeft(left);
		setTop(top);
		setWidth(width);
		setHeight(height);
		setIconUrl(icon_url);
	};
	
	@Override
	public String getKML() {
		String kml = "" +
			"<ScreenOverlay>" +
//			"<name>...</name>" +
			"<visibility>1</visibility>" +
//			"<open>0</open>" +
//			"<atom:author>...<atom:author>" +
//			"<atom:link href=\" \"/>" +
//			"<address>...</address>" +
//			"<xal:AddressDetails>...</xal:AddressDetails>" +
//			"<phoneNumber>...</phoneNumber>" +
//			"<Snippet maxLines=\"2\">...</Snippet>" +
//			"<description>...</description>" +
//			"<AbstractView>...</AbstractView>" +
//			"<TimePrimitive>...</TimePrimitive>" +
//			"<styleUrl>...</styleUrl>" +
//			"<StyleSelector>...</StyleSelector>" +
//			"<Region>...</Region>" +
//			"<Metadata>...</Metadata>              <!-- deprecated in KML 2.2 -->" +
//			"<ExtendedData>...</ExtendedData>      <!-- new in KML 2.2 -->" +
//			"<!-- inherited from Overlay element -->" +
//			"<color>ffffffff</color>                  <!-- kml:color -->" +
//			"<drawOrder>0</drawOrder>                 <!-- int -->" +
			"<Icon>" +
				"<href>" + Options.SERVER_URL + getIconUrl() + "</href>" +
			"</Icon>" +
//			"<!-- specific to ScreenOverlay -->" +
			"<overlayXY x=\"0.5\" y=\"0.5\" xunits=\"fraction\" yunits=\"fraction\"/>" +    
		    "<!-- vec2 -->" +
		    "<!-- xunits and yunits can be one of: fraction, pixels, or insetPixels -->" +
			"<screenXY x=\"" + getLeft() + "\" y=\"" + (GEPlugin.getPluginHeight() - getTop()) + "\" xunits=\"pixels\" yunits=\"pixels\"/>" +      
		    "<!-- vec2 -->" +
//			"<rotationXY x=\"0.5\" y=\"0.5\" xunits=\"fraction\" yunits=\"fraction\"/>" +  
		    "<!-- vec2 -->" +
			"<size x=\"" + getWidth() + "\" y=\"" + getHeight() + "\" xunits=\"pixels\" yunits=\"pixels\"/>" +              
		    "<!-- vec2 -->" +
//		    "<rotation>0</rotation>                   <!-- float -->" +
		    "</ScreenOverlay>";
//		GWT.log("Top=" + Top);
//		GWT.log("Height=" + Height);
		return kml;
	}

	@Override
	public void setDrawId(int draw_id) {
		DrawId = draw_id;
	}

	@Override
	public int getDrawId() {
		return DrawId;
	}

	@Override
	public void setVisible(boolean visible) {
//		GWT.log("isVisible=" + isVisible());
//		Visible = !visible;
//		GEPlugin.UpdateDrawable(this);
		Visible = visible;
		GEPlugin.UpdateDrawable(this);
	}

	@Override
	public boolean isVisible() {
		return Visible;
	}

	public void setLeft(int left) {
		Left = left;
		if (Left < 0)
			Left += GEPlugin.getPluginWidth();
	}

	public int getLeft() {
		return Left;
	}

	public void setTop(int top) {
		Top = top;
		if (Top < 0)
			Top += GEPlugin.getPluginHeight();
	}

	public int getTop() {
		return Top;
	}

	public void setIconUrl(String iconUrl) {
		IconUrl = iconUrl;
	}

	public String getIconUrl() {
		return IconUrl;
	}

	public void setWidth(int width) {
		Width = width;
	}

	public int getWidth() {
		return Width;
	}

	public void setHeight(int height) {
		Height = height;
	}

	public int getHeight() {
		return Height;
	}
	
	public int getMyTop() {
		return (getTop() + getHeight());
	}
	
	public boolean isProceed(int x, int y) {
		
		return
			(isVisible()&&
			 (x >= (Left - Width / 2))&& 
			 (x <= (Left + Width / 2))&&
			 (y >= (Top - Height / 2))&&
			 (y <= (Top + Height / 2)));
		
//		int PluginWidth = GEWrapper.getPluginWidth();
//		int PluginHeight = GEWrapper.getPluginHeight();
//		int left = Left;
//		int top = Top;
////		GWT.log("Left" + Left);
////		GWT.log("TOP" + Top);
////		GWT.log("Width" + Width);
//		
//		while(left < 0) left += PluginWidth;
//		while(top < 0) top += PluginHeight;
//		boolean a = left < x;
//		boolean b = (left + Width) > x;
//		boolean c = top < y;
//		boolean d = (top + Height) > y;
////		GWT.log("ABCD:" + a + b + c + d);
//		return 	a&&b&&c&&d;
	}
}
