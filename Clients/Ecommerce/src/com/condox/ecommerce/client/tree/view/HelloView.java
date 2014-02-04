package com.condox.ecommerce.client.tree.view;

import java.util.List;

import com.condox.clientshared.abstractview.Log;
import com.condox.ecommerce.client.FilteredListDataProvider;
import com.condox.ecommerce.client.IFilter;
import com.condox.ecommerce.client.tree.presenter.HelloPresenter;
import com.condox.ecommerce.client.tree.presenter.HelloPresenter.I_Display;
import com.google.gwt.cell.client.ClickableTextCell;
import com.google.gwt.cell.client.FieldUpdater;
import com.google.gwt.cell.client.TextCell;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.safehtml.shared.SafeHtml;
import com.google.gwt.safehtml.shared.SafeHtmlBuilder;
import com.google.gwt.text.shared.AbstractSafeHtmlRenderer;
import com.google.gwt.text.shared.SafeHtmlRenderer;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.cellview.client.Column;
import com.google.gwt.user.cellview.client.DataGrid;
import com.google.gwt.user.cellview.client.RowStyles;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.SelectionChangeEvent;
import com.google.gwt.view.client.SingleSelectionModel;
import com.google.gwt.user.client.ui.TextBox;

public class HelloView extends Composite implements I_Display, IFilter<ViewOrderInfo> {

	//
	public static String SELECTED_ID = null;
	//
	private static HelloAgentViewUiBinder uiBinder = GWT
			.create(HelloAgentViewUiBinder.class);
	MyDataGridResource resource = GWT.create(MyDataGridResource.class);
	@UiField (provided = true) DataGrid<ViewOrderInfo> dataGrid = new DataGrid<ViewOrderInfo>(50, resource);
	private FilteredListDataProvider<ViewOrderInfo> dataProvider = new FilteredListDataProvider<ViewOrderInfo>(this);
	@UiField Hyperlink hyperlink;
	@UiField Button buttonSettings;
	@UiField Button buttonShowHistory;
	@UiField Button button_1;
	@UiField Button button;
	@UiField Label textNickName;
	@UiField TextBox url;

	interface HelloAgentViewUiBinder extends UiBinder<Widget, HelloView> {
	}

	private HelloPresenter presenter = null;

	public HelloView() {
		initWidget(uiBinder.createAndBindUi(this));
		CreateDataGrid();
	}

