package com.condox.ecommerce.client;

import com.condox.clientshared.communication.User;
import com.condox.clientshared.document.I_JSON;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;

public class UserInfo implements I_JSON {
	/*
	 * {"id":75, "version":[0,0,0,0,0,61,12,241], "estateDeveloperId":null,
	 * "role":"Agent", "nickName":"eugene", "primaryEmail":"simonov",
	 * "timeZone":"", "brokerage": { "id":1, "version":[0,0,0,0,0,61,12,242],
	 * "name":"Andrew's Real Estate", "streetAddress":"4900 Glen Erin Dr",
	 * "city":"Mississauga", "stateProvince":"ON", "postalCode":"L5M7S2",
	 * "country":"Canada", "phoneNumbers":["+16475015545"],
	 * "emails":["andrey.masliuk@3dcondox.com"],
	 * "webSite":"http://3dcondox.com"}, "passwordChangeRequired":false,
	 * "creditUnits":0, "loginType":0, "login":"Eugene"}
	 */

	private Integer id = null;
	private String login = "";
	private String password = "";
	private User.UserRole role = null;
	private String primaryEmail = null;
	// private String version = null; // ??
	private Integer estateDeveloperId = null;
	private String nickName = "";
	private String timeZone = null;
	private Integer loginType = null;
	// -------------------------------
	private String firstName = null;
	private String lastName = null;
	private String email = null;
	private String phone = null;
	//-------------------------------
	private PersonalInfo personalInfo = new PersonalInfo();
	
	public PersonalInfo getPersonalInfo() {
		return personalInfo;
	}
	
	public void setPersonalInfo(PersonalInfo newPersonalInfo) {
		personalInfo = newPersonalInfo;
	}

	// public UserInfo(JSONObject source) {
	// this.fromJSONObject(source);
	// }

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

	// public String getEmail() {
	// return primaryEmail;
	// }

	// public void setEmail(String newEmail) {
	// primaryEmail = newEmail;
	// }

	// public String getNickName() {
	// return nickName;
	// }

	public void setNickName(String newNickName) {
		nickName = newNickName;
	}

	public String getFirstName() {
		return firstName;
	}

	public void setFirstName(String newFirstName) {
		firstName = newFirstName;
	}

	public String getLastName() {
		return lastName;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String newEmail) {
		this.email = newEmail;
	}

	public String getPhone() {
		return phone;
	}

	// Utils

	public void setLastName(String lastName) {
		this.lastName = lastName;
	}

	public void setPhone(String phone) {
		this.phone = phone;
	}

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
		try {
			personalInfo.fromJSONObject(JSONParser.parseStrict(getString(obj, "personalInfo")).isObject());
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
//		personalInfo.fromJSONObject(obj.get("personalInfo")));
		// --------------------------------------------
		firstName = "<none>";
		lastName = "<none>";
		String n = getString(obj, "n");
		if (n != null) {
			String[] s = n.split(";");
			lastName = s[0];
			firstName = (s.length > 1) ? s[1] : firstName;
		}

		email = getString(obj, "email");
		email = (email != null) ? email : "<none>";

		phone = getString(obj, "tel");
		phone = (phone != null) ? phone : "<none>";
	}

	@Override
	public JSONObject toJSONObject() {
		JSONObject obj = new JSONObject();
		setString(obj, "login", login);
		setString(obj, "password", password);

		setString(obj, "role", role.name());

		setString(obj, "nickName", nickName);
		setString(obj, "primaryEmail", primaryEmail);
//		obj.put("personalInfo", personalInfo.toJSONObject());
		setString(obj, "personalInfo", personalInfo.toJSONObject().toString());
		// --------------------------------------------
		setString(obj, "n", lastName + ";" + firstName);
		setString(obj, "email", email);
		setString(obj, "tel", phone);

		return obj;
	}
}
