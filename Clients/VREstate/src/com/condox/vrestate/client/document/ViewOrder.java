package com.condox.vrestate.client.document;

import com.condox.vrestate.client.document.I_VRObject.VRObjectType;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

public class ViewOrder {
	/*
    {
      "id": "ffda6616-daa1-4978-8dbf-4acd732de044", 
      "targetObjectType": "Suite", 
      "targetObjectId": 7678, 
      "product": "PublicListing", 
      "options": "FloorPlan", 
      "infoUrl": "", 
      "vTourUrl": "http://www.venturehomes.ca/TREBTour.asp?TourID=28924"
    }
	 */
	
	public enum Options
	{
		None,
		ExternalTour
	}
	
	public enum ProductType
	{
		None,
		PrivateListing,
		PublicListing,
		Building3DLayout,
		Banner /* not supported yet */
	}
	
	private String id = "";
	private VRObjectType targetObjectType = VRObjectType.None; 
	private int targetObjectId = -1;
	private Options options = Options.None; 
	private String vTourUrl = null;
	private String infoUrl = null;
	private ProductType productType = ProductType.None;
	
	public void Parse(JSONValue value) {
		JSONObject obj = value.isObject();
		id = obj.get("id").isString().stringValue();

		if (obj.get("product").isString() != null) {
			String productTypeStr = obj.get("product").isString().stringValue();
			if (productTypeStr.equals("PrivateListing")) 
				this.productType = ProductType.PrivateListing;
			else if (productTypeStr.equals("PublicListing")) 
				this.productType = ProductType.PublicListing;
			else if (productTypeStr.equals("Building3DLayout")) 
				this.productType = ProductType.Building3DLayout;
			else if (productTypeStr.equals("Banner")) 
				this.productType = ProductType.Banner;
		}
		
		
		if (obj.get("targetObjectType").isString() != null) {
			String targetObjectType = obj.get("targetObjectType").isString().stringValue();
			if (targetObjectType.equals("Suite")) 
				this.targetObjectType = VRObjectType.Suite;
			else if (targetObjectType.equals("Building")) 
				this.targetObjectType = VRObjectType.Building;
			else if (targetObjectType.equals("Site")) 
				this.targetObjectType = VRObjectType.Site;
		}
		
		targetObjectId = (int) obj.get("targetObjectId").isNumber().doubleValue();

		if (obj.get("vTourUrl").isString() != null) {
			vTourUrl = obj.get("vTourUrl").isString().stringValue();
			if (vTourUrl != null && vTourUrl != "")
				this.options = Options.ExternalTour;
		}

		if (obj.get("infoUrl").isString() != null) {
			infoUrl = obj.get("infoUrl").isString().stringValue();
		}
	}

	public String getId() {
		return id;
	}

	public VRObjectType getTargetObjectType() {
		return targetObjectType;
	}

	public int getTargetObjectId() {
		return targetObjectId;
	}

	public Options getOptions() {
		return options;
	}

	public String getVTourUrl() {
		return vTourUrl;
	}

	private I_VRObject targetObject = null;
	public I_VRObject getTargetObject() {
		return targetObject;
	}

	public void setTargetObject(I_VRObject targetObject) {
		this.targetObject = targetObject;
	}

	public ProductType getProductType() {
		return productType;
	}

	public String getInfoUrl() {
		return infoUrl;
	}
}
