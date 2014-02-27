package com.condox.ecommerce.client.model;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.User.UserRole;
import com.condox.clientshared.document.I_JSON;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;

public class LoginModel implements I_JSON {
	private String uid = "";
	private String pwd = "";
	private UserRole role = UserRole.SellingAgent;
	private String sid = "";
	
	public String getUid() {
		if (uid.isEmpty() && pwd.isEmpty())
			return "web";
		return uid;
	}
	public void setUid(String uid) {
		this.uid = uid;
	}
	public String getPwd() {
		if (uid.isEmpty() && pwd.isEmpty())
			return "web";
		return pwd;
	}
	public void setPwd(String pwd) {
		this.pwd = pwd;
	}
	public UserRole getRole() {
		if (uid.isEmpty() && pwd.isEmpty())
			return UserRole.Visitor;
		return role;
	}
	public void setRole(UserRole role) {
		this.role = role;
	}
	public String getSid() {
		return sid;
	}
	public void setSid(String sid) {
		this.sid = sid;
	}
	@Override
	public JSONObject toJSONObject() {
		// TODO Auto-generated method stub
		JSONObject obj = new JSONObject();
		obj.put("uid", new JSONString(uid));
		obj.put("pwd", new JSONString(pwd));
		obj.put("role", new JSONString(role.name()));
		obj.put("sid", new JSONString(sid));
		return obj;
	}
	@Override
	public void fromJSONObject(JSONObject json) {
		if (json == null)
			return;
		
		// uid
		if (json.containsKey("uid"))
			if (json.get("uid").isString() != null)
				uid = json.get("uid").isString().stringValue();
		
		// pwd
		if (json.containsKey("pwd"))
			if (json.get("pwd").isString() != null)
				pwd = json.get("pwd").isString().stringValue();
		
		// role
		if (json.containsKey("role"))
			if (json.get("role").isString() != null)
				role = UserRole.valueOf(json.get("role").isString().stringValue());
		
		// sid
		if (json.containsKey("sid"))
			if (json.get("sid").isString() != null)
				sid = json.get("sid").isString().stringValue();

	}
	
	
}
