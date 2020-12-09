package dal;

import java.sql.*;
import

/**
 * handles user CRUDs
 */
public class DataManager implements DataAccessable{

    /**
     *
     * @return
     */
    @Override
    public boolean createUser() {

        boolean creationSuccessful = false;

        Connection conn = DriverManager.getConnection("jbdc:default:connection", "root", "P@ssw0rd");;

        try {
            CallableStatement cs = conn.prepareCall("{CALL CreateUser(?,?,?)}");
        }
        catch (SQLException e) {

        }


        return creationSuccessful;
    }

    /**
     *
     * @return
     */
    @Override
    public String readUser() {
        return null;
    }

    /**
     *
     * @return
     */
    @Override
    public boolean updateUser() {
        return false;
    }

    /**
     *
     * @return
     */
    @Override
    public boolean deleteUser() {
        return false;
    }
}
