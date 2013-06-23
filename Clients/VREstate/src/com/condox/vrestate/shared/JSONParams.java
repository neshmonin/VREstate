package com.condox.vrestate.shared;

import com.google.gwt.core.client.JavaScriptObject;
import com.google.gwt.core.client.JsArray;

public class JSONParams extends JavaScriptObject {

	// Overlay types always have protected, zero-arg ctors
	protected JSONParams() {
	}

	// Typically, methods on overlay types are JSNI
	public final native String getString(String key) /*-{
		return this[key];
	}-*/;
	
	public final native int getInteger(String key) /*-{
		return this[key];
	}-*/;
	
	public final native Double getDouble(String key) /*-{
		if (this[key] === undefined)
			return null;
		else
			return @com.condox.vrestate.client.JO::Double(D)(this[key]);
	}-*/;
	
	public final native JsArray<JSONParams> getArray(String key) /*-{
		if (this[key] === undefined)
			return null;
		else
			return this[key];
	}-*/;
	
	public final native double getDouble(Integer index) /*-{
	if (this[index] === undefined)
		return 0;
	if (this[index] === null)
		return 0;
		return this[index];
	}-*/;
	
	public final native double getLength() /*-{
		return this.length;
	}-*/;
	
	public final native JSONParams getParams(String key) /*-{
		return this[key];
	}-*/;
	

	public static native JSONParams parse(String json) /*-{
		eval(" var result =  " + json);
		return result;
	}-*/;
}
