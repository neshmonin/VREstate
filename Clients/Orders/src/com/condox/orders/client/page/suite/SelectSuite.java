package com.condox.orders.client.page.suite;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

import com.condox.orders.client.GET;
import com.condox.orders.client.IPage;
import com.condox.orders.client.Log;
import com.condox.orders.client.Options;
import com.condox.orders.client.Orders;
import com.condox.orders.client.User;
import com.condox.orders.client.page.building.Building;
import com.condox.orders.client.page.submit.Submit;
import com.google.gwt.cell.client.ClickableTextCell;
import com.google.gwt.cell.client.FieldUpdater;
import com.google.gwt.core.client.GWT;
import com.google.gwt.http.client.Request;
import com.google.gwt.http.client.RequestCallback;
import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.safehtml.shared.SafeHtml;
import com.google.gwt.safehtml.shared.SafeHtmlBuilder;
import com.google.gwt.text.shared.AbstractSafeHtmlRenderer;
import com.google.gwt.text.shared.SafeHtmlRenderer;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.cellview.client.Column;
import com.google.gwt.user.cellview.client.ColumnSortEvent.ListHandler;
import com.google.gwt.user.cellview.client.DataGrid;
import com.google.gwt.user.cellview.client.TextColumn;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.DialogBox;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.ListDataProvider;

public class SelectSuite extends Composite implements IPage {

	private static SelectSuiteUiBinder uiBinder = GWT
			.create(SelectSuiteUiBinder.class);
	@UiField(provided = true)
	DataGrid<Floor> dataGrid = new DataGrid<Floor>();

	interface SelectSuiteUiBinder extends UiBinder<Widget, SelectSuite> {
	}

	public SelectSuite() {
		instance = this;
		Init();
	}

	public SelectSuite(String firstName) {
		Init();
		instance = this;
	}

	private void Init() {
		initWidget(uiBinder.createAndBindUi(this));
		initNativeFuncs();
		String caption = "";
		if (Orders.selectedBuilding != null)
			caption = "Selected building: "
					+ Orders.selectedBuilding.getStreet() + ", "
					+ Orders.selectedBuilding.getName() + ".";
		// lblCaption.setText(caption + " Please, select suite:");
		// ------------------------
	}

	private native void initNativeFuncs()/*-{
		$wnd.onSelectSuite = function(id) {
			@com.condox.orders.client.page.suite.SelectSuite::onSelectSuite(I)(id);
		}
	}-*/;

	private static SelectSuite instance = null;

	private static void onSelectSuite(int id) {
		// Window.alert("++" + id);
		for (Floor floor : instance.dataProvider.getList())
			for (Suite suite : floor.suites)
				if (suite.getId() == id) {
					Orders.selectedSuite = suite;
					instance.dialogBox.setGlassEnabled(true);
					instance.dialogBox.setAnimationEnabled(true);
					instance.dialogBox.clear();
					instance.dialogBox
							.setWidget(new Submit(instance.dialogBox));
					instance.dialogBox.center();
					
					Building selectedBuilding = Orders.selectedBuilding;
					Suite selectedSuite = Orders.selectedSuite;
					String caption = selectedSuite.getName() + " - " +
							selectedBuilding.getStreet() + 
							"  (" + selectedBuilding.getName() + ")"; 
					
					instance.dialogBox.setText(caption);

					((Submit) instance.dialogBox.getWidget())
							.setFloorPlanUrl(suite.getFloorplan_url());

					instance.dialogBox.show();
					return;
				}
	}

	@Override
	public void Update() {
		Log.write("--");
		dataProvider.getList().clear();
		GetSuitesList();
	}

	private class Floor {
		private ArrayList<Suite> suites = new ArrayList<Suite>();
		private String name = "";

		public void add(Suite suite) {
			suites.add(suite);
			if (name.isEmpty())
				name = suite.getFloorName();
		}

		public void sort() {
			Collections.sort(suites, new Comparator<Suite>() {

				@Override
				public int compare(Suite arg0, Suite arg1) {
					int result = String.CASE_INSENSITIVE_ORDER.compare(
							arg0.getName(), arg1.getName());
					return result;
				}
			});
		}

		public String getName() {
			return name;
		}

