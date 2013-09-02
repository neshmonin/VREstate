package com.condox.order.client;

import com.google.gwt.core.client.GWT;
import com.google.gwt.editor.client.Editor;
import com.google.gwt.editor.client.SimpleBeanEditorDriver;
import com.google.gwt.uibinder.client.UiBinder;
import com.google.gwt.user.client.ui.Composite;
import com.google.gwt.user.client.ui.Widget;

public class ContactEditor extends Composite implements Editor<Person> {

    interface Binder extends UiBinder<Widget, ContactEditor> {}

    interface ContactEditorDriver extends
        SimpleBeanEditorDriver<Person, ContactEditor> {}
    private final ContactEditorDriver editorDriver;

//    @UiField TextBox salutation;

    public ContactEditor(Person person) {
    	initWidget(GWT.<Binder> create(Binder.class).createAndBindUi(this));
        editorDriver = GWT.create(ContactEditorDriver.class);
        editorDriver.initialize(this); 
        editorDriver.edit(person);
    }
}