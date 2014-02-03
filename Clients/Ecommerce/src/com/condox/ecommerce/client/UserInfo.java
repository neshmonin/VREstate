package com.condox.ecommerce.client;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.document.I_JSON;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;

public class UserInfo implements I_JSON {

	private Integer id = null;
	private String login = "";
	private String password = "";
	private User.UserRole role = null;
	// private String version = null; // ??
	private Integer estateDeveloperId = null;
	private String nickName = "";
	private String primaryEmail = "";
	private String timeZone = null;
	private Integer loginType = null;

//	public UserInfo(JSONObject source) {
//		this.fromJSONObject(source);
//	}
	
	public Integer getId() {
		return id;
	}

	public void setId(int newId) {
		id = newId;
	}

	public String getLogin() {
		return login;
	}

	public void setLogin(String login) {
		this.login = login;
	}

	public String getPassword() {
		return password;
	}

	public User.UserRole getRole() {
		return role;
	}

	public void setPassword(String password) {
		this.password = password;
	}

	public void setRole(User.UserRole role) {
		this.role = role;
	}

	public String getEmail() {
		return primaryEmail;
	}

	public void setEmail(String newEmail) {
		primaryEmail = newEmail;
	}

	public String getNickName() {
		return nickName;
	}

	public void setNickName(String newNickName) {
		nickName = newNickName;
	}

	// Utils

	private String getString(JSONObject obj, String key) {
		if (obj != null)
			if (obj.get(key) != null)
				if (obj.get(key).isString() != null)
					return obj.get(key).isString().stringValue();
		return null;
	}

	private void setString(JSONObject obj, String key, String value) {
		if (key != null && !key.isEmpty())
			if (value != null)
				obj.put(key, new JSONString(value));
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
		login = getString(obj, "login");
		password = getString(obj, "password");
		
		if (getString(obj, "role") != null)
			role = User.UserRole.valueOf(getString(obj, "role"));

		nickName = getString(obj, "nickName");
		primaryEmail = getString(obj, "primaryEmail");
	}

	@Override
	public JSONObject toJSONObject() {
		JSONObject obj = new JSONObject();
		setString(obj, "login", login);
		setString(obj, "password", password);

		setString(obj, "role", role.name());

		setString(obj, "nickName", nickName);
		setString(obj, "primaryEmail", primaryEmail);

		return obj;
	}

}
