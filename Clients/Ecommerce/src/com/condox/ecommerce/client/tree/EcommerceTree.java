package com.condox.ecommerce.client.tree;

import java.util.ArrayList;
import java.util.List;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.container.I_Container;
import com.condox.clientshared.tree.Data;
import com.condox.ecommerce.client.I_Presenter;
import com.condox.ecommerce.client.tree.presenter.AgreementPresenter;
import com.condox.ecommerce.client.tree.presenter.ChangingPasswordPresenter;
import com.condox.ecommerce.client.tree.presenter.CongratulationsPresenter;
import com.condox.ecommerce.client.tree.presenter.ForgotPasswordPresenter;
import com.condox.ecommerce.client.tree.presenter.HelloPresenter;
import com.condox.ecommerce.client.tree.presenter.HistoryPresenter;
import com.condox.ecommerce.client.tree.presenter.LoginPresenter;
import com.condox.ecommerce.client.tree.presenter.NewOrderPresenter;
import com.condox.ecommerce.client.tree.presenter.OptionsPresenter;
import com.condox.ecommerce.client.tree.presenter.OrderSourcePresenter;
import com.condox.ecommerce.client.tree.presenter.PickBuildingPresenter;
import com.condox.ecommerce.client.tree.presenter.PickSuitePresenter;
import com.condox.ecommerce.client.tree.presenter.ProfileStep1Presenter;
import com.condox.ecommerce.client.tree.presenter.ProfileStep2Presenter;
import com.condox.ecommerce.client.tree.presenter.SettingsPresenter;
import com.condox.ecommerce.client.tree.presenter.SummaryPresenter;
import com.condox.ecommerce.client.tree.view.AgreementView;
import com.condox.ecommerce.client.tree.view.ChangingPasswordView;
import com.condox.ecommerce.client.tree.view.CongratulationsView;
import com.condox.ecommerce.client.tree.view.ForgotPasswordView;
import com.condox.ecommerce.client.tree.view.HelloView;
import com.condox.ecommerce.client.tree.view.HistoryView;
import com.condox.ecommerce.client.tree.view.LoginView;
import com.condox.ecommerce.client.tree.view.NewOrderView;
import com.condox.ecommerce.client.tree.view.OptionsView;
import com.condox.ecommerce.client.tree.view.OrderSourceView;
import com.condox.ecommerce.client.tree.view.PickBuildingView;
import com.condox.ecommerce.client.tree.view.PickSuiteView;
import com.condox.ecommerce.client.tree.view.ProfileStep1View;
import com.condox.ecommerce.client.tree.view.ProfileStep2View;
import com.condox.ecommerce.client.tree.view.SettingsView;
import com.condox.ecommerce.client.tree.view.SummaryView;
import com.google.gwt.user.client.ui.Composite;

public class EcommerceTree {

	public I_Container container = new PopupContainer(); // TODO first version.

	private List<String> leafs = new ArrayList<String>();
	private List<String> MVPs = new ArrayList<String>();
	private EcommerceNode currNode = new EcommerceNode("Defaults");

	public enum Actions {
		Guest, Agent, Close, ProfileStep1, Settings, NewOrder, History, ForgotPassword, Logout, Next, Finish, Cancel, Prev, SelectAvatar, Ok, Submit, UsingMLS, UsingAddress, IncorrectMLS, Congratulations
	}

