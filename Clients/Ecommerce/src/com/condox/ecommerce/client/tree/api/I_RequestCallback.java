package com.condox.ecommerce.client.tree.api;

import com.google.gwt.json.client.JSONObject;

public interface I_RequestCallback {
	public void onSuccess(JSONObject obj);
	public void onError(String errMessage);
}
