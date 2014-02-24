package com.condox.ecommerce.client.tree.view;

import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HasWidgets;
import com.google.gwt.user.client.ui.Widget;

public class ProgressPresenter implements I_Presenter {

	public static interface I_Display {
		void setPresenter(ProgressPresenter presenter);

		void setProgress(int percent);
		
		Widget asWidget();
	}

	private I_Display display = null;
	private EcommerceTree tree = null;
	private Timer timer;
	
	public ProgressPresenter(I_Display display) {
		this.display = display;
	}

	@Override
	public void go(HasWidgets container) {
		container.clear();
		container.add(display.asWidget());
		timer = new Timer() {
			int percent  = 10;
			@Override
			public void run() {
				percent += 10;
				if ((percent < 0)||(percent > 100))
					percent = 10;
				display.setProgress(percent);
			}
			
		};
		timer.scheduleRepeating(1000);
	}
	
	@Override
	public void setView(Composite view) {
//		display = (I_Display) view;
		display.setPresenter(this);
	}

	@Override
	public void setTree(EcommerceTree tree) {
		this.tree = tree;
	}

}
