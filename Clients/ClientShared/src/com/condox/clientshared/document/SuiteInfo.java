package com.condox.clientshared.document;

import com.condox.clientshared.abstractview.Log;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.json.client.JSONValue;

public class SuiteInfo implements I_JSON {

	public static SuiteInfo fromJSON(JSONObject obj) {
		SuiteInfo info = new SuiteInfo();
		info.fromJSONObject(obj);
		return info;
	}

	public enum Status {
		Sold, Available, OnHold, ResaleAvailable, Selected, Layout, AvailableRent // ??
		, AvailableResale
	}

	private int id = -1;
	private String name = "";
	private int level_number = -1;
	private String floor_name = "";
	private Status status = Status.Selected; // by default
	private String floorplan_url = "";
	private double area = -1;
	private int bedrooms = -1;
	private int dens = -1;
	private double bathrooms = -1;
	private int balconies = -1;
	private int terraces = -1;
	private int price = -1;
	private String currentPriceDisplay = "";
	private int buildingId = -1;
	private String virtualTourURL = "";
	private String moreInfoURL = "";
	private String mls = "";
	private String address = "";

	private JSONObject backup = null;

	private void setString(JSONObject obj, String key, String value) {
		if (value != null)
			obj.put(key, new JSONString(value));
	}

	@Override
	public JSONObject toJSONObject() {
		JSONObject obj = new JSONObject();
		obj = backup;
		obj.put("mlsId", new JSONString(mls));
		obj.put("currentPrice", new JSONNumber(price));
		obj.put("buildingId", new JSONNumber(buildingId));
		obj.put("status", new JSONString(status.toString()));
		obj.put("vTourUrl", new JSONString(virtualTourURL));
		obj.put("infoUrl", new JSONString(moreInfoURL));
		setString(obj, "currentPriceDisplay", currentPriceDisplay);
		return obj;
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
		backup = obj;
		id = (int) obj.get("id").isNumber().doubleValue();
		// -------------
		if (obj.get("buildingId") != null)
			if (obj.get("buildingId").isNumber() != null)
				buildingId = (int) obj.get("buildingId").isNumber()
						.doubleValue();

		if ((obj.containsKey("name") && (obj.get("name").isString() != null)))
			name = obj.get("name").isString().stringValue();

		if ((obj.containsKey("levelNumber") && (obj.get("levelNumber")
				.isNumber() != null)))
			level_number = (int) obj.get("levelNumber").isNumber()
					.doubleValue();

		floor_name = obj.get("floorName").isString().stringValue();

		// TODO
		try {
			virtualTourURL = obj.get("vTourUrl").isString().stringValue();
			moreInfoURL = obj.get("infoUrl").isString().stringValue();
			// mls = obj.get("mlsId").isString().stringValue();
			address = obj.get("address").isString().stringValue();
		} catch (Exception e) {

		}
		if ((obj.containsKey("mlsId")) && (obj.get("mlsId").isString() != null))
			mls = obj.get("mlsId").isString().stringValue();

		String _status = obj.get("status").isString().stringValue();
		try {
			status = Status.valueOf(_status);
		} catch (Exception e) {
			e.printStackTrace();
			status = Status.Available;
		}
		// Log.write(_status);

		if (obj.containsKey("floorPlanUrl"))
			if (obj.get("floorPlanUrl").isString() != null)
				setFloorplan_url(obj.get("floorPlanUrl").isString()
						.stringValue());
		
		if ((obj.containsKey("area"))&&(obj.get("area").isNumber()!=null))
		 area = obj.get("area").isNumber().doubleValue();
		
		if ((obj.containsKey("bedrooms"))
				&& (obj.get("bedrooms").isNumber() != null))
			bedrooms = (int) obj.get("bedrooms").isNumber().doubleValue();
		
		if ((obj.containsKey("dens"))&&(obj.get("dens").isNumber()!=null))
		dens=(int) obj.get("dens").isNumber().doubleValue();
		
		if ((obj.containsKey("bathrooms"))
				&& (obj.get("bathrooms").isNumber() != null))
			bathrooms = obj.get("bathrooms").isNumber().doubleValue();
		// setBalconies((int) obj.get("balconies").isNumber().doubleValue());
		// setTerraces((int) obj.get("terraces").isNumber().doubleValue());

		// Processing of price
		JSONValue jv = obj.get("currentPrice");
		if (jv != null) {
			JSONString js = jv.isString();
			if (js != null) {
				String priceStr = js.stringValue();
				try {
					Double priceDouble = Double.valueOf(priceStr);
					price = priceDouble.intValue();
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		}
		if ((obj.get("currentPrice") != null)
				&& (obj.get("currentPrice").isNumber() != null))
			price = (int) obj.get("currentPrice").isNumber().doubleValue();
		// Log.write(id + ": "+"$" + price + " - " + status);

		/*
		 * if (price < 0) { // Workaround for prices price = (int) (500 + 500 *
		 * Math.random()); price *= 1000; }
		 */
		currentPriceDisplay = getString(obj, "currentPriceDisplay");
	}

	public String getCurrentPriceDisplay() {
		return currentPriceDisplay;
	}

	public void setCurrentPriceDisplay(String currentPriceDisplay) {
		this.currentPriceDisplay = currentPriceDisplay;
	}

	private String getString(JSONObject obj, String key) {
		if (obj.get(key) != null)
			if (obj.get(key).isString() != null)
				return obj.get(key).isString().stringValue();
		return null;
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

	public void setStatus(Status newStatus) {
		status = newStatus;
	}

	public int getPrice() {
		return price;
	}

	public void setPrice(int newPrice) {
		price = newPrice;
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

	public int getBuildingId() {
		return buildingId;
	}

	public void setTerraces(int terraces) {
		this.terraces = terraces;
	}

	public int getTerraces() {
		return terraces;
	}

	public String getAddress() {
		return address;
	}

	public String getTooltip() {
		String tooltip = "Suite " + getName() + "\r\n";
		if (getLevelNumber() > -1)
			tooltip += "Level number: " + getLevelNumber() + "\r\n";

		if (getPrice() > 0)
			tooltip += ("Price: $" + getPrice() + "\r\n");

		if ((getBedrooms() == 0) && (int) getBathrooms() != 0)
			tooltip += "Bedrooms: Studio \r\n";
		else
			tooltip += "Bedrooms: " + (getBedrooms()  + ((dens!=0)? 0.5 : 0)) + "\r\n";

		if ((getBedrooms() == 0) && (int) getBathrooms() == 0) {
			tooltip = "";
			return tooltip;
		}

		if ((int) getBathrooms() == getBathrooms())
			tooltip += "Bathrooms: " + (int) getBathrooms() + "\r\n";
		else
			tooltip += "Bathrooms: " + getBathrooms() + "\r\n";

		if (getArea() > 0)
			tooltip += "Area: " + getArea() + " Sq.Ft.\r\n";
		// tooltip += "Floorplan: " + getFloorplan_url() + "\r\n";

		if (!getMLS().isEmpty())
			tooltip += "MLS#: " + getMLS() + "\r\n";
		return tooltip;
	}

	public String getVirtualTourURL() {
		return virtualTourURL;
	}

	public void setVirtualTourURL(String newUrl) {
		virtualTourURL = newUrl;
	}

	public String getMoreInfoURL() {
		return moreInfoURL;
	}

	public void setMoreInfoURL(String newUrl) {
		moreInfoURL = newUrl;
	}

	public String getMLS() {
		return mls;
	}

	public void setMLS(String newMLS) {
		mls = newMLS;
	}

}