	public void config() {
		leafs.add("Defaults/Login.ForgotPassword/ForgotPassword.Submit/ChangingPassword.Close=>Defaults/Login");
		leafs.add("Defaults/Login.ForgotPassword/ForgotPassword.Close=>Defaults/Login");
		
		leafs.add("Defaults/Login.Guest/Hello.Logout=>Defaults/Login");
		leafs.add("Defaults/Login.Guest/Hello.ProfileStep1/ProfileStep1.Close=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.Settings/Settings.Close=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.Cancel=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.Prev=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Cancel=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Prev=>Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Cancel=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Prev=>Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Cancel=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Prev=>Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Cancel=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Prev=>Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Next/Agreement.Cancel=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Next/Agreement.Prev=>Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary");
		leafs.add("Defaults/Login.Guest/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Next/Agreement.Congratulations/Congratulations.Cancel=>Defaults/Login.Guest/Hello");
		leafs.add("Defaults/Login.Guest/Hello.History/History.Close=>Defaults/Login.Guest/Hello");

		leafs.add("Defaults/Login.Agent/Hello.Logout=>Defaults/Login");
		leafs.add("Defaults/Login.Agent/Hello.ProfileStep1/ProfileStep1.Close=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.ProfileStep1/ProfileStep1.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.ProfileStep1/ProfileStep1.Next/ProfileStep2.Close=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.ProfileStep1/ProfileStep1.Finish=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.Settings/Settings.Close=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.Prev=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingMLS/Options.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingMLS/Options.Prev=>Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingMLS/Options.Next/Summary.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingMLS/Options.Next/Summary.Prev=>Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingMLS/Options");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingMLS/Options.Next/Summary.Next/Agreement.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingMLS/Options.Next/Summary.Next/Agreement.Prev=>Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingMLS/Options.Next/Summary");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Prev=>Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Prev=>Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Prev=>Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Prev=>Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Next/Agreement.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Next/Agreement.Prev=>Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary");
		leafs.add("Defaults/Login.Agent/Hello.NewOrder/NewOrder.Next/OrderSource.UsingAddress/PickBuilding.Next/PickSuite.Next/Options.Next/Summary.Next/Agreement.Congratulations/Congratulations.Cancel=>Defaults/Login.Agent/Hello");
		leafs.add("Defaults/Login.Agent/Hello.History/History.Close=>Defaults/Login.Agent/Hello");

