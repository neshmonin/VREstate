package com.condox.order.client.view;

import java.util.Comparator;
import java.util.Iterator;

import com.condox.order.client.context.Log;
import com.condox.order.client.context.SuitesContext;
import com.condox.order.client.context.Tree;
import com.condox.order.client.utils.GET;
import com.condox.order.client.utils.Globals;
import com.condox.order.client.view.factory.IView;
import com.condox.order.client.view.utils.BuildingInfo;
import com.condox.order.client.view.utils.FilteredListDataProvider;
import com.condox.order.client.view.utils.IFilter;
import com.google.gwt.cell.client.ClickableTextCell;
import com.google.gwt.cell.client.FieldUpdater;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.BlurEvent;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ClickEvent;
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
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.SelectionChangeEvent;
import com.google.gwt.view.client.SingleSelectionModel;

public class BuildingsView extends Composite implements IView,
		IFilter<BuildingInfo> {

	private static BuildingsViewUiBinder uiBinder = GWT
			.create(BuildingsViewUiBinder.class);
	@UiField
	Button btnNext;
	@UiField(provided = true)
	// TODO !!!
	DataGrid<BuildingInfo> dataGrid = new DataGrid<BuildingInfo>(100); 
	@UiField TextBox textFilter;

	interface BuildingsViewUiBinder extends UiBinder<Widget, BuildingsView> {
	}

	private Tree tree;
	private BuildingInfo selectedBuilding;

	public BuildingsView(Tree tree) {
		this();

		this.tree = tree;
		/*listBuildings.clear();
		for (int i = 50; i > 0; i--)
			listBuildings.insertItem("Building #" + i, 0);*/

		/*
		 * try { Integer index =
		 * Integer.valueOf(tree.getValue("selectedBuilding")); if (index !=
		 * null) listBuildings.setSelectedIndex(index - 1); } catch
		 * (NumberFormatException e) { e.printStackTrace(); }
		 */

		updateBuildingsList();
	}

	public BuildingsView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@UiHandler("btnNext")
	void onBtnNextClick(ClickEvent event) {
		if (selectedBuilding != null) {
			tree.setValue("selectedBuilding", selectedBuilding.getName());
			tree.next(new SuitesContext());
		}
	}

	// =========================================
	private FilteredListDataProvider<BuildingInfo> dataProvider = new FilteredListDataProvider<BuildingInfo>(
			this);

	private void updateBuildingsList() {
		GET.send(Globals.getBuildingsListRequest(), onBuildingsList);
	}

	private RequestCallback onBuildingsList = new RequestCallback() {

		@Override
		public void onResponseReceived(Request request, Response response) {
			Log.write("answer:" + response.getText());
			String json = response.getText();
			JSONObject obj = JSONParser.parseStrict(json).isObject();
			JSONArray arr = obj.get("buildings").isArray();
			for (int index = 0; index < arr.size(); index++) {
				BuildingInfo info = new BuildingInfo();
				info.Parse(arr.get(index));
				dataProvider.getList().add(info);
			}
			CreateDataGrid();
		}

		@Override
		public void onError(Request request, Throwable exception) {
			// TODO Auto-generated method stub

		}
	};

	@Override
	public boolean isValid(BuildingInfo value, String filter) {
		if (value.getName().toLowerCase().contains(filter.toLowerCase()))
			return true;
		if (value.getStreet().toLowerCase().contains(filter.toLowerCase()))
			return true;
		if (value.getPostal().toLowerCase().contains(filter.toLowerCase()))
			return true;
		return false;
	}

	private void CreateDataGrid() {
		int count = dataGrid.getColumnCount();
		for (int i = 0; i < count; i++)
			dataGrid.removeColumn(0);

		ListHandler<BuildingInfo> sortHandler = new ListHandler<BuildingInfo>(
				dataProvider.getList());
		dataGrid.addColumnSortHandler(sortHandler);

		// Add a text column to show the name.
		TextColumn<BuildingInfo> nameColumn = new TextColumn<BuildingInfo>() {
			@Override
			public String getValue(BuildingInfo object) {
				return object.getName();
			}
		};
		nameColumn.setSortable(true);
		sortHandler.setComparator(nameColumn, new Comparator<BuildingInfo>() {
			@Override
			public int compare(BuildingInfo A, BuildingInfo B) {
				return A.getName().compareTo(B.getName());
			}
		});
		dataGrid.addColumn(nameColumn, "Building Name");

		// Add a text column to show the street.
		TextColumn<BuildingInfo> streetColumn = new TextColumn<BuildingInfo>() {
			@Override
			public String getValue(BuildingInfo object) {
				return object.getStreet();
			}
		};
		streetColumn.setSortable(true);
		sortHandler.setComparator(streetColumn, new Comparator<BuildingInfo>() {
			@Override
			public int compare(BuildingInfo A, BuildingInfo B) {
				return A.getStreet().compareTo(B.getStreet());
			}
		});
		dataGrid.addColumn(streetColumn, "Street");

		// Add a text column to show the postal.
		TextColumn<BuildingInfo> postalColumn = new TextColumn<BuildingInfo>() {
			@Override
			public String getValue(BuildingInfo object) {
				return object.getPostal();
			}
		};
		postalColumn.setSortable(true);
		sortHandler.setComparator(postalColumn, new Comparator<BuildingInfo>() {
			@Override
			public int compare(BuildingInfo A, BuildingInfo B) {
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
		Column<BuildingInfo, String> viewSuitesColumn = new Column<BuildingInfo, String>(
				new ClickableTextCell(anchorRenderer)) {
			@Override
			public String getValue(BuildingInfo object) {
				return "Pick a Suite...";
			}

		};

		viewSuitesColumn
				.setFieldUpdater(new FieldUpdater<BuildingInfo, String>() {

					@Override
					public void update(int index, BuildingInfo object,
							String value) {
						tree.setValue("selectedBuilding", object.getName());
						// tree.setValue("selectedBuilding",
						// String.valueOf(object.getId()));
						// Orders.selectedBuildingInfo = object;
						// ***************************************
						// UrlBuilder builder =
						// Window.Location.createUrlBuilder();
						// builder.setParameter("BuildingInfoId",
						// String.valueOf(object.getId()));
						// Window.Location.replace(builder.buildString().replaceAll("BuildingInfos",
						// "suits"));
						// ***************************************
						tree.next(new SuitesContext());
						// History.newItem("suits");
					}
				});
		// dataGrid.addColumn(viewSuitesColumn, "");

		// Add a column to fit free space.
		TextColumn<BuildingInfo> freeSpaceColumn = new TextColumn<BuildingInfo>() {
			@Override
			public String getValue(BuildingInfo object) {
				return /* object.getPostal() */null;
			}
		};

		// Add a selection model to handle user selection.
		final SingleSelectionModel<BuildingInfo> selectionModel = new SingleSelectionModel<BuildingInfo>();
		dataGrid.setSelectionModel(selectionModel);
		selectionModel
				.addSelectionChangeHandler(new SelectionChangeEvent.Handler() {
					public void onSelectionChange(SelectionChangeEvent event) {
						BuildingInfo selected = selectionModel
								.getSelectedObject();
						if (selected != null) {
							selectedBuilding = selected;
							// Window.alert("You selected: " + selected.name);
						}
					}
				});

		postalColumn.setSortable(true);
		sortHandler.setComparator(postalColumn, new Comparator<BuildingInfo>() {
			@Override
			public int compare(BuildingInfo A, BuildingInfo B) {
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

		/*
		 * SelectionModel<? super BuildingInfo> model =
		 * dataGrid.getSelectionModel(); Log.write("model: " + model);
		 * BuildingInfo info = dataProvider.getList().get(0); Log.write("info: "
		 * + info);
		 */

		// ((SelectionModel<BuildingInfo>)dataGrid.getSelectionModel()).setSelected(dataProvider.getList().get(0),
		// true);
		Iterator<BuildingInfo> iter = dataProvider.getList().iterator();
		while (iter.hasNext()) {
			BuildingInfo info = iter.next();
			// String s1 = info.getName();
			// String s2 = tree.getValue("selectedBuilding");
			// Log.write(s1 + " vs " + s2 + " = " + s1.equals(s2));
			if (info.getName().equals(tree.getValue("selectedBuilding"))) {
				dataGrid.getSelectionModel().setSelected(info, true);
			}
		}
	}

	@UiHandler("textFilter")
	void onTextFilterKeyUp(KeyUpEvent event) {
		dataProvider.setFilter(textFilter.getText());
	}

	@UiHandler("textFilter")
	void onTextFilterFocus(FocusEvent event) {
		/*if (textFilter.getText().equals(Config.TXT_START_TYPING))
			textFilter.setText("");
		else*/
		if (!textFilter.getText().isEmpty())
			textFilter.selectAll();
	}

	@UiHandler("textFilter")
	void onTextFilterBlur(BlurEvent event) {
		/*if (textFilter.getText().isEmpty())
			textFilter.setText(Config.TXT_START_TYPING);*/
	}
}
