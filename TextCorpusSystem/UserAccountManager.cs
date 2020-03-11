using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace TextCorpusSystem
{
    public class UserAccountManager
    {
        static public void SignUpUser(string login, string password)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                db.Open();
                NpgsqlCommand checkLoginQuery = new NpgsqlCommand($"select count(*) from users where login = '{login}'", db);
                if ((long)checkLoginQuery.ExecuteScalar() > 0)
                    throw new UserAlreadyExistException();
                // TODO: encrypt passwords
                NpgsqlCommand signUpUser = new NpgsqlCommand($"insert into users (login, password, isAdmin) values ('{login}', '{password}', 'false')");
                signUpUser.ExecuteNonQuery();
            }
        }

        static public void SignInUser(string login, string password)
        {
            using (NpgsqlConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                NpgsqlCommand checkLoginQuery = new NpgsqlCommand($"select count(*) from users where login = '{login}' and password = '{password}'", db);
                if ((long)checkLoginQuery.ExecuteScalar() > 0)
                    throw new InvalidPasswordException();

            }
        }
    }
}
