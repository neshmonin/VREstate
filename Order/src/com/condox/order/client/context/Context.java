package com.condox.order.client.context;

import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Map.Entry;

public abstract class Context implements IContext {
	private Map<String, String> data = new HashMap<String, String>();

	public Object getData() {
		return this.data;
	}

	public void setValue(String key, String value) {
		this.data.put(key, value);
	}

	public String getValue(String key) {
		if (data.containsKey(key))
			return data.get(key);
		return null;
	}

	@Override
	public String getString() {
		String result = "";
		Iterator<Entry<String,String>> iter = data.entrySet().iterator();
		while (iter.hasNext()) {
			Entry<String,String> entry = iter.next();
			result += " - " + entry.getKey() + " - " + entry.getValue() + " - ";
		}
		return result;
	}

	@Override
	public Boolean isValid() {
		return true;
	}

	@Override
	public String getName() {
		return "";
	}

	@Override
	public ContextType getType() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public boolean equals(Object obj) {
//		return super.equals(obj);
		Boolean result = getData().equals(((IContext)obj).getData());
		Log.write(result.toString());
		return result;
//		Log.write("1: " + getString());
//		Log.write("2: " + ((IContext)obj).getString());
//		Log.write("result: " + getString().equals(((IContext)obj).getString()));
//		return getString().equals(((IContext)obj).getString());
	}

	@Override
	public int hashCode() {
//		return super.hashCode();
		return getData().hashCode();
//		return getString().hashCode();
	}
	
	
	
	
	

}
