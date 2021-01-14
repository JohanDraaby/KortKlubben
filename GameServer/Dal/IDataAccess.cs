using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Dal
{
    interface IDataAccess
    {
        bool CreateUser(string username, string password, string mail);
        string ReadUser(string username);
        bool ModifyMail(string username, string mail);
        bool ModifyPassword(string username, string newPassword);
        bool DeleteUser(string username, string password);
    }
}
