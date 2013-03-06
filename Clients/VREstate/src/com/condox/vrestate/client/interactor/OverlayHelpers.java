package com.condox.vrestate.client.interactor;

import com.condox.vrestate.client.ge.GE;
import com.nitrous.gwt.earth.client.api.KmlScreenOverlay;
import com.nitrous.gwt.earth.client.api.KmlUnits;

public class OverlayHelpers {
	/* ================= defining resizable clickable regions =================*/
    // Helper class - defining a single dimension either in UNITS_PIXELS
    // or in UNITS_FRACTION
    public class OvlDimension
    {
        public float fVal;
        public int iVal;
        public KmlUnits Units;
        // use this constructor to define an element if UNITS_FRACTION 
        public OvlDimension(float val)
        {
            fVal = val;
            Units = KmlUnits.UNITS_FRACTION;
        }

        // use this constructor to define an element if UNITS_PIXELS 
        public OvlDimension(int val)
        {
            iVal = val;
            Units = KmlUnits.UNITS_PIXELS;
        }

        public float getVal()
        {
            if (Units == KmlUnits.UNITS_PIXELS)
                return (float)iVal;

            return fVal;
        }
    }

    // With this class you can define a point using any combination of
    // UNITS_PIXELS/UNITS_FRACTION for its coordinates
    public class OvlPoint
    {
        public OvlDimension x;
        public OvlDimension y;
        public OvlPoint(OvlDimension x, OvlDimension y)
        {
            this.x = x;
            this.y = y;
        }
    }

    // With this class you can define a clickable and resizable rectangle using any
    // combination of UNITS_PIXELS/UNITS_FRACTION.
    public class OvlRectangle
    {
        public OvlPoint origin;
        public OvlDimension width;
        public OvlDimension height;

        // constructor
        public OvlRectangle(OvlPoint origin, 
						    OvlDimension width, 
						    OvlDimension height)
        {
            this.origin = origin;
            this.width = width;
            this.height = height;
        }
        
        public boolean ContainsPixel(int x, int y)
        {
        	int WinW = GE.getEarthWidth();
        	int WinH = GE.getEarthHeight();
		    
            int pixX = (origin.x.Units == KmlUnits.UNITS_PIXELS) ? origin.x.iVal : (int)(WinW * origin.x.fVal);
            int pixY = (origin.y.Units == KmlUnits.UNITS_PIXELS) ? origin.y.iVal : (int)(WinH * (1f-origin.y.fVal));
            
            int pixWidth = (width.Units == KmlUnits.UNITS_PIXELS) ? width.iVal : (int)(WinW * width.fVal);
            int pixHeight = (height.Units == KmlUnits.UNITS_PIXELS) ? height.iVal : (int)(WinH * height.fVal);

            pixX -= pixWidth / 2;
            pixY -= pixHeight / 2;
            
        	return (x > pixX && x < pixX + pixWidth) &&
        		   (y > pixY && y < pixY + pixHeight);
        }
        
        // Setting the ScreenOverlay's position and size
        public void InitScreenOverlay(KmlScreenOverlay overlay)
        {
            // Set the ScreenOverlay's position in the window
			overlay.getOverlayXY().setXUnits(origin.x.Units);
			overlay.getOverlayXY().setX(origin.x.getVal());
			overlay.getOverlayXY().setYUnits(origin.y.Units);
			overlay.getOverlayXY().setY(origin.y.getVal());

            // Set the overlay's size in pixels
			overlay.getSize().setXUnits(width.Units);
			overlay.getSize().setX(width.getVal());
			overlay.getSize().setYUnits(height.Units);
			overlay.getSize().setY(height.getVal());
        }

    }

}
