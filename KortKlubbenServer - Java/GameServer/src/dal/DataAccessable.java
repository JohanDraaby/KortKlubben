package dal;

public interface DataAccessable {
    boolean createUser();
    String readUser();
    boolean updateUser();
    boolean deleteUser();
}
