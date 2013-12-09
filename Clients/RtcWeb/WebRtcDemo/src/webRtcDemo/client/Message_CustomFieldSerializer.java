package webRtcDemo.client;




import com.google.gwt.user.client.rpc.CustomFieldSerializer;
import com.google.gwt.user.client.rpc.SerializationException;
import com.google.gwt.user.client.rpc.SerializationStreamReader;
import com.google.gwt.user.client.rpc.SerializationStreamWriter;


public class Message_CustomFieldSerializer extends CustomFieldSerializer<Message> {

	@Override
	public void deserializeInstance(SerializationStreamReader streamReader,
			Message instance) throws SerializationException {
		deserialize(streamReader, instance);		
	}
	
	public static void deserialize(SerializationStreamReader streamReader,
			Message instance) throws SerializationException {
		String jsonObjectAsString = streamReader.readString();
		instance.Jso = jsonObjectAsString;		
	}

	@Override
	public void serializeInstance(SerializationStreamWriter streamWriter,
			Message instance) throws SerializationException {
		serialize(streamWriter, instance);		
	}

	public static void serialize(SerializationStreamWriter streamWriter,
			Message instance) throws SerializationException {
		streamWriter.writeString(instance.Jso);		
	}
}
