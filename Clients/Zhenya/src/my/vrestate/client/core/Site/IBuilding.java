package my.vrestate.client.core.Site;

import java.util.ArrayList;

import my.vrestate.client.core.Point;

public interface IBuilding {

	public void rePaint();

	Point getCenter();

	public ArrayList<ISuite> getSuites();
	
	public ISite getSite();
}
