package com.condox.vrestate.client;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class State {
	public static Map<String, List<String>> contextMap = new HashMap<String, List<String>>();

	private State(){}
	public static State Create(){
		return new State();
	}

}
