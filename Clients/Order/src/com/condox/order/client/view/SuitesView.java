package com.condox.order.client.view;

import com.condox.order.client.context.OrderTypeContext;
import com.condox.order.client.context.Tree;
import com.condox.order.client.view.factory.IView;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.ListBox;
import com.google.gwt.user.client.ui.Widget;

public class SuitesView extends Composite implements IView {

	private static SuitesViewUiBinder uiBinder = GWT
			.create(SuitesViewUiBinder.class);
	@UiField ListBox listSuites;
	@UiField Button btnPrev;
	@UiField Button btnNext;

	interface SuitesViewUiBinder extends UiBinder<Widget, SuitesView> {
	}

	private Tree tree;
	public SuitesView(Tree tree) {
		this();
		
		this.tree = tree;
		listSuites.clear();
		for (int i = 100; i > 0; i--)
			listSuites.insertItem("Suite #" + i, 0);
	
		try {
			Integer index = Integer.valueOf(tree.getValue("selectedSuite"));
			if (index != null)
				listSuites.setSelectedIndex(index-1);
		} catch (NumberFormatException e) {
			e.printStackTrace();
		}
	}
	
	public SuitesView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@UiHandler("btnPrev")
	void onBtnPrevClick(ClickEvent event) {
		tree.prev();
	}
	
	@UiHandler("listSuites")
	void onListSuitesChange(ChangeEvent event) {
		tree.setValue("selectedSuite", String.valueOf(listSuites.getSelectedIndex()+1));
		btnNext.setEnabled(tree.isValid());
	}
	@UiHandler("btnNext")
	void onBtnNextClick(ClickEvent event) {
		tree.next(new OrderTypeContext());
	}
}
