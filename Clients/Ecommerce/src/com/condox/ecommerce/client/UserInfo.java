package com.condox.ecommerce.client;

import com.condox.clientshared.document.I_JSON;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;

public class UserInfo implements I_JSON {
//	public enum Types {
//		Guest, Admin
//	}
//	private Types type = Types.Guest;
//	public setType 
	
	

	private Integer id = null;
	// private String version = null; // ??
	private Integer estateDeveloperId = null;
	private String role = null;
	private String nickName = "";
	private String primaryEmail = null;
	private String timeZone = null;
	private Integer loginType = null;
	private String login = "";
	private JSONObject backup = new JSONObject(); // TODO remove

	public Integer getId() {
		return id;
	}

	public void setId(int newId) {
		id = newId;
	}

	public String getNickName() {
		return nickName;
	}

	public void setNickName(String newNickName) {
		nickName = newNickName;
	}

	public String getEmail() {
		return primaryEmail;
	}

	public void setEmail(String newEmail) {
		primaryEmail = newEmail;
	}

	@Override
	public JSONObject toJSONObject() {
		JSONObject obj = new JSONObject();
		obj = backup; // TODO remove
		obj.put("nickName", new JSONString(nickName));
		obj.put("primaryEmail", new JSONString(primaryEmail));
		return obj;
	}

	@Override
	public void fromJSONObject(JSONObject json) {
		backup = json; // TODO remove
		nickName = json.get("nickName").isString().stringValue();
		primaryEmail = json.get("primaryEmail").isString().stringValue();
	}

}