		MVPs.add("Defaults/null/null/null");
		MVPs.add("Login/null/LoginView/LoginPresenter");
		MVPs.add("Hello/null/HelloView/HelloPresenter");
		MVPs.add("ProfileStep1/null/ProfileStep1View/ProfileStep1Presenter");
		MVPs.add("Settings/null/SettingsView/SettingsPresenter");
		MVPs.add("NewOrder/null/NewOrderView/NewOrderPresenter");
		MVPs.add("History/null/HistoryView/HistoryPresenter");
		MVPs.add("OrderSource/null/OrderSourceView/OrderSourcePresenter");
		MVPs.add("IncorrectMLS/null/IncorrectMLSView/IncorrectMLSPresenter");
		MVPs.add("PickBuilding/null/PickBuildingView/PickBuildingPresenter");
		MVPs.add("PickSuite/null/PickSuiteView/PickSuitePresenter");
		MVPs.add("Options/null/OptionsView/OptionsPresenter");
		MVPs.add("Summary/null/SummaryView/SummaryPresenter");
		MVPs.add("Agreement/null/AgreementView/AgreementPresenter");
		MVPs.add("Congratulations/null/CongratulationsView/CongratulationsPresenter");
		MVPs.add("ForgotPassword/null/ForgotPasswordView/ForgotPasswordPresenter");
		MVPs.add("ChangingPassword/null/ChangingPasswordView/ChangingPasswordPresenter");
	}

	public void next(Actions action) {
		// Log.write("EcommerceTree.next()");
		if (currNode == null)
			currNode = new EcommerceNode("Defaults");
		else {
			currNode.setAction(action);
			String currPath = currNode.getLeaf();
			Log.write("currPath: " + currPath);
			String nextPath = null;
			for (String leaf : leafs)
				if (leaf.startsWith(currNode.getLeaf()))
					nextPath = leaf.substring(currPath.length());
			Log.write("nextPath: " + nextPath);
			if (nextPath == null)
				return;
			if (nextPath.startsWith("/")) {
				String nextNodeType = nextPath.substring(1);
				int index = nextNodeType.indexOf("/");
				if (index > 0)
					nextNodeType = nextNodeType.substring(0, index);
				index = nextNodeType.indexOf(".");
				if (index > 0)
					nextNodeType = nextNodeType.substring(0, index);
				EcommerceNode nextNode = createNode(nextNodeType);
				currNode = currNode.addChild(nextNode);
				// currNode = nextNode;
				goNode(currNode);
			} else if (nextPath.startsWith("=>")) { // Jump to prev node
				// Log.write("Jumping nextNode:");
				String nextNodeLeaf = nextPath.substring(2);
				// Log.write("nextNodeLeaf = " + nextNodeLeaf);
				while (currNode != null) {
					String str = currNode.getLeaf();
					str = str.substring(0, str.lastIndexOf("."));
					// Log.write(str);
					if (nextNodeLeaf.equals(str)) {
						goNode(currNode);
						break;
					}
					currNode = currNode.getParent();
				}
			}
		}
	}

	private EcommerceNode createNode(String nextNodeType) {
		EcommerceNode result = new EcommerceNode(nextNodeType);
		return result;
	}

	private void goNode(EcommerceNode node) {
		Composite view = null;
		I_Presenter presenter = null;
		String name = node.getName();
		for (String str : MVPs)
			if (str.startsWith(name)) {
				String[] mvp = str.split("/");
				view = getView(mvp[2]);
				presenter = getPresenter(mvp[3]);
			}
		if (presenter != null) {
			presenter.setTree(this);
			if (view != null)
				presenter.setView(view);
			presenter.go(this.container);
		}
	}

	public Data getData(Field key) {
		EcommerceNode node = (EcommerceNode) currNode;
		while (node != null) {
			if (node.getData(key) != null)
				return node.getData(key);
			else
				node = (EcommerceNode) node.getParent();
		}
		return null;
	}

	public void setData(Field key, Data value) {
		if (key != null)
			currNode.setData(key, value);
	}

	private I_Presenter getPresenter(String name) {
		if ("LoginPresenter".equals(name)) return new LoginPresenter(); 
		if ("ForgotPasswordPresenter".equals(name)) return new ForgotPasswordPresenter(); 
		if ("ChangingPasswordPresenter".equals(name)) return new ChangingPasswordPresenter(); 
		if ("HelloPresenter".equals(name)) return new HelloPresenter(); 
		if ("NewOrderPresenter".equals(name)) return new NewOrderPresenter(); 
		if ("OptionsPresenter".equals(name)) return new OptionsPresenter(); 
		if ("PickBuildingPresenter".equals(name)) return new PickBuildingPresenter(); 
		if ("PickSuitePresenter".equals(name)) return new PickSuitePresenter(); 
		if ("ProfileStep1Presenter".equals(name)) return new ProfileStep1Presenter(); 
		if ("ProfileStep2Presenter".equals(name)) return new ProfileStep2Presenter(); 
		if ("SettingsPresenter".equals(name)) return new SettingsPresenter(); 
		if ("SummaryPresenter".equals(name)) return new SummaryPresenter(); 
		if ("OrderSourcePresenter".equals(name)) return new OrderSourcePresenter(); 
		if ("AgreementPresenter".equals(name)) return new AgreementPresenter(); 
		if ("CongratulationsPresenter".equals(name)) return new CongratulationsPresenter(); 
		if ("HistoryPresenter".equals(name)) return new HistoryPresenter(); 
		return null;
	}

	private Composite getView(String name) {
		if ("LoginView".equals(name)) return new LoginView(); 
		if ("ForgotPasswordView".equals(name)) return new ForgotPasswordView(); 
		if ("ChangingPasswordView".equals(name)) return new ChangingPasswordView(); 
		if ("HelloView".equals(name)) return new HelloView(); 
		if ("NewOrderView".equals(name)) return new NewOrderView(); 
		if ("OptionsView".equals(name)) return new OptionsView(); 
		if ("PickBuildingView".equals(name)) return new PickBuildingView(); 
		if ("PickSuiteView".equals(name)) return new PickSuiteView(); 
		if ("ProfileStep1View".equals(name)) return new ProfileStep1View(); 
		if ("ProfileStep2View".equals(name)) return new ProfileStep2View(); 
		if ("SettingsView".equals(name)) return new SettingsView(); 
		if ("SummaryView".equals(name)) return new SummaryView(); 
		if ("OrderSourceView".equals(name)) return new OrderSourceView(); 
		if ("AgreementView".equals(name)) return new AgreementView(); 
		if ("CongratulationsView".equals(name)) return new CongratulationsView(); 
		if ("HistoryView".equals(name)) return new HistoryView(); 
		return null;
	}

	// -----------------

	public enum Field {
		FILTERING_BY_CITY, VirtualTourUrl, MoreInfoUrl // TODO review this
														// constants
		, UserInfo, UsingMLS, BuildingInfo, SuiteInfo, EmailToRecoverPassword

	}

	public void close() {
		container.clear();
	}

	// ------------------------
}
