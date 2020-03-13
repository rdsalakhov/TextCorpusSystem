using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
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
                string saltedPassword = HashPassword(password + GetUserSalt(login));
                NpgsqlCommand signUpUser = new NpgsqlCommand($"insert into users (login, password, isAdmin) values ('{login}', '{saltedPassword}', 'false')", db);
                signUpUser.ExecuteNonQuery();
            }
        }

        static public void SignInUser(string login, string password)
        {
            using (NpgsqlConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                string saltedPassword = HashPassword(password + GetUserSalt(login));
                NpgsqlCommand checkLoginQuery = new NpgsqlCommand($"select count(*) from users where login = '{login}' and password = '{saltedPassword}'", db);
                if ((long)checkLoginQuery.ExecuteScalar() == 0)
                    throw new InvalidPasswordException();

            }
        }

        private static string HashPassword(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.Unicode.GetBytes(password));
            string result = BitConverter.ToString(bytes).Replace("-", string.Empty);
            return result.ToLower();
        }

        private static string GetUserSalt(string login)
        {
            Random rnd = new Random(login.GetHashCode());

            string salt = (Math.Round(rnd.NextDouble(), 5) * 10).ToString();
            return salt;
        }
    }
}
