package com.condox.order.client.context;

import java.util.HashMap;
import java.util.Map;

import com.condox.order.client.utils.Log;

public class BaseContext implements IContext {

	private Map<String, String> data = new HashMap<String, String>();
	
	@Override
	public String getValue(String key) {
		return data.get(key);
	}

	@Override
	public void setValue(String key, String value) {
		data.put(key, value);
	}

	@Override
	public boolean equals(Object obj) {
		Boolean result = data.equals(((BaseContext)obj).data);
		Log.write("equals: " + result);
		return result;
//		return super.equals(obj);
	}
	
	@Override
	public int hashCode() {
//		return super.hashCode();
		return data.hashCode();
//		return getString().hashCode();
	}

	@Override
	public String toString() {
		return data.toString();
	}

	@Override
	public Boolean containsValue(String key) {
		return data.containsKey(key);
	}

	private IContext.Types type;
	
	@Override
	public Types getType() {
		// TODO Auto-generated method stub
		return type;
	}
	
	public BaseContext(Types type) {
		this.type = type;
	}
	
	
	
	

}
