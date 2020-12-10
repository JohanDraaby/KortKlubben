import dal.DataManager;

public class Manager {
    DataManager dm;

    public void testDb() {
        dm = new DataManager();

        dm.createConnection();
    }
}
