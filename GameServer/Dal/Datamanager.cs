using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace GameServer.Dal
{
    class Datamanager : IDataAccess
    {
        SqlConnection conn;

        string connectionString = @"Server=(localdb)\mssqllocaldb;Database=CardDb;Trusted_Connection=True;";

        public bool CreateUser()
        {
            bool userCreated = true;

            using (conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("CreateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = "usernameTest";
                cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = "passwordTest";
                cmd.Parameters.Add("@Mail", SqlDbType.VarChar).Value = "mailTest";

                cmd.ExecuteNonQuery();
            }

            return userCreated;
        }

        public bool DeleteUser()
        {
            bool userDeleted = true;

            using (conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DeleteUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = "usernameTest";

                cmd.ExecuteNonQuery();
            }

            return userDeleted;
        }

        public string ReadUser()
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser()
        {
            throw new NotImplementedException();
        }
    }
}
