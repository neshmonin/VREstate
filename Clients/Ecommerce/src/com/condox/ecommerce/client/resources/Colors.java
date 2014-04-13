package com.condox.ecommerce.client.resources;

import com.google.gwt.resources.client.CssResource;

public interface Colors extends CssResource {

	@ClassName("suite-disabled")
	String suite_disabled();

	@ClassName("suite-resale")
	String suite_resale();

	@ClassName("suite-rent")
	String suite_rent();
	
	@ClassName("suite-default")
	String suite_default();
	
	@ClassName("resale-color")
	String resale_color();
	
	@ClassName("rent-color")
	String rent_color();
}
