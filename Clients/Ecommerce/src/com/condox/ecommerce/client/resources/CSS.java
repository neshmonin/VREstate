package com.condox.ecommerce.client.resources;

import com.google.gwt.core.shared.GWT;
import com.google.gwt.resources.client.ClientBundle;
import com.google.gwt.resources.client.CssResource.NotStrict;

public interface CSS extends ClientBundle {

	public static final CSS Instance = GWT.create(CSS.class);

	@Source("my.css")
	@NotStrict
	My my();

}
