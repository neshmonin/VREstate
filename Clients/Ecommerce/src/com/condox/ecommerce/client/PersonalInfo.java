package com.condox.ecommerce.client;

import com.condox.clientshared.document.I_JSON;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;

public class PersonalInfo implements I_JSON {
	private String firstName = "";
	private String lastName = "";
	private String email = "";
	private String phone = "";
	
	public String getFirstName() {
		return firstName;
	}

	public String getLastName() {
		return lastName;
	}

	public String getEmail() {
		return email;
	}

	public String getPhone() {
		return phone;
	}

	public void setFirstName(String firstName) {
		this.firstName = firstName;
	}

	public void setLastName(String lastName) {
		this.lastName = lastName;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public void setPhone(String phone) {
		this.phone = phone;
	}

	@Override
	public JSONObject toJSONObject() {
		JSONObject obj = new JSONObject();
		obj.put("firstName", new JSONString(firstName));
		obj.put("lastName", new JSONString(lastName));
		obj.put("email", new JSONString(email));
		obj.put("phone", new JSONString(phone));
		return obj;
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
		if (obj == null)
			return;
		
		if (obj.get("firstName") != null)
			if (obj.get("firstName").isString() != null)
				firstName = obj.get("firstName").isString().stringValue();
		
		if (obj.get("lastName") != null)
			if (obj.get("lastName").isString() != null)
				lastName = obj.get("lastName").isString().stringValue();

		if (obj.get("email") != null)
			if (obj.get("email").isString() != null)
				email = obj.get("email").isString().stringValue();
		
		if (obj.get("phone") != null)
			if (obj.get("phone").isString() != null)
				phone = obj.get("phone").isString().stringValue();
	}
}
