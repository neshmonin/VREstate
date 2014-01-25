package com.condox.ecommerce.client.tree.presenter;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.communication.GET;
import com.condox.clientshared.communication.Options;
import com.condox.clientshared.communication.User;
import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.document.BuildingInfo;
import com.condox.clientshared.document.HistoryTransactionInfo;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.condox.ecommerce.client.tree.EcommerceTree.Actions;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class HistoryPresenter implements I_Presenter, I_GetHistoryHandler  {

	public static interface I_Display extends I_Contained {
		void setPresenter(HistoryPresenter presenter);

		void setHistoryTransactions(List<HistoryTransactionInfo> transactions);

		void setAddress(int index, String value);

		Widget asWidget();
		void setSuiteInfo(SuiteInfo info);
		void setBuildingInfo(BuildingInfo info);
	}

	private I_Display display = null;
	private EcommerceTree tree = null;
	private List<HistoryTransactionInfo> transactions = new ArrayList<HistoryTransactionInfo>();

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add((I_Contained) display);

		String url = Options.URL_VRT;
		url += "data/ft?";
		url += "&userId=" + User.id;
		url += "&sid=" + User.SID;

		GET.send(url, new GetHistoryHandler(this));
	}

	@Override
	public void onGetHistoryOk(String response) {
		String json = response;
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		transactions.clear();
		if (obj.containsKey("transactions"))
			if (obj.get("transactions").isArray() != null) {
				JSONArray items = obj.get("transactions").isArray();
				for (int i = 0; i < 100/* items.size() */; i++) {
					HistoryTransactionInfo transaction = HistoryTransactionInfo
							.fromJSON(items.get(i).isObject());
					transactions.add(transaction);
				}
				display.setHistoryTransactions(transactions);

			}
	}

	@Override
	public void onGetHistoryError() {
		// TODO Auto-generated method stub

	}
	
	public void getSuiteInfo(int id) {
		String url = Options.URL_VRT //
				+ "data/suite/" + id //
				+ "?&sid=" + User.SID; //
		GET.send(url, new RequestCallback(){

			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				SuiteInfo info = new SuiteInfo();
				info.fromJSONObject(JSONParser.parseStrict(response.getText()).isObject());
				display.setSuiteInfo(info);
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
//		GET.send(url, new GetSuiteInfoHandler(this));
	}
	
	public void getBuildingInfo(int id) {
		String url = Options.URL_VRT //
				+ "data/building/" + id //
				+ "?&sid=" + User.SID; //
		GET.send(url, new RequestCallback(){
			
			@Override
			public void onResponseReceived(Request request, Response response) {
				// TODO Auto-generated method stub
				BuildingInfo info = new BuildingInfo();
				info.fromJSONObject(JSONParser.parseStrict(response.getText()).isObject());
				display.setBuildingInfo(info);
			}
			
			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub
				
			}});
//		GET.send(url, new GetSuiteInfoHandler(this));
	}
	
	
	
	
	
	
	
	
//	
//	@Override
//	public void onGetSuiteInfoOk(String response) {
//		display.setSuiteInfo(response);
//		
//	}
//
//	@Override
//	public void onGetSuiteInfoError() {
//		// TODO Auto-generated method stub
//		
//	}

	// private GetHistoryHandler onGetHistoryHandler = new GetHistoryHandler();

	// private HistoryTransactionInfo transaction = null;
	//
	// private RequestCallback onGetHistoryHandler = new RequestCallback() {
	//
	// @Override
	// public void onResponseReceived(Request request, Response response) {
	// Log.write(response.getText());
	// String json = response.getText();
	// JSONObject obj = JSONParser.parseLenient(json).isObject();
	// if (obj.containsKey("transactions"))
	// if (obj.get("transactions").isArray() != null) {
	// JSONArray items = obj.get("transactions").isArray();
	// // String html = "";
	// for (int i = 0; i < 20/*items.size()*/ ; i++) {
	// HistoryTransactionInfo transaction = HistoryTransactionInfo
	// .fromJSON(items.get(i).isObject());
	// transactions.add(transaction);
	// }
	// if (!transactions.isEmpty()) {
	// transaction = transactions.get(0);
	// // getSubjectData();
	// }
	// display.setHistoryTransactions(transactions);
	//
	// }
	// // TODO Auto-generated method stub
	//
	// }
	//
	// @Override
	// public void onError(Request request, Throwable exception) {
	// // TODO Auto-generated method stub
	//
	// }
	// };
	//
	// public void getAddress(final int index) {
	// // if ("ViewOrder".equals(info.getSubject())) {
	// // // String url = Options.URL_VRT + "data/viewOrder/" //
	// // // + transaction.getExtraTargetInfo().replace("-", "") //
	// // // + "?userId=" + User.id //
	// // // + "&ed=6" //
	// // // + "&sid=" + User.SID; //
	// // // GET.send(url, new RequestCallback(){
	// // //
	// // // @Override
	// // // public void onResponseReceived(Request request,
	// // // Response response) {
	// // //
	// // //
	// // // }
	// // //
	// // // @Override
	// // // public void onError(Request request, Throwable exception) {
	// // // // TODO Auto-generated method stub
	// // //
	// // // }});
	// //
	// // }
	// if ("Suite".equals(transactions.get(index).getTarget())) {
	// String url = Options.URL_VRT//
	// + "data/suite/" + transactions.get(index).getTargetId() //
	// + "?&sid=" + User.SID; //
	// GET.send(url, new RequestCallback() {
	//
	// @Override
	// public void onResponseReceived(Request request,
	// Response response) {
	// // int index = transactions.indexOf(transaction);
	// // index++;
	// // if (index < transactions.size()) {
	// // transaction = transactions.get(index);
	// // }
	//
	// SuiteInfo info = SuiteInfo.fromJSON(JSONParser
	// .parseLenient(response.getText()).isObject());
	// int buildingId = info.getBuildingId();
	// String url = Options.URL_VRT + "data/building/" + buildingId//
	// +"?sid=" + User.SID;//
	// GET.send(url, new RequestCallback(){
	//
	// @Override
	// public void onResponseReceived(Request request,
	// Response response) {
	// BuildingInfo info = new BuildingInfo();
	// info.fromJSONObject(JSONParser.parseStrict(response.getText()).isObject());
	//
	// // String str = transactions.get(index).getDate();
	// // str = str.replace("/Date(", "");
	// // str = str.replace(")/", "");
	// // Date date = new Date(Integer.valueOf(str));
	// // display.setAddress(index, date.
	// // + " - " + info.getName()+", "+info.getStreet()+", "+ info.getCity());
	// // Log.write(response.getText());
	// }
	//
	// @Override
	// public void onError(Request request, Throwable exception) {
	// // TODO Auto-generated method stub
	//
	// }});
	//
	//
	// }
	//
	// @Override
	// public void onError(Request request, Throwable exception) {
	// // TODO Auto-generated method stub
	//
	// }
	// });
	//
	// }
	// }
	//
	// // private void getSubjectData() {
	// // // Log.write(transaction.getSubject());
	// // if ("ViewOrder".equals(transaction.getSubject())) {
	// // String url = Options.URL_VRT + "data/viewOrder/" //
	// // + transaction.getExtraTargetInfo().replace("-", "") //
	// // + "?userId=" + User.id //
	// // + "&ed=6" //
	// // + "&sid=" + User.SID; //
	// // GET.send(url, onGetViewOrder);
	// // }
	// // }
	// //
	// // private RequestCallback onGetViewOrder = new RequestCallback() {
	// //
	// // @Override
	// // public void onResponseReceived(Request request, Response response) {
	// // String json = response.getText();
	// // JSONObject obj = JSONParser.parseStrict(json).isObject(); //
	// // transaction.setSubjectData(obj);
	// // getTargetData();
	// //
	// // }
	// //
	// // @Override
	// // public void onError(Request request, Throwable exception) {
	// // // TODO Auto-generated method stub
	// //
	// // }
	// // };
	// //
	// // private void getTargetData() {
	// // if ("Suite".equals(transaction.getTarget())) {
	// // String url = Options.URL_VRT;
	// // url += "data/suite/" + transaction.getTargetId();
	// // url += "?sid=" + User.SID;
	// // GET.send(url, onGetSuite);
	// // }
	// // }
	// //
	// // private RequestCallback onGetSuite = new RequestCallback() {
	// //
	// // @Override
	// // public void onResponseReceived(Request request, Response response) {
	// // JSONObject obj = JSONParser.parseStrict(response.getText())
	// // .isObject();
	// // transaction.setTargetData(obj);
	// //
	// // int index = transactions.indexOf(transaction);
	// // index++;
	// // if (index < transactions.size()) {
	// // transaction = transactions.get(index);
	// // getSubjectData();
	// // } else
	// // display.setHistoryTransactions(transactions);
	// // }
	// //
	// // @Override
	// // public void onError(Request request, Throwable exception) {
	// // // TODO Auto-generated method stub
	// //
	// // }
	// // };

	// Events
	public void onClose() {
		tree.next(Actions.Close);
	}

	@Override
	public void setView(Composite view) {
		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

}
