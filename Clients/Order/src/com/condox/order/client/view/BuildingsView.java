package com.condox.order.client.view;

import java.util.Comparator;
import java.util.List;

import com.condox.order.client.presenter.BuildingsPresenter;
import com.condox.order.client.view.utils.FilteredListDataProvider;
import com.condox.order.client.view.utils.IFilter;
import com.condox.order.shared.BuildingInfo;
import com.google.gwt.cell.client.ClickableTextCell;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.BlurEvent;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.FocusEvent;
import com.google.gwt.event.dom.client.KeyUpEvent;
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
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.SelectionChangeEvent;
import com.google.gwt.view.client.SingleSelectionModel;

public class BuildingsView extends Composite implements BuildingsPresenter.IDisplay, IFilter<BuildingInfo> {

	private static MyBuildingsViewUiBinder uiBinder = GWT
			.create(MyBuildingsViewUiBinder.class);
	private BuildingsPresenter presenter;
	private BuildingInfo selectedBuilding;
	private FilteredListDataProvider<BuildingInfo> dataProvider = new FilteredListDataProvider<BuildingInfo>(
			this);
	
	@UiField(provided=true) DataGrid<BuildingInfo> dataGrid = new DataGrid<BuildingInfo>(100);
	@UiField Hyperlink hyperlink;
	@UiField TextBox textFilter;

	interface MyBuildingsViewUiBinder extends UiBinder<Widget, BuildingsView> {
	}

	public BuildingsView() {
		initWidget(uiBinder.createAndBindUi(this));
		textFilter.setText(filterStr);
	}

	@Override
	public void setPresenter(BuildingsPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public void setData(List<BuildingInfo> data, BuildingInfo selected) {
		this.dataProvider.getList().clear();
		this.dataProvider.getList().addAll(data);
		this.selectedBuilding = selected;
		CreateDataGrid();
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
						selectedBuilding = selectionModel.getSelectedObject();
					}
				});
		if (selectedBuilding != null)
			selectionModel.setSelected(selectedBuilding, true);
		
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
		//================================
		String s = "Loading buildings list, please wait for few seconds.."; 
		Label loadingLabel = new Label(s);
		loadingLabel.setStylePrimaryName("my-loading-label");
		dataGrid.setLoadingIndicator(loadingLabel);
	}

	@Override
	public BuildingInfo getSelectedBuilding() {
		return selectedBuilding;
	}
	@UiHandler("hyperlink")
	void onHyperlinkClick(ClickEvent event) {
		presenter.onNext();
	}
	
	private String filterStr = "Type here Name, Street or Postal to filter...";
	@Override
	public boolean isValid(BuildingInfo value, String filter) {
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
		if (textFilter.getText().equals(filterStr))
			textFilter.setText("");
		else
			textFilter.selectAll();
	}

	@UiHandler("textFilter")
	void onTextFilterBlur(BlurEvent event) {
		if (textFilter.getText().isEmpty())
			textFilter.setText(filterStr);
	}
}
