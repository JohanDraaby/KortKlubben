using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace GameServer.Dal
{
    class DataManager : IDataAccess
    {
        SqlConnection conn;

        string connectionString = @"Server=WIN-0S4008C1LUM\SQLKORTKLUBBEN,9001\mssqllocaldb;Database=CardGameDB;User Id = sa; Password=Kode1234!;";

        /// <summary>
        /// Creates a new user record in the SQL database.
        /// </summary>
        public bool CreateUser(string username, string password, string mail)
        {
            bool userCreated = true;

            using (conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("CreateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = password;
                cmd.Parameters.Add("@Mail", SqlDbType.VarChar).Value = mail;

                cmd.ExecuteNonQuery();
            }

            return userCreated;
        }

        /// <summary>
        /// Deletes a user record from the SQL database.
        /// </summary>
        public bool DeleteUser(string username, string password)
        {
            bool userDeleted = true;

            using (conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DeleteUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = password;

                cmd.ExecuteNonQuery();
            }

            return userDeleted;
        }

        /// <summary>
        /// Reads a user record from the SQL database.
        /// </summary>
        public string ReadUser(string username)
        {
            string receivedData = "";

            using (conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("ReadUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    receivedData = reader.GetValue(1).ToString() + ",";
                    receivedData += reader.GetValue(3).ToString() + ",";
                    receivedData += reader.GetValue(4).ToString();
                }
            }

            return receivedData;
        }

        /// <summary>
        /// Modifies a user's mail in the SQL database.
        /// </summary>
        public bool ModifyMail(string username, string mail)
        {
            bool succeded = true;

            using (conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("ModifyMail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@Mail", SqlDbType.VarChar).Value = mail;

                cmd.ExecuteNonQuery();
            }

            return succeded;
        }

        /// <summary>
        /// Modifies a user's password in the SQL database.
        /// </summary>
        public bool ModifyPassword(string username, string newPassword)
        {
            bool succeded = true;

            using (conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("ModifyPassword", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = newPassword;

                cmd.ExecuteNonQuery();
            }

            return succeded;
        }
    }
}
