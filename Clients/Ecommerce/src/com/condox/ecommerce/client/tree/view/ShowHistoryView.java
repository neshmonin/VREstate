package com.condox.ecommerce.client.tree.view;

import java.util.List;

import com.condox.ecommerce.client.FilteredListDataProvider;
import com.condox.ecommerce.client.IFilter;
import com.condox.ecommerce.client.tree.node.ShowHistoryNode.HistoryTransaction;
import com.condox.ecommerce.client.tree.presenter.ShowHistoryPresenter;
import com.condox.ecommerce.client.tree.presenter.ShowHistoryPresenter.I_Display;
import com.google.gwt.cell.client.ClickableTextCell;
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
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.Label;
import com.google.gwt.user.client.ui.PopupPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.CellPreviewEvent;

public class ShowHistoryView extends Composite implements I_Display,
		IFilter<HistoryTransaction> {

	private static ShowHistoryViewUiBinder uiBinder = GWT
			.create(ShowHistoryViewUiBinder.class);
	@UiField
	Hyperlink linkClose;
	@UiField(provided = true)
	DataGrid<HistoryTransaction> dataGrid = new DataGrid<HistoryTransaction>();
	PopupPanel tooltipPanel = new PopupPanel();
	private FilteredListDataProvider<HistoryTransaction> dataProvider = new FilteredListDataProvider<HistoryTransaction>(
			this);

	interface ShowHistoryViewUiBinder extends UiBinder<Widget, ShowHistoryView> {
	}

	private ShowHistoryPresenter presenter = null;

	public ShowHistoryView() {
		initWidget(uiBinder.createAndBindUi(this));
		CreateDataGrid();
	}

	@Override
	public void setPresenter(ShowHistoryPresenter presenter) {
		this.presenter = presenter;
	}

	private void CreateDataGrid() {
		// Add a selection model to handle user selection.
		// dataGrid.setSelectionModel(selectionModel);
		// selectionModel.addSelectionChangeHandler(new
		// SelectionChangeEvent.Handler() {
		// public void onSelectionChange(SelectionChangeEvent event) {
		// SELECTED_ID = selectionModel.getSelectedObject().getId();
		// }
		// });

		SafeHtmlRenderer<String> anchorRenderer = new AbstractSafeHtmlRenderer<String>() {

			@Override
			public SafeHtml render(String object) {
				SafeHtmlBuilder sb = new SafeHtmlBuilder();
				sb.appendHtmlConstant(object);
				return sb.toSafeHtml();
			}
		};

		// Date column
		Column<HistoryTransaction, String> DateColumn = new Column<HistoryTransaction, String>(
				new ClickableTextCell(anchorRenderer)) {

			@Override
			public String getValue(HistoryTransaction object) {
				// return "<span title=\"" + object.getToo
				return "<span>" + object.getDate() + "</span>";
			}
		};
		dataGrid.setColumnWidth(DateColumn, "220px");
		dataGrid.addColumn(DateColumn, "Date");

		 // Subject column
		 Column<HistoryTransaction, String> SubjectColumn = new
		 Column<HistoryTransaction, String>(
		 new ClickableTextCell(anchorRenderer)) {
		
		 @Override
		 public String getValue(HistoryTransaction object) {
		 return "<span title=\"" + object.getTooltip() + "\">"
		 + object.getSubject() + "</span>";
		 }
		 };
		 dataGrid.setColumnWidth(SubjectColumn, "150px");
		 dataGrid.addColumn(SubjectColumn, "Subject");
		
		 // Operation column
		 Column<HistoryTransaction, String> OperationColumn = new
		 Column<HistoryTransaction, String>(
		 new ClickableTextCell(anchorRenderer)) {
		
		 @Override
		 public String getValue(HistoryTransaction object) {
		 return "<span title=\"" + object.getTooltip() + "\">"
		 + object.getOperation() + "</span>";
		 }
		 };
		 dataGrid.setColumnWidth(OperationColumn, "80px");
		 dataGrid.addColumn(OperationColumn, "Operation");
		
		 // Amount column
		 Column<HistoryTransaction, String> AmountColumn = new
		 Column<HistoryTransaction, String>(
		 new ClickableTextCell(anchorRenderer)) {
		
		 @Override
		 public String getValue(HistoryTransaction object) {
		 return "<span title=\"" + object.getTooltip() + "\">"
		 + object.getAmount() + "</span>";
		 }
		 };
		 dataGrid.setColumnWidth(AmountColumn, "80px");
		 dataGrid.addColumn(AmountColumn, "Amount,$");

		if (!dataProvider.getDataDisplays().contains(dataGrid))
			dataProvider.addDataDisplay(dataGrid);

		dataGrid.addCellPreviewHandler(new CellPreviewEvent.Handler<HistoryTransaction>() {

			@Override
			public void onCellPreview(
					final CellPreviewEvent<HistoryTransaction> event) {
				String type = event.getNativeEvent().getType();
				if ("mouseover".equals(type)) {
					Timer update = new Timer() {

						@Override
						public void run() {
							String tooltip = event.getValue().getTooltip();
							tooltipPanel.setWidget(new Label(tooltip));
						}
					};
					update.scheduleRepeating(1000);
					int x = event.getNativeEvent().getClientX();
					int y = event.getNativeEvent().getClientY();
					tooltipPanel.setPopupPosition(x, y);
					tooltipPanel.show();
				} else if ("mouseout".equals(type))
					tooltipPanel.hide();
			}
		});
	}

	@UiHandler("linkClose")
	void onLinkCloseClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}

	@Override
	public void setHistoryData(List<HistoryTransaction> data) {
		PopupPanel loading = new PopupPanel();
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
		}
	}

	@Override
	public boolean isValid(HistoryTransaction value, String filter) {
		// TODO Auto-generated method stub
		return false;
	}
}
