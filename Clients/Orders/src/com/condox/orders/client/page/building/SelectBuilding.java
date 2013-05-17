package com.condox.orders.client.page.building;

import java.util.Comparator;

import com.condox.orders.client.Config;
import com.condox.orders.client.FilteredListDataProvider;
import com.condox.orders.client.GET;
import com.condox.orders.client.IFilter;
import com.condox.orders.client.IPage;
import com.condox.orders.client.Log;
import com.condox.orders.client.Options;
import com.condox.orders.client.Orders;
import com.condox.orders.client.User;
import com.google.gwt.cell.client.ClickableTextCell;
import com.google.gwt.cell.client.FieldUpdater;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.BlurEvent;
import com.google.gwt.event.dom.client.FocusEvent;
import com.google.gwt.event.dom.client.KeyUpEvent;
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
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.cellview.client.Column;
import com.google.gwt.user.cellview.client.ColumnSortEvent.ListHandler;
import com.google.gwt.user.cellview.client.DataGrid;
import com.google.gwt.user.cellview.client.TextColumn;
import com.google.gwt.user.client.History;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;

public class SelectBuilding extends Composite implements IFilter<Building>,
		IPage {

	private static SelectBuildingUiBinder uiBinder = GWT
			.create(SelectBuildingUiBinder.class);
	@UiField(provided = true)
	DataGrid<Building> dataGrid = new DataGrid<Building>(100);
	@UiField
	TextBox textFilter;
	@UiField
	ListBox boxCity;

	interface SelectBuildingUiBinder extends UiBinder<Widget, SelectBuilding> {
	}

	// **************************************************************************
	// private class CustomHeaderBuilder extends Header<Building> {
	//
	// public CustomHeaderBuilder(Cell<Building> cell) {
	// super(cell);
	// }
	//
	// @Override
	// public Building getValue() {
	// // TODO Auto-generated method stub
	// return null;
	// }
	//
	// }
	// **************************************************************************

	public SelectBuilding() {
		initWidget(uiBinder.createAndBindUi(this));
		setup();
	}

	public SelectBuilding(String firstName) {
		initWidget(uiBinder.createAndBindUi(this));
		setup();
	}

	private void setup() {
		boxCity.addItem("Toronto");
		boxCity.setEnabled(true);
		textFilter.setText(Config.TXT_START_TYPING);

		// resources = GWT.create(Resources.class);
		// resources.styles().ensureInjected();
	}

	@Override
	public void Update() {
		dataProvider.getList().clear();
		GetBuildingsList();
	}

	// private Column<Building, String> viewSuitesColumn;
	// private Column<Building, String> viewSuitesColumn2;
	private FilteredListDataProvider<Building> dataProvider = new FilteredListDataProvider<Building>(
			this);

	private void CreateDataGrid() {
		int count = dataGrid.getColumnCount();
		for (int i = 0; i < count; i++)
			dataGrid.removeColumn(0);

		ListHandler<Building> sortHandler = new ListHandler<Building>(
				dataProvider.getList());
		dataGrid.addColumnSortHandler(sortHandler);

		// Add a text column to show the name.
		TextColumn<Building> nameColumn = new TextColumn<Building>() {
			@Override
			public String getValue(Building object) {
				return object.getName();
			}
		};
		nameColumn.setSortable(true);
		sortHandler.setComparator(nameColumn, new Comparator<Building>() {
			@Override
			public int compare(Building A, Building B) {
				return A.getName().compareTo(B.getName());
			}
		});
		dataGrid.addColumn(nameColumn, "Building Name");
		// Add a text column to show the street.
		TextColumn<Building> streetColumn = new TextColumn<Building>() {
			@Override
			public String getValue(Building object) {
				return object.getStreet();
			}
		};
		streetColumn.setSortable(true);
		sortHandler.setComparator(streetColumn, new Comparator<Building>() {
			@Override
			public int compare(Building A, Building B) {
				return A.getStreet().compareTo(B.getStreet());
			}
		});
		dataGrid.addColumn(streetColumn, "Street");

		// Add a text column to show the postal.
		TextColumn<Building> postalColumn = new TextColumn<Building>() {
			@Override
			public String getValue(Building object) {
				return object.getPostal();
			}
		};
		postalColumn.setSortable(true);
		sortHandler.setComparator(postalColumn, new Comparator<Building>() {
			@Override
			public int compare(Building A, Building B) {
				return A.getPostal().compareTo(B.getPostal());
			}
		});
		dataGrid.addColumn(postalColumn, "Postal");

		// View friends.
		SafeHtmlRenderer<String> anchorRenderer = new AbstractSafeHtmlRenderer<String>() {

			@Override
			public SafeHtml render(String object) {
				// TODO Auto-generated method stub
				SafeHtmlBuilder sb = new SafeHtmlBuilder();
				sb.appendHtmlConstant("<img src=\"Select_but.png\"/>");
				return sb.toSafeHtml();
			}
		};

		// Add a button column to pick a suite.
		Column<Building, String> viewSuitesColumn = new Column<Building, String>(
				new ClickableTextCell(anchorRenderer)) {
			@Override
			public String getValue(Building object) {
				return "Pick a Suite...";
			}

		};
		viewSuitesColumn.setFieldUpdater(new FieldUpdater<Building, String>() {

			@Override
			public void update(int index, Building object, String value) {
				Orders.selectedBuilding = object;
				// ***************************************
				// UrlBuilder builder = Window.Location.createUrlBuilder();
				// builder.setParameter("BuildingId",
				// String.valueOf(object.getId()));
				// Window.Location.replace(builder.buildString().replaceAll("buildings",
				// "suits"));
				// ***************************************
				History.newItem("suits");
			}
		});
		dataGrid.addColumn(viewSuitesColumn, "");

		// Add a column to fit free space.
		TextColumn<Building> freeSpaceColumn = new TextColumn<Building>() {
			@Override
			public String getValue(Building object) {
				return /* object.getPostal() */null;
			}
		};
		postalColumn.setSortable(true);
		sortHandler.setComparator(postalColumn, new Comparator<Building>() {
			@Override
			public int compare(Building A, Building B) {
				return A.getPostal().compareTo(B.getPostal());
			}
		});
		dataGrid.addColumn(freeSpaceColumn, "");

		if (!dataProvider.getDataDisplays().contains(dataGrid))
			dataProvider.addDataDisplay(dataGrid);

		dataGrid.setColumnWidth(nameColumn, "200px");
		dataGrid.setColumnWidth(streetColumn, "200px");
		dataGrid.setColumnWidth(postalColumn, "100px");
		dataGrid.setColumnWidth(viewSuitesColumn, "150px");
	}

	private void GetBuildingsList() {
		String url = Options.URL_VRT
				+ "data/building?scopeType=address&ad_mu=Toronto"
				+ "&ed=Resale" + "&sid=" + User.SID;
		GET.send(url, new RequestCallback() {

			@Override
			public void onResponseReceived(Request request, Response response) {
				Log.write("answer:" + response.getText());
				String json = response.getText();
				JSONObject obj = JSONParser.parseStrict(json).isObject();
				JSONArray arr = obj.get("buildings").isArray();
				for (int index = 0; index < arr.size(); index++) {
					Building new_building = new Building();
					new_building.Parse(arr.get(index));
					dataProvider.getList().add(new_building);
				}
				CreateDataGrid();
			}

			@Override
			public void onError(Request request, Throwable exception) {
				// TODO Auto-generated method stub

			}

		});
	}

	@Override
	public boolean isValid(Building value, String filter) {
		if (value.getName().toLowerCase().contains(filter.toLowerCase()))
			return true;
		// if (value.getAddress().toLowerCase().contains(filter.toLowerCase()))
		// return true;
		// if (String.valueOf(value.getId()).toLowerCase()
		// .contains(filter.toLowerCase()))
		// return true;
		if (value.getStreet().toLowerCase().contains(filter.toLowerCase()))
			return true;
		// if (value.getCity().toLowerCase().contains(filter.toLowerCase()))
		// return true;
		 if (value.getPostal().toLowerCase().contains(filter.toLowerCase()))
		 return true;
		return false;
	}

	@UiHandler("textFilter")
	void onTextFilterKeyUp(KeyUpEvent event) {
		dataProvider.setFilter(textFilter.getText());
	}

	@UiHandler("textFilter")
	void onTextFilterFocus(FocusEvent event) {
		if (textFilter.getText().equals(Config.TXT_START_TYPING))
			textFilter.setText("");
		else
			textFilter.selectAll();
	}

	@UiHandler("textFilter")
	void onTextFilterBlur(BlurEvent event) {
		if (textFilter.getText().isEmpty())
			textFilter.setText(Config.TXT_START_TYPING);
	}
}
