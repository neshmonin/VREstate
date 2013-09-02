package com.condox.order.client;

import com.google.gwt.core.client.GWT;
import com.google.gwt.editor.client.SimpleBeanEditorDriver;

public class PersonEditingWorkflow {
		  // Empty interface declaration, similar to UiBinder
		  interface Driver extends SimpleBeanEditorDriver<Person, PersonEditor> {}
		 
		  // Create the Driver
		  Driver driver = GWT.create(Driver.class);
		 
		  void edit(Person p) {
		    // PersonEditor is a DialogBox that extends Editor<Person>
		    PersonEditor editor = new PersonEditor();
		    // Initialize the driver with the top-level editor
		    driver.initialize(editor);
		    // Copy the data in the object into the UI
		    driver.edit(p);
		     // Put the UI on the screen.
		    editor.center();
		  }
		 
		  // Called by some UI action
		  void save() {
		    Person edited = driver.flush();
		    if (driver.hasErrors()) {
		      // A sub-editor reported errors
		    }
//		    doSomethingWithEditedPerson(edited);
		  }
		}
