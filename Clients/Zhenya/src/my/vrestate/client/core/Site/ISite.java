package my.vrestate.client.core.Site;

import java.util.ArrayList;

import my.vrestate.client.core.Point;
import my.vrestate.client.core.GEPlugin.IGEPlugin;

public interface ISite {
	public void Load();
	
	public Point getCenterPos();
	public void setCenterPos(Point center_pos);
	
	public void setGEPlugin(IGEPlugin ge_plugin);
	public IGEPlugin getGEPlugin();
	
	public ArrayList<IBuilding> getBuildings();
	public ArrayList<ISuite> getSuites();
}