		public String getValue() {
			sort();
			String result = "";
			for (Suite suite : suites) {
				String disabled = "";
				switch (suite.getStatus()) {
				case Sold:
					break;
				default:
					disabled += " disabled=\"true\" ";
				}

				result += "<button ";
				result += "type=\"button\"" + "onclick=\"onSelectSuite("
						+ suite.getId() + ")\"" + "style=\"width:50px\""
						+ "class=\"btnSelectSuite\"" + disabled + "title=\""
						+ suite.getTooltip() + "\">";
				result += suite.getName();
				result += "</button> ";

				// result = "";
				// result += "<img src=\"floor_but.png\" " +
				// "title=\"" + suite.getTooltip() + "\" " +
				// "onclick=\"onSelectSuite(" + suite.getId() + ")\"/>";
			}
			return result;
		}
	}

	private Column<Building, String> viewSuitesColumn;
	private Column<Floor, String> dataColumn;
	private ListDataProvider<Floor> dataProvider = new ListDataProvider<Floor>();

	// private FilteredListDataProvider<Building> dataProvider = new
	// FilteredListDataProvider<Building>(this);

	private void CreateDataGrid() {
		// ==============================================
		// dataGrid = new DataGrid<Building>();
		int count = dataGrid.getColumnCount();
		for (int i = 0; i < count; i++)
			dataGrid.removeColumn(0);

		ListHandler<Floor> sortHandler = new ListHandler<Floor>(
				dataProvider.getList());
		dataGrid.addColumnSortHandler(sortHandler);
		// buildings.setKeyboardSelectionPolicy(KeyboardSelectionPolicy.ENABLED);
		// -------------------------
		// -------------------------
		// //
		// Add a text column to show the no.
		TextColumn<Floor> nameColumn = new TextColumn<Floor>() {
			@Override
			public String getValue(Floor object) {
				return "Floor " + object.getName();
			}
		};
		nameColumn.setSortable(true);
		sortHandler.setComparator(nameColumn, new Comparator<Floor>() {
			@Override
			public int compare(Floor A, Floor B) {
				return A.getName().compareTo(B.getName());
			}
		});
		dataGrid.setColumnWidth(nameColumn, "200px");
		Building selectedBuilding = Orders.selectedBuilding;
		dataGrid.addColumn(nameColumn, selectedBuilding.getStreet());
//		dataGrid.addColumn(nameColumn, "Floor name");

		// -------------------------
		// Show names.
		SafeHtmlRenderer<String> anchorRenderer2 = new AbstractSafeHtmlRenderer<String>() {
			@Override
			public SafeHtml render(String object) {
				SafeHtmlBuilder sb = new SafeHtmlBuilder();
				sb.appendHtmlConstant(object);
				// for (int j = 0; j < 20; j++) {
				// sb.appendHtmlConstant(
				// "<button type=\"submit\" style=\"width:50px\">")
				// .appendEscaped(
				// String.valueOf((int) (Math.random() * 1000)))
				// .appendHtmlConstant("</button>");
				// }
				return sb.toSafeHtml();
			}
		};
		dataColumn = new Column<Floor, String>(new ClickableTextCell(
				anchorRenderer2)) {
			@Override
			public String getValue(Floor object) {
				return object.getValue();
				// return object.getName();
			}
		};
		dataColumn.setFieldUpdater(new FieldUpdater<Floor, String>() {
			@Override
			public void update(int index, Floor object, String value) {

				// dialogBox.setGlassEnabled(true);
				// dialogBox.setAnimationEnabled(true);
				// dialogBox.center();
				// dialogBox.show();
				// History.newItem("submit");
			}
		});
		// viewSuitesColumn2.setHorizontalAlignment(HasHorizontalAlignment.ALIGN_CENTER);
		// dataGrid.setColumnWidth(viewSuitesColumn2, "10em");
		// dataColumn.setCellStyleNames("header");
		dataGrid.addColumn(dataColumn, "");

		if (!dataProvider.getDataDisplays().contains(dataGrid)) {
			dataProvider.addDataDisplay(dataGrid);
		}
		// ColumnSortEvent.fire(dataGrid, dataGrid.getColumnSortList());
		// dataGrid.getColumnSortList().push(nameColumn);
	}

	private Building parentBuilding = null;

	class Params {
		private Map<String, String> params = new HashMap<String, String>();

		public Params(String str) {
			Log.write(str);
			String strParams = str.substring(str.indexOf("?") + 1);
			String[] arrParams = strParams.split("&");
			for (int i = 0; i < arrParams.length; i++) {
				String key = arrParams[i].split("=")[0];
				Log.write(key);
				String value = arrParams[i].split("=")[1];
				Log.write(value);
				params.put(key, value);
			}
		}

		public boolean containsKey(String key) {
			return params.containsKey(key);
		}

		public String getValue(String key) {
			if (containsKey(key))
				return params.get(key);
			else
				return "";
		}

	}

