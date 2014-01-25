package com.condox.ecommerce.client.tree.view;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.document.BuildingInfo;
import com.condox.clientshared.document.HistoryTransactionInfo;
import com.condox.clientshared.document.SuiteInfo;
import com.condox.ecommerce.client.tree.presenter.HistoryPresenter;
import com.condox.ecommerce.client.tree.presenter.HistoryPresenter.I_Display;
import com.google.gwt.cell.client.AbstractCell;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.safehtml.shared.SafeHtmlBuilder;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.cellview.client.CellList;
import com.google.gwt.user.cellview.client.HasKeyboardSelectionPolicy.KeyboardSelectionPolicy;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.view.client.SelectionChangeEvent;
import com.google.gwt.view.client.SingleSelectionModel;

public class HistoryView extends Composite implements I_Display {

	private static HistoryViewUiBinder uiBinder = GWT
			.create(HistoryViewUiBinder.class);

	@UiField(provided = true)
	CellList<HistoryTransactionInfo> cellList = new CellList<HistoryTransactionInfo>(
			new AbstractCell<HistoryTransactionInfo>() {
				@Override
				public void render(Context context,
						HistoryTransactionInfo value, SafeHtmlBuilder sb) {
					if (value == null) {
						return;
					}
					// sb.appendEscaped(value.getDate());
					sb.appendHtmlConstant("<table>");

					// // Add the contact image.
					// sb.appendHtmlConstant("<tr><td rowspan='3'>");
					// sb.appendHtmlConstant(imageHtml);
					// sb.appendHtmlConstant("</td>");
					//
					// Add the name and address.
					sb.appendHtmlConstant("<td style='width:150px;font-size:95%;'>");
					sb.appendEscaped(value.getDate());
					sb.appendHtmlConstant("</td>");
					sb.appendHtmlConstant("<td style='width:75px;font-size:95%;'>");
					sb.appendEscaped(value.getSubject());
					sb.appendHtmlConstant("</td>");
					sb.appendHtmlConstant("<td style='width:75px;font-size:95%;'>");
					sb.appendEscaped(value.getOperation());
					sb.appendHtmlConstant("</td></tr></table>");
				}
			});
	@UiField
	HTML form;

	interface HistoryViewUiBinder extends UiBinder<Widget, HistoryView> {
	}

	@SuppressWarnings("unused")
	private HistoryPresenter presenter = null;
	private int currSuiteId = 0;

	public HistoryView() {
		cellList.setKeyboardSelectionPolicy(KeyboardSelectionPolicy.ENABLED);
		// ------
		// Add a selection model to handle user selection.
		final SingleSelectionModel<HistoryTransactionInfo> selectionModel = new SingleSelectionModel<HistoryTransactionInfo>();
		cellList.setSelectionModel(selectionModel);
		selectionModel
				.addSelectionChangeHandler(new SelectionChangeEvent.Handler() {
					public void onSelectionChange(SelectionChangeEvent event) {
						HistoryTransactionInfo selected = selectionModel
								.getSelectedObject();
						if (selected != null) {
//							html = "";
							currSuiteId = selected.getTargetId();
							if (presenter != null)
								presenter.getSuiteInfo(currSuiteId);
						}
					}
				});
		// --------------

		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(HistoryPresenter presenter) {
		this.presenter = presenter;
	}

	private List<HistoryTransactionInfo> transactions = new ArrayList<HistoryTransactionInfo>();

	@Override
	public void setHistoryTransactions(List<HistoryTransactionInfo> transactions) {
		this.transactions.clear();
		this.transactions.addAll(transactions);
		cellList.setRowData(0, transactions);

	}

	@Override
	public void setAddress(int index, String value) {
	}

	private String html = "";

	@Override
	public void setSuiteInfo(SuiteInfo info) {
		html = "";
//		SuiteInfo.Status status = info.getStatus();
		html += "Object: Suite<br>";
		html += "Status: " + info.getStatus() + "<br>";
		html += "Price: " + info.getCurrentPriceDisplay() + "<br>";
		if (presenter != null)
			presenter.getBuildingInfo(info.getBuildingId());
//		form.setHTML(html);
	}
	
	@Override
	public void setBuildingInfo(BuildingInfo info) {
		html += "Address: " + info.getName() + ", " + info.getStreet() + ", " + info.getCity() + "<br>";
		html += "Country: " + info.getCountry() + "<br>";
//		html += "Price: " + info.getCurrentPriceDisplay() + "<br>";
		form.setHTML(html);
	}

	@UiHandler("hyperlink")
	void onHyperlinkClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}
}
