package connection;

import java.util.ArrayList;

public interface Communicatable {
    int port = 0;
    ArrayList<String> connectedDevices = null;

    void send();
    String receive();
    void checkConnections();
    void removeConnectedDevice(String ip);
}
