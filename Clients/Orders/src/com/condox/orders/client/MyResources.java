package com.condox.orders.client;

import com.google.gwt.core.client.GWT;
import com.google.gwt.resources.client.ClientBundle;
import com.google.gwt.resources.client.CssResource;
import com.google.gwt.resources.client.CssResource.NotStrict;
import com.google.gwt.resources.client.DataResource;
import com.google.gwt.resources.client.TextResource;

public interface MyResources extends ClientBundle {
	  public static final MyResources INSTANCE =  GWT.create(MyResources.class);
//	  @NotStrict
//	  @Source("MyStyles.css")
//	  public CssResource css();

//	  @Source("config.xml")
//	  public TextResource initialConfiguration();
//
//	  @Source("manual.pdf")
//	  public DataResource ownersManual();
	}
