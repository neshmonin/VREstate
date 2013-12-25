package com.condox.ecommerce.client.tree.node;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.document.I_JSON;
import com.condox.ecommerce.client.tree.AbstractNode;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.presenter.ShowHistoryPresenter;
import com.condox.ecommerce.client.tree.view.ShowHistoryView;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PopupPanel;

public class ShowHistoryNode extends AbstractNode {

	public class HistoryTransaction implements I_JSON {
		private String date = "date";
		private String subject = null;
		private String target = null;
		private String operation = "operation";
		private Double amount = 0.0;
		private String extraTargetInfo = "";
		private String tooltip = "";
		private JSONObject backup;

		public String getDate() {
			return date;
		}

		public String getSubject() {
			if (subject != null && target != null)
				return subject + " (" + target + ")";
			return "";
		}

		public String getOperation() {
			return operation;
		}

		public Double getAmount() {
			return amount;
		}

		public String getTooltip() {
			if ("ViewOrder".equals(subject) && tooltip.isEmpty()) {
				String url = Options.URL_VRT + "data/vieworder/"
						+ extraTargetInfo + "?sid=" + User.SID;
				// final PopupPanel loading = new PopupPanel();
				// loading.clear();
				// loading.setModal(true);
				// loading.setGlassEnabled(true);
				// loading.add(new Label("Loading, please wait..."));
				// loading.center();
				GET.send(url, new RequestCallback() {

					@Override
					public void onResponseReceived(Request request,
							Response response) {
//						if (response.getStatusCode() != 200)
//							return;
						setTooltip(response.getStatusText());
//						JSONObject obj = JSONParser.parseStrict(
//								response.getText()).isObject();

						// String json = response.getText();
						// JSONObject obj =
						// JSONParser.parseLenient(json).isObject();
						// if (obj.containsKey("transactions"))
						// if (obj.get("transactions").isArray() != null) {
						// JSONArray items = obj.get("transactions").isArray();
						// List<HistoryTransaction> result = new
						// ArrayList<HistoryTransaction>();
						// for (int i = items.size() - 1; i >= 0; i--) {
						// HistoryTransaction newTransaction = node.new
						// HistoryTransaction();
						// newTransaction.fromJSONObject(items.get(i).isObject());
						// result.add(newTransaction);
						// }
						// display.setHistoryData(result);
						// // loading.hide();
						// }
					}

					@Override
					public void onError(Request request, Throwable exception) {

					}
				});
				setTooltip("Loading...");
			}
			return tooltip;
		}

		private void setTooltip(String newTooltip) {
			tooltip = newTooltip;
		}

		@Override
		public JSONObject toJSONObject() {
			JSONObject obj = new JSONObject();
			obj = backup;
			return obj;
		}

		@Override
		public void fromJSONObject(JSONObject obj) {
			// Log.write(obj.toString());
			backup = obj;
			if (obj.get("created") != null)
				if (obj.get("created").isString() != null) {
					String str = obj.get("created").isString().stringValue();
					str = str.replace("/", "");
					str = str.replace("Date", "");
					str = str.replace("(", "");
					str = str.replace(")", "");
					Date date = new Date();
					date.setTime(Long.valueOf(str));
					this.date = date.toString();
				}
			if (obj.get("operation") != null)
				if (obj.get("operation").isString() != null)
					operation = obj.get("operation").isString().stringValue();

			if (obj.get("amount") != null)
				if (obj.get("amount").isNumber() != null)
					amount = obj.get("amount").isNumber().doubleValue();

			if (obj.get("extraTargetInfo") != null)
				if (obj.get("extraTargetInfo").isString() != null)
					extraTargetInfo = obj.get("extraTargetInfo").isString()
							.stringValue().replace("-", "");

			if (obj.get("subject") != null)
				if (obj.get("subject").isString() != null)
					subject = obj.get("subject").isString().stringValue();

			if (obj.get("target") != null)
				if (obj.get("target").isString() != null)
					target = obj.get("target").isString().stringValue();
		}

	}

	@Override
	public void go(final EcommerceTree tree) {
		super.go(tree);
		ShowHistoryPresenter presenter = new ShowHistoryPresenter(
				new ShowHistoryView(), this);
		presenter.go(tree.container);
	}
}
