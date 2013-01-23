package com.condox.vrestate.client.document;

import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

public class ViewOrder {
	/*
    {
      "id":"61f2f27e-cc8f-4df1-b8c1-f825c059ba0e", 
      "targetObjectType":"Suite", 
      "targetObjectId":3185, 
      "product":"ExternalTour", 
      "mlsId":"", 
      "productUrl":"http://www.venturehomes.ca/TREBTour.asp?TourID=28924"
    }
	 */
	
	public enum TargetType
	{
		None,
		Suite,
		Building,
		Site
	}
	
	public enum ProductType
	{
		None,
		ExternalTour
	}
	
	private String id = "";
	private TargetType targetObjectType = TargetType.None; 
	private int targetObjectId = -1;
	private ProductType product = ProductType.None; 
	private String productUrl = "";
	
	public void Parse(JSONValue value) {
		JSONObject obj = value.isObject();
		id = obj.get("id").isString().stringValue();

		if (obj.get("targetObjectType").isString() != null) {
			String status = obj.get("targetObjectType").isString().stringValue();
			if (status.equals("Suite")) this.targetObjectType = TargetType.Suite;
			else if (status.equals("Building")) this.targetObjectType = TargetType.Building;
			else if (status.equals("Site")) this.targetObjectType = TargetType.Site;
		}
		
		targetObjectId = (int) obj.get("targetObjectId").isNumber().doubleValue();

		if (obj.get("product").isString() != null) {
			String product = obj.get("product").isString().stringValue();
			if (product.equals("ExternalTour")) this.product = ProductType.ExternalTour;
		}
		
		if (obj.get("productUrl").isString() != null)
			productUrl = obj.get("productUrl").isString().stringValue();
	}

	public String getId() {
		return id;
	}

	public TargetType getTargetObjectType() {
		return targetObjectType;
	}

	public int getTargetObjectId() {
		return targetObjectId;
	}

	public ProductType getProduct() {
		return product;
	}

	public String getProductUrl() {
		return productUrl;
	}

}
