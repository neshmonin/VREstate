package com.condox.ecommerce.client.tree.presenter;

import com.condox.clientshared.container.I_Contained;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.EcommerceTree.Field;
import com.condox.ecommerce.client.tree.EcommerceTree.NodeStates;
import com.condox.ecommerce.client.tree.node.AgreementNode;
import com.google.gwt.user.client.ui.Widget;

public class AgreementPresenter implements I_Presenter {

	public static interface I_Display extends I_Contained {
		void setPresenter(AgreementPresenter presenter);

		Widget asWidget();
	}

	private I_Display display = null;
	private AgreementNode node = null;

	public AgreementPresenter(I_Display newDisplay, AgreementNode newNode) {
		display = newDisplay;
		display.setPresenter(this);
		node = newNode;
	}

	@Override
	public void go(I_Container container) {
		container.clear();
		container.add(display);
	}
	
	// Navigation events
	public void onCancel() {
		node.next(NodeStates.Cancel);
	}

	public void onPrev() {
		node.next(NodeStates.Prev);
	}
	
	public void onProceed() {
//		saveData();
		node.next(NodeStates.Proceed);
	}
	
//	// Data utils
//	private String getString(Field key) {
//		return (node.getData(key) == null)? "" : node.getData(key).asString();
//	}
//	
//	private void loadData() {
//		String listing = getString(Field.Address);
//		String mls = getString(Field.MLS);
//		String price = getString(Field.Price);
//		String virtual_tour = getString(Field.VirtualTourUrl);
//		String more_info = getString(Field.MoreInfoUrl);
//		
//		String html = 
//				"Listing: " + (listing.isEmpty()? "<<empty>>" : listing) + "<br>" +
//				"MLS# " + mls + "<br>" + " Price  " + (price.isEmpty()? "<<empty>>" : price);
////				"Third Party Virtual Tour<br>" +
////				"	" + (virtual_tour.isEmpty()? "<<empty>>" : virtual_tour) + "<br>";
//	
//		html += "More Info Link<br>";
//		if (more_info.isEmpty())
//			html += "<<empty>>";
//		else
//			html += "<<a href=\"" + more_info + "\">>" + more_info + "<</a>>";
//		
////		display.setData(html);
//		
//	}
//	
//	private void saveData() {
////		node.setData(Field.SuiteId, new Data(display.getSelectedSuite().getId()));
////		node.setData(Field.SuiteName, new Data(display.getSelectedSuite().getName()));
////		node.setData(Field.Address, new Data(display.getSelectedSuite().getAddress()));
////		node.setData(Field.MLS, new Data(display.getSelectedSuite().getMLS()));
//	}

}
