package dal;

import java.sql.*;

/**
 * handles user CRUDs
 */
public class DataManager implements DataAccessable{

    Connection conn;

    /**
     *
     * @return
     */
    @Override
    public boolean createUser() {

        boolean creationSuccessful = false;

        //Connection conn = DriverManager.getConnection("jbdc:default:connection", "root", "P@ssw0rd");;

        try {
            System.out.println("ttempting call to Createuser stored procedure");
            CallableStatement cs = conn.prepareCall("{CALL CreateUser(?,?,?)}");
            cs.setString(1, "testUsername");
            cs.setString(2, "testPassword");
            cs.setString(3, "testMail");

            System.out.println("Called Createser stored procedure");

            cs.close();
        }
        catch (SQLException e) {
            System.out.println("error: " + e);
        }

        return creationSuccessful;
    }

    public void createConnection() {
        try {
            System.out.println("Attempting call to create a connection");
            Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
            conn = DriverManager.getConnection("jdbc:mssql://localhost:1433/CardDb", "Kortklubben", "Kode1234!");
            System.out.println("called to create a connection");
            createUser();

            conn.close();
        }
        catch (ClassNotFoundException | SQLException e) {
            System.out.println("error: " + e);
        }
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
