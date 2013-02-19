package com.condox.orders.client.pages.suits;

import com.condox.orders.client.Options;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

public class Suite {

	public enum Status {
		Sold, 
		Available, 
		OnHold, 
		ResaleAvailable,
		Selected,
		Layout		
	}
	
	private int id = -1;
	private String name = "";
	private int level_number = -1;
	private String floor_name = "";
	private Status status = Status.Selected;	// by default
	private String floorplan_url = "";
	private double area = -1;
	private int bedrooms = -1;
	private int dens = -1;
	private double bathrooms = -1;
	private int balconies = -1;
	private int terraces = -1;
	private int price = -1;

	public void Parse(JSONValue value) {
		JSONObject obj = value.isObject();
		id = (int) obj.get("id").isNumber().doubleValue();
		name = obj.get("name").isString().stringValue();
		level_number = (int) obj.get("levelNumber").isNumber().doubleValue();
		floor_name = obj.get("floorName").isString().stringValue();
		String _status = obj.get("status").isString().stringValue();
		status = Status.valueOf(_status);
		if (obj.containsKey("floorPlanUrl"))
			if (obj.get("floorPlanUrl").isString() != null)
				setFloorplan_url(obj.get("floorPlanUrl").isString().stringValue());
		setArea(obj.get("area").isNumber().doubleValue());
		setBedrooms((int) obj.get("bedrooms").isNumber().doubleValue());
		setDens((int) obj.get("dens").isNumber().doubleValue());
		setBathrooms(obj.get("bathrooms").isNumber().doubleValue());
		setBalconies((int) obj.get("balconies").isNumber().doubleValue());
		setTerraces((int) obj.get("terraces").isNumber().doubleValue());
		
		if ((obj.get("currentPrice") != null)&&
			(obj.get("currentPrice").isNumber() != null) &&
			obj.get("currentPrice").isNumber().doubleValue() != 0)
			price = (int) obj.get("currentPrice").isNumber().doubleValue();
		
		if (Options.DEBUG_MODE)
		{
			 // Workaround for prices	
			 price = (int) (500 + 500 * Math.random());
			 price *= 1000;
		}
	}
	
	public int getId() {
		return id;
	}
	
	public String getName() {
		return name;
	}
	
	public int getLevelNumber() {
		return level_number;
	}
	
	public String getFloorName() {
		return floor_name;
	}

	public Status getStatus() {
		return status;
	}
	

	public int getPrice() {
		return price;
	}

	public void setBalconies(int balconies) {
		this.balconies = balconies;
	}

	public int getBalconies() {
		return balconies;
	}

	public void setFloorplan_url(String floorplan_url) {
		this.floorplan_url = floorplan_url;
	}

	public String getFloorplan_url() {
		return floorplan_url;
	}

	public void setArea(double area) {
		this.area = area;
	}

	public double getArea() {
		return area;
	}

	public void setBedrooms(int bedrooms) {
		this.bedrooms = bedrooms;
	}

	public int getBedrooms() {
		return bedrooms;
	}

	public void setBathrooms(double bathrooms) {
		this.bathrooms = bathrooms;
	}

	public double getBathrooms() {
		return bathrooms;
	}

	public void setDens(int dens) {
		this.dens = dens;
	}

	public int getDens() {
		return dens;
	}

	public void setTerraces(int terraces) {
		this.terraces = terraces;
	}

	public int getTerraces() {
		return terraces;
	}
	
	public String getTooltip() {
		String tooltip = "Suite " + getName() + "\r\n";
		if (getLevelNumber() > -1)
			tooltip += "Level number: " + getLevelNumber() + "\r\n";
		
		if(getPrice() > 0)
			tooltip += ("Price: $" + getPrice() + "\r\n");
		tooltip += "Bedrooms: " + getBedrooms() + "\r\n";
		if ((int)getBathrooms() == getBathrooms())
			tooltip += "Bathrooms: " + (int)getBathrooms() + "\r\n";
		else
			tooltip += "Bathrooms: " + getBathrooms() + "\r\n";
		tooltip += "Area: " + getArea() + " Sq.Ft.\r\n";
//		tooltip += "Floorplan: " + getFloorplan_url() + "\r\n";
		return tooltip;
	}
}