	private void GetSuitesList() {
		// for (int i = 0; i < 100; i++)
		// dataProvider.getList().add("" + i);
		// *************************************
		// Params params = new Params(History.getToken());

		// Map<String, String> params = new HashMap<String, String>();
		// String token = History.getToken();
		// token = token.split("?")[1];
		//
		// int id = -1;
		// if (params.containsKey("building"))
		// id = Integer.valueOf(params.getValue("building"));
		// else {
		// // There is no param named "building"
		// }
		// Log.write("id:" + id);
		// if (id != -1) {
		// Iterator<Building> iterator = Document.get().getBuildings()
		// .iterator();
		// while (iterator.hasNext()) {
		// Building currentBuilding = iterator.next();
		// if (currentBuilding.getId() == id) {
		// parentBuilding = currentBuilding;
		// break;
		// }
		// }
		// }

		Log.write("+");
		Log.write("" + parentBuilding);
		String url = Options.URL_VRT + "data/inventory?building="
				+ Orders.selectedBuilding.getId() + "&sid=" + User.SID;
		Log.write("+");
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				Log.write("answer:" + response.getText());
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("inventory").isArray();
				for (int index = 0; index < arr.size(); index++) {
					Suite newSuite = new Suite();
					newSuite.Parse(arr.get(index));
					String floorName = newSuite.getFloorName();
					Iterator<Floor> floors = dataProvider.getList().iterator();
					while (floors.hasNext()) {
						Floor floor = floors.next();
						if (floor.getName().equals(floorName)) {
							floor.add(newSuite);
							newSuite = null;
							break;
						}
					}
					if (newSuite != null) {
						Floor floor = new Floor();
						dataProvider.getList().add(floor);
						floor.add(newSuite);
					}
				}
				Log.write("Floors count: " + dataProvider.getList().size());
				Collections.sort(dataProvider.getList(),
						new Comparator<Floor>() {

							@Override
							public int compare(Floor o1, Floor o2) {
								int A = Integer.valueOf(o1.getName());
								int B = Integer.valueOf(o2.getName());
								return (A > B) ? 1 : -1;
							}
						});
				// CreateTable();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub

			}

		});

		// *************************************
		CreateDataGrid();
		// String url =
		// "https://vrt.3dcondox.com/data/building?scopeType=address&ad_mu=Toronto&sid="
		// + User.SID;
		// GET.send(url, new RequestCallback() {
		//
		// @Override
		// public void onResponseReceived(Request request, Response response) {
		// Log.write("answer:" + response.getText());
		// String json = response.getText();
		// JSONObject obj = JSONParser.parseStrict(json).isObject();
		// JSONArray arr = obj.get("buildings").isArray();
		// for (int index = 0; index < arr.size(); index++) {
		// Building new_building = new Building();
		// new_building.Parse(arr.get(index));
		// // dataProvider.getList().add(new_building);
		// }
		// CreateDataGrid();
		// }
		//
		// @Override
		// public void onError(Request request, Throwable exception) {
		// // TODO Auto-generated method stub
		//
		// }
		// });
	}

	// @Override
	// public boolean isValid(Building value, String filter) {
	// if (value.getName().toLowerCase().contains(filter.toLowerCase()))
	// return true;
	// // if (value.getAddress().toLowerCase().contains(filter.toLowerCase()))
	// // return true;
	// if
	// (String.valueOf(value.getId()).toLowerCase().contains(filter.toLowerCase()))
	// return true;
	// if (value.getStreet().toLowerCase().contains(filter.toLowerCase()))
	// return true;
	// if (value.getCity().toLowerCase().contains(filter.toLowerCase()))
	// return true;
	// if (value.getPostal().toLowerCase().contains(filter.toLowerCase()))
	// return true;
	// return false;
	// }

	// SUBMIT

	final DialogBox dialogBox = createDialogBox();

	private DialogBox createDialogBox() {
		// Create a dialog box and set the caption text
		final DialogBox dialogBox = new DialogBox();
		dialogBox.setText("caption");

		// Create a table to layout the content
		// VerticalPanel dialogContents = new VerticalPanel();
		// dialogContents.setSpacing(4);
		dialogBox.setSize("100px", "100px");
		// dialogBox.setWidget(new Submit(dialogBox));

		// Add some text to the top of the dialog
		// HTML details = new HTML("details");
		// dialogContents.add(details);

		// Return the dialog box
		// dialogBox.center();
		return dialogBox;
	}
	// @UiHandler("inlineHyperlink")
	// void onInlineHyperlinkClick(ClickEvent event) {
	// History.back();
	// }
	//
	// @Override
	// public void Update(String json) {
	// // TODO Auto-generated method stub
	//
	// }
}
