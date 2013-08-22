package com.condox.order.client;

import com.google.gwt.core.client.GWT;
import com.google.gwt.resources.client.ClientBundle;
import com.google.gwt.resources.client.CssResource;

public interface MyResources extends ClientBundle {
	  public static final MyResources INSTANCE =  GWT.create(MyResources.class);
//	  @NotStrict
	  @Source("MyStyles.css")
	  public CssResource css();

//	  @Source("config.xml")
//	  public TextResource initialConfiguration();
//
//	  @Source("manual.pdf")
//	  public DataResource ownersManual();
	}
