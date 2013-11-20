package com.condox.clientshared.document;

import com.google.gwt.json.client.JSONObject;

public class ViewOrder implements I_JSON{
	/*
	 * { "id": "ffda6616-daa1-4978-8dbf-4acd732de044", "targetObjectType":
	 * "Suite", "targetObjectId": 7678, "product": "PublicListing", "options":
	 * "FloorPlan", "infoUrl": "", "vTourUrl":
	 * "http://www.venturehomes.ca/TREBTour.asp?TourID=28924" }
	 */

	public enum Options {
		None, ExternalTour
	}

	public enum ProductType {
		None, PrivateListing, PublicListing, Building3DLayout, Banner
		/* not supported yet */
	}

	private String id = "";
	private I_VRObject.VRObjectType targetObjectType = I_VRObject.VRObjectType.None;
	private int targetObjectId = -1;
	private Options options = Options.None;
	private String vTourUrl = null;
	private String infoUrl = null;
	private ProductType productType = ProductType.None;

	@Override
	public JSONObject toJSONObject() {
		return null;
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
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
			String targetObjectType = obj.get("targetObjectType").isString()
					.stringValue();
			if (targetObjectType.equals("Suite"))
				this.targetObjectType = I_VRObject.VRObjectType.Suite;
			else if (targetObjectType.equals("Building"))
				this.targetObjectType = I_VRObject.VRObjectType.Building;
			else if (targetObjectType.equals("Site"))
				this.targetObjectType = I_VRObject.VRObjectType.Site;
		}

		targetObjectId = (int) obj.get("targetObjectId").isNumber()
				.doubleValue();

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

	public I_VRObject.VRObjectType getTargetObjectType() {
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
