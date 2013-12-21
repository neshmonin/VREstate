package com.condox.ecommerce.client.tree.view;

import com.condox.clientshared.communication.Options;
import com.condox.ecommerce.client.tree.presenter.UpdateAvatarPresenter;
import com.condox.ecommerce.client.tree.presenter.UpdateAvatarPresenter.I_Display;
import com.google.gwt.core.client.GWT;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.RootPanel;
import com.google.gwt.user.client.ui.Widget;
import com.google.gwt.uibinder.client.UiField;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.uibinder.client.UiHandler;
import com.google.gwt.event.dom.client.ClickEvent;
import com.google.gwt.user.client.ui.Image;
import com.google.gwt.user.client.ui.FileUpload;
import com.google.gwt.event.dom.client.ChangeEvent;
import com.google.gwt.event.shared.EventHandler;
import com.google.gwt.event.shared.GwtEvent;
import com.google.gwt.user.client.ui.FormPanel;
import com.google.gwt.user.client.ui.FormPanel.SubmitCompleteEvent;
import com.google.gwt.event.dom.client.LoadEvent;
import com.google.gwt.user.client.ui.HTML;

public class UpdateAvatarView extends Composite implements I_Display {

	private static UpdateAvatarViewUiBinder uiBinder = GWT
			.create(UpdateAvatarViewUiBinder.class);
	@UiField
	Button buttonApply;
	@UiField
	Button buttonClose;
	@UiField FormPanel form;
	@UiField FileUpload upload;
	@UiField HTML htmlImage;

	interface UpdateAvatarViewUiBinder extends
			UiBinder<Widget, UpdateAvatarView> {
	}

	private UpdateAvatarPresenter presenter = null;

	public UpdateAvatarView() {
		initWidget(uiBinder.createAndBindUi(this));
	}

	@Override
	public void setPresenter(UpdateAvatarPresenter presenter) {
		this.presenter = presenter;
	}

	@UiHandler("upload")
	void onUploadChange(ChangeEvent event) {
		form.setEncoding(FormPanel.ENCODING_MULTIPART);
		form.setMethod(FormPanel.METHOD_POST);
		form.setAction("");
		
		String html = "";
		html += "<img src=\"" + Options.URL_VRT + "ecommerce/test.jpg\" id=\"cropbox\" width=400 height=400/>";
		htmlImage.setHTML(html);
		f();
//		image.setUrl(Options.URL_VRT + "ecommerce/test.jpg");
//		form.submit();
	}
	
	private native void f() /*-{
		$wnd.jQuery(function(){
			$wnd.jQuery('#cropbox').Jcrop({
				trackDocument: true,
            	setSelect:   [ 100, 100, 300, 300 ],
            	aspectRatio: 1
			});
    });
	}-*/;
	
	@UiHandler("form")
	void onFormSubmitComplete(SubmitCompleteEvent event) {
	}
	@UiHandler("buttonApply")
	void onButtonApplyClick(ClickEvent event) {
		if (presenter != null)
			presenter.onApply();
	}
	@UiHandler("buttonClose")
	void onButtonCloseClick(ClickEvent event) {
		if (presenter != null)
			presenter.onClose();
	}
}
