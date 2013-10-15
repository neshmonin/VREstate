package com.condox.order.client;

import com.google.gwt.editor.client.Editor;
import com.google.gwt.user.client.ui.Button;
import com.google.gwt.user.client.ui.DialogBox;

public class PersonEditor extends DialogBox implements Editor<Person> {

	Button nameEditor;

	public PersonEditor() {
		nameEditor = new Button("nameEditor");
		nameEditor.setText("nameEditor");
		nameEditor.setSize("200px", "200px");
		add(nameEditor);
//		add(new Label("label"));
//		setWidget(nameEditor);
	}
}
