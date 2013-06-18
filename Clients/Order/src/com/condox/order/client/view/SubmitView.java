package com.condox.order.client.view;

import com.condox.order.client.presenter.SubmitPresenter;
import com.google.gwt.core.client.GWT;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.event.dom.client.KeyUpEvent;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.user.client.Element;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.HTML;
import com.google.gwt.user.client.ui.Hyperlink;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.TextBox;
import com.google.gwt.user.client.ui.VerticalPanel;
import com.google.gwt.user.client.ui.Widget;

public class SubmitView extends Composite implements SubmitPresenter.IDisplay {

	private static SubmitViewUiBinder uiBinder = GWT
			.create(SubmitViewUiBinder.class);
	@UiField TextBox textName;
	@UiField TextBox textEmail;
	@UiField TextBox textPhone;
	@UiField TextBox textPhoneExt;
	@UiField Button buttonSubmit;
	@UiField Button button;
	@UiField Hyperlink hyperlink;
	@UiField Hyperlink hyperlink_1;
	@UiField RadioButton rbPrivate;
	@UiField VerticalPanel vpFloorplan;
	@UiField HTML htmlFloorplanUrl;
	@UiField HTML htmlPayPal;

	interface SubmitViewUiBinder extends UiBinder<Widget, SubmitView> {
	}

	public SubmitView() {
		initWidget(uiBinder.createAndBindUi(this));
		ValidateInput();
	/*	String s = "<div><script src=\"paypal-button.min.js?merchant=ecommerce@3dcondox.com\" "; 
	    s += "data-env=\"sandbox\""; 
	    s += "data-callback=\"http://vrt.3dcondox.com/vre/order-test/order.html\""; 
//	    s += "data-tax=\"tax\" ";
//	    s += "data-shipping=\"1234\" "; 
//	    s += "data-currency=\"USD\" ";
	    s += "data-amount=\"1234\" ";			// Price for one item
	    s += "data-quantity=\"5\" "; 
	    s += "data-name=\"Suite listing\" ";		// Description
//	    s += "data-button=\"buynow\"";
	    s += "></script></div>";
	    evalScripts(new HTML(s).getElement());
		htmlPayPal.setHTML(s);*/
	}
	
	/**
	 * Evaluate scripts in an HTML string. Will eval both <script src=""></script>
	 * and <script>javascript here</scripts>.
	 *
	 * @param element a new HTML(text).getElement()
	 */
	public static native void evalScripts(Element element) /*-{
	    var scripts = element.getElementsByTagName("script");

	    for (i=0; i < scripts.length; i++) {
	        // if src, eval it, otherwise eval the body
	        if (scripts[i].hasAttribute("src")) {
	            
	            tempScript = $doc.getElementById("tempScript");
	            if (tempScript != null)
	            	tempScript.parentNode.removeChild(tempScript);
	            
	            var src = scripts[i].getAttribute("src");
	            var script = $doc.createElement('script');
	            script.setAttribute("src", src);
	            script.setAttribute("id","tempScript");
	            $doc.getElementsByTagName('body')[0].appendChild(script);
	            
	            
	        } else {
	            $wnd.eval(scripts[i].innerHTML);
	        }
	    }
	}-*/;

	
	private boolean isValid = true;
	private void ValidateInput() {
		String name = textName.getText();
		String mail = textEmail.getText();
		String phone = textPhone.getText();
		String ext_phone = textPhoneExt.getText();

		phone = phone.replaceAll("\\s", "");
		phone = phone.replaceAll("-", "");
		phone = phone.replaceAll("\\(", "");
		phone = phone.replaceAll("\\)", "");
		isValid = true;
		isValid &= name.matches(".+");
		isValid &= mail.matches("\\S+@\\S+");
		isValid &= phone.matches("\\d{10,}");
		if (!ext_phone.isEmpty())
			isValid &= ext_phone.matches("\\d{0,4}");
//		buttonSubmit.setEnabled(isValid);
	}

	// Implementation of SubmitPresenter.IDisplay
	private SubmitPresenter presenter;

	@Override
	public void setPresenter(SubmitPresenter presenter) {
		this.presenter = presenter;
	}

	@Override
	public String getCustomerName() {
		return textName.getText();
	}

	@Override
	public String getCustomerMail() {
		return textEmail.getText();
	}

	@Override
	public String getCustomerPhone() {
		return textPhone.getText();
	}
	
	@Override
	public String getCustomerPhoneExt() {
		return textPhoneExt.getText();
	}

	@Override
	public Boolean isListingPrivate() {
		return rbPrivate.getValue();
	}

	@UiHandler("hyperlink")
	void onHyperlinkClick(ClickEvent event) {
		presenter.onCancel();
	}
	@UiHandler("hyperlink_1")
	void onHyperlink_1Click(ClickEvent event) {
		ValidateInput();
		if (isValid)
			presenter.onSubmit();
		else
			Window.alert("Check fields - there is an error!");
	}
	@UiHandler("textName")
	void onTextNameKeyUp(KeyUpEvent event) {
		ValidateInput();
	}
	@UiHandler("textEmail")
	void onTextEmailKeyUp(KeyUpEvent event) {
		ValidateInput();
	}
	@UiHandler("textPhone")
	void onTextPhoneKeyUp(KeyUpEvent event) {
		ValidateInput();
	}
	@UiHandler("textPhoneExt")
	void onTextPhoneExtKeyUp(KeyUpEvent event) {
		ValidateInput();
	}

	@Override
	public void setFloorplanUrl(String url) {
		if ((url != null)&&(!url.isEmpty())) {
			htmlFloorplanUrl.setHTML("Do you want to check the floorplan for this suite? <br> <center><a href=\"" + url + "\" target = \"_blank\">Show me...</a></center>");
		} else
			htmlFloorplanUrl.setHTML("Sorry, there is no floorplan for this suite...");
	}
}
