package com.condox.ecommerce.client.tree.api;

import com.google.gwt.json.client.JSONObject;

public interface I_RequestCallback {
	void onOK(JSONObject result);
	void onError();
}
