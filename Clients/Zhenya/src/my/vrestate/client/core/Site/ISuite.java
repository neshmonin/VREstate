package my.vrestate.client.core.Site;

import my.vrestate.client.core.Point;

public interface ISuite {

//	Координаты квартиры
	public void setPosition(Point position);
	Point getPosition();
	
//Перерисовка квартиры (проверка видимости, статуса и т.д.)
	public void rePaint();
	
	
	public void showInfo();
	
	enum Status{AVAILABLE, ON_HOLD, SOLVED};
	public void setStatus(Status status);
	public Status getStatus();
	
	
	
	
	public boolean isVisible();
	
	
	public IBuilding getBuilding();
	
	
	
	
	
	public void setSelected(boolean selected);
	
	public String getTag();

	public String getName();
	
	public double getHDG();

}