	@Override
	public void setPresenter(HelloPresenter presenter) {
		this.presenter = presenter;
	}
	final SingleSelectionModel<ViewOrderInfo> selectionModel = new SingleSelectionModel<ViewOrderInfo>();
	private void CreateDataGrid() {
		// Add a selection model to handle user selection.
		dataGrid.setSelectionModel(selectionModel);
		selectionModel.addSelectionChangeHandler(new SelectionChangeEvent.Handler() {
			public void onSelectionChange(SelectionChangeEvent event) {
				SELECTED_ID = selectionModel.getSelectedObject().getId();
			}
		});
		
		SafeHtmlRenderer<String> anchorRenderer = new AbstractSafeHtmlRenderer<String>() {

			@Override
			public SafeHtml render(String object) {
				SafeHtmlBuilder sb = new SafeHtmlBuilder();
				sb.appendHtmlConstant(/*"<a>" + */object/* + "</a>"*/);
				return sb.toSafeHtml();
			}
		};

		// Add a button column to pick a suite.
		Column<ViewOrderInfo, String> AddressColumn = new Column<ViewOrderInfo, String>(
				new ClickableTextCell(anchorRenderer)) {
			@Override
			public String getValue(ViewOrderInfo object) {
				if (object.isEnabled())
					return "<a style=\"cursor:pointer;\">" + object.getLabel()+ "</a>";
				else
					return "<span style=\"color:grey;\">" + object.getLabel() + "</span>";
			}

		};

		AddressColumn.setFieldUpdater(new FieldUpdater<ViewOrderInfo, String>() {

					@Override
					public void update(int index, ViewOrderInfo object,
							String value) {
//						selectedBuilding = object;
						if (object.isEnabled())
							presenter.openAddress(object);
//						 presenter.onNext();
					}
				});

		dataGrid.addColumn(AddressColumn, "Address");
		
		// MLS# column
		Column<ViewOrderInfo, String> MLSColumn = new Column<ViewOrderInfo, String>(
				new TextCell(anchorRenderer)) {
			@Override
			public String getValue(ViewOrderInfo object) {
				if (object.isEnabled())
					return "<a>" + object.getMLS()+ "</a>";
				else
					return "<span style=\"color:grey;\">" + object.getMLS() + "</span>";
			}
		};
		dataGrid.addColumn(MLSColumn, "MLS#");
		
		// Disable column
				Column<ViewOrderInfo, String> DisableColumn = new Column<ViewOrderInfo, String>(
						new ClickableTextCell(anchorRenderer)) {
					@Override
					public String getValue(ViewOrderInfo object) {
						String disable = "<a style=\"cursor:pointer;\">disable</a>";
						String enable = "<a style=\"cursor:pointer;\">enable</a>";
						
						return object.isEnabled()?  disable : enable;
					}

				};
		
		DisableColumn.setFieldUpdater(new FieldUpdater<ViewOrderInfo, String>() {
			
			@Override
			public void update(int index, ViewOrderInfo object,
					String value) {
//				SELECTED_ID = object.getId();
				presenter.setEnabled(object, !object.isEnabled());
			}
		});
		
		dataGrid.addColumn(DisableColumn, "");
		
		// Delete column
		Column<ViewOrderInfo, String> DeleteColumn = new Column<ViewOrderInfo, String>(
				new ClickableTextCell(anchorRenderer)) {
			@Override
			public String getValue(ViewOrderInfo object) {
				return "<a style=\"cursor:pointer;\">delete</a>";
			}
			
		};
		
		DeleteColumn.setFieldUpdater(new FieldUpdater<ViewOrderInfo, String>() {
			
			@Override
			public void update(int index, ViewOrderInfo object,
					String value) {
//				SELECTED_ID = selectionModel.getSelectedObject().getId();
				presenter.delete(object);
			}
		});
		
		dataGrid.addColumn(DeleteColumn, "");
		
		// Get column
		Column<ViewOrderInfo, String> GetUrlColumn = new Column<ViewOrderInfo, String>(
				new ClickableTextCell(anchorRenderer)) {
			@Override
			public String getValue(ViewOrderInfo object) {
				return "<a style=\"cursor:pointer;\">get url</a>";
			}
			
		};
		
		GetUrlColumn.setFieldUpdater(new FieldUpdater<ViewOrderInfo, String>() {
			
			@Override
			public void update(int index, ViewOrderInfo object,
					String value) {
//				SELECTED_ID = selectionModel.getSelectedObject().getId();
				presenter.getUrl(object);
			}
		});
		
		dataGrid.addColumn(GetUrlColumn, "Url");
		
				
//
//		// Add a column to fit free space.
//		TextColumn<BuildingInfo> freeSpaceColumn = new TextColumn<BuildingInfo>() {
//			@Override
//			public String getValue(BuildingInfo object) {
//				return /* object.getPostal() */null;
//			}
//		};
//
//	
		
//		final SingleSelectionModel<BuildingInfo> selectionModel = new SingleSelectionModel<BuildingInfo>();
//		dataGrid.setSelectionModel(selectionModel);
//		selectionModel
//				.addSelectionChangeHandler(new SelectionChangeEvent.Handler() {
//					public void onSelectionChange(SelectionChangeEvent event) {
//						selectedBuilding = selectionModel.getSelectedObject();
//						presenter.setSelectedBuilding(selectedBuilding);
////						buttonNext.setEnabled(selectedBuilding != null);
//					}
//				});
//		if (selectedBuilding != null) {
//			selectionModel.setSelected(selectedBuilding, true);
//		}
//
//		postalColumn.setSortable(true);
//		sortHandler.setComparator(postalColumn, new Comparator<BuildingInfo>() {
//			@Override
//			public int compare(BuildingInfo A, BuildingInfo B) {
//				return A.getPostal().compareTo(B.getPostal());
//			}
//		});
//
//		dataGrid.addColumn(freeSpaceColumn, "");
//
//		if (!dataProvider.getDataDisplays().contains(dataGrid))
//			dataProvider.addDataDisplay(dataGrid);
//
		dataGrid.setColumnWidth(AddressColumn, "400px");
		dataGrid.setColumnWidth(MLSColumn, "80px");
		dataGrid.setColumnWidth(DisableColumn, "60px");
		dataGrid.setColumnWidth(DeleteColumn, "55px");
		dataGrid.setColumnWidth(GetUrlColumn, "70px");
//		// ================================
//		String s = "Loading buildings list, please wait for few seconds..";
//		Label loadingLabel = new Label(s);
//		loadingLabel.setStylePrimaryName("my-loading-label");
//		// dataGrid.setLoadingIndicator(loadingLabel);
//		dataGrid.setEmptyTableWidget(loadingLabel);
		
		if (!dataProvider.getDataDisplays().contains(dataGrid))
			dataProvider.addDataDisplay(dataGrid);

		//		dataGrid.get
	}
	
	private PopupPanel loading = new PopupPanel();

	@Override
	public void setData(List<ViewOrderInfo> data) {
		if (data == null) {
			loading.clear();
			loading.setModal(true);
			loading.setGlassEnabled(true);
			loading.add(new Label("Loading, please wait..."));
			loading.center();
		} else {
			loading.hide();
			this.dataProvider.getList().clear();
			this.dataProvider.getList().addAll(data);
//			selectionModel.setSelected(data.get(3), true);
			for (ViewOrderInfo item : data)
				if (item.getId().equals(SELECTED_ID))
					selectionModel.setSelected(item, true);
//				else
//					selectionModel.setSelected(item, false);
		}
//			this.selectedBuilding = selected;
//			dataGrid.getSelectionModel().setSelected(selected, true);
	}
	
	@Override
	public boolean isValid(ViewOrderInfo value, String filter) {
		// TODO Auto-generated method stub
		return false;
	}

	
	@UiHandler("hyperlink")
	void onHyperlinkClick(ClickEvent event) {
		if (presenter != null)
			presenter.onLogout();
	}

	@Override
	public void setNickName(String value) {
		textNickName.setText("Hello, " + value + "!");	// TODO
	}
	
	@UiHandler("buttonShowHistory")
	void onButtonShowHistoryClick(ClickEvent event) {
		if (presenter != null)
			presenter.onShowHistory();
	}
	@UiHandler("button_1")
	void onButton_1Click(ClickEvent event) {
		if (presenter != null)
			presenter.onNewOrder();
	}
	@UiHandler("buttonSettings")
	void onButtonSettingsClick(ClickEvent event) {
		if (presenter != null)
			presenter.onShowSettings();
	}
	@UiHandler("button")
	void onButtonClick(ClickEvent event) {
		if (presenter != null)
			presenter.onUpdateProfile();
	}

	@Override
	public void setViewOrderUrl(String url) {
		this.url.setText(url);
		this.url.selectAll();
	}
}