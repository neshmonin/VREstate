package com.condox.clientshared.document;

import java.util.Calendar;
import java.util.Date;

import com.condox.clientshared.abstractview.Log;
import com.google.gwt.json.client.JSONObject;

public class HistoryTransactionInfo implements I_JSON {
	private String date = ""; // TODO remake to Date
	private String systemRefId = "";
	private int initiatorId = 0;
	private String account = "";
	private int accountId = 0;
	private String operation = "";
	private double amount = 0;
	private String subject = "";
	private String target = "";
	private int targetId = 0;
	private String extraTargetInfo = "";

	public static HistoryTransactionInfo fromJSON(JSONObject obj) {
		HistoryTransactionInfo info = new HistoryTransactionInfo();
		info.fromJSONObject(obj);
		return info;
	}

	@Override
	public JSONObject toJSONObject() {
		JSONObject obj = new JSONObject();
		return obj;
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
		Log.write(obj.toString());
		if (obj != null)
			if (obj.get("created") != null)
				if (obj.get("created").isString() != null) {
					date = obj.get("created").isString().stringValue();
					date = date.replace("/Date(", "").replace(")/", "");

					String[] params = new Date(Long.parseLong(this.date))
							.toString().split(" ");
					// Formatting date
					date = "";
					date += params[2] + " ";
					date += params[1] + " ";
					date += params[5] + ", ";
					date += params[3].split(":")[0] + ":";
					date += params[3].split(":")[1] + ":";
					date += params[3].split(":")[2];
				}
		if (obj != null)
			if (obj.get("operation") != null)
				if (obj.get("operation").isString() != null)
					operation = obj.get("operation").isString().stringValue();
		if (obj != null)
			if (obj.get("subject") != null)
				if (obj.get("subject").isString() != null)
					subject = obj.get("subject").isString().stringValue();
		if (obj != null)
			if (obj.get("target") != null)
				if (obj.get("target").isString() != null)
					target = obj.get("target").isString().stringValue();

		if (obj != null)
			if (obj.get("targetId") != null)
				if (obj.get("targetId").isNumber() != null)
					targetId = (int) obj.get("targetId").isNumber()
							.doubleValue();
		if (obj != null)
			if (obj.get("extraTargetInfo") != null)
				if (obj.get("extraTargetInfo").isString() != null)
					extraTargetInfo = obj.get("extraTargetInfo").isString()
							.stringValue();
	}

	public String getDate() {
		return date;
	}

	public void setDate(String date) {
		this.date = date;
	}

	public String getSystemRefId() {
		return systemRefId;
	}

	public void setSystemRefId(String systemRefId) {
		this.systemRefId = systemRefId;
	}

	public int getInitiatorId() {
		return initiatorId;
	}

	public void setInitiatorId(int initiatorId) {
		this.initiatorId = initiatorId;
	}

	public String getAccount() {
		return account;
	}

	public void setAccount(String account) {
		this.account = account;
	}

	public int getAccountId() {
		return accountId;
	}

	public void setAccountId(int accountId) {
		this.accountId = accountId;
	}

	public String getOperation() {
		return operation;
	}

	public void setOperation(String operation) {
		this.operation = operation;
	}

	public double getAmount() {
		return amount;
	}

	public void setAmount(double amount) {
		this.amount = amount;
	}

	public String getSubject() {
		return subject;
	}

	public void setSubject(String subject) {
		this.subject = subject;
	}

	public String getTarget() {
		return target;
	}

	public void setTarget(String target) {
		this.target = target;
	}

	public int getTargetId() {
		return targetId;
	}

	public void setTargetId(int targetId) {
		this.targetId = targetId;
	}

	public String getExtraTargetInfo() {
		return extraTargetInfo;
	}

	public void setExtraTargetInfo(String extraTargetInfo) {
		this.extraTargetInfo = extraTargetInfo;
	}

}
