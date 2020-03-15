using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        public static int SignInUser(string login, string password)
        {
            using (NpgsqlConnection db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                db.Open();
                string saltedPassword = HashPassword(password + GetUserSalt(login));
                NpgsqlCommand checkLoginQuery = new NpgsqlCommand($"select id from users where login = '{login}' and password = '{saltedPassword}'", db);
                NpgsqlDataReader userReader = checkLoginQuery.ExecuteReader();
                if (userReader.HasRows)
                {
                    userReader.Read();
                    return userReader.GetInt32(0);
                }
                else
                {
                    throw new InvalidPasswordException();
                }
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

        public static bool CheckUserAccess(int userId, int textId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                db.Open();

                string checkAccessQueryString = "select count(*) from userAccess where user_id = @userId and text_id = @textId";
                NpgsqlCommand checkAccessQuery = new NpgsqlCommand(checkAccessQueryString, db);
                checkAccessQuery.Parameters.Add("@userid", NpgsqlTypes.NpgsqlDbType.Integer);
                checkAccessQuery.Parameters.Add("@textid", NpgsqlTypes.NpgsqlDbType.Integer);
                checkAccessQuery.Parameters["@userid"].Value = userId;
                checkAccessQuery.Parameters["@textid"].Value = textId;

                if ((long)checkAccessQuery.ExecuteScalar() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool CheckAdminStatus(int userId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                db.Open();

                string checkIsAdminQueryString = "select isadmin from users where id = @userId";
                NpgsqlCommand checkIsAdminqQuery = new NpgsqlCommand(checkIsAdminQueryString, db);
                checkIsAdminqQuery.Parameters.Add("@userid", NpgsqlTypes.NpgsqlDbType.Integer);
                checkIsAdminqQuery.Parameters["@userid"].Value = userId;

                if ((bool)checkIsAdminqQuery.ExecuteScalar())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void GrantAdminStatus(int userId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                db.Open();

                string grantAdminStatusCommandString = "update users set isadmin = True where user_id = @userId";
                NpgsqlCommand grantAdminStatusCommand = new NpgsqlCommand(grantAdminStatusCommandString, db);
                grantAdminStatusCommand.Parameters.Add("@userid", NpgsqlTypes.NpgsqlDbType.Integer);
                grantAdminStatusCommand.Parameters["@userid"].Value = userId;

                grantAdminStatusCommand.ExecuteNonQuery();
            }
        }

        public static void GrantAccessToText(int userId, int textId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                db.Open();

                string giveAccessCommandString = "insert into userAccess (user_id, text_id) values (@userId, @textId)";
                NpgsqlCommand grantAccessCommand = new NpgsqlCommand(giveAccessCommandString, db);
                grantAccessCommand.Parameters.Add("@userid", NpgsqlTypes.NpgsqlDbType.Integer);
                grantAccessCommand.Parameters["@userid"].Value = userId;
                grantAccessCommand.Parameters.Add("@textid", NpgsqlTypes.NpgsqlDbType.Integer);
                grantAccessCommand.Parameters["@textid"].Value = textId;

                grantAccessCommand.ExecuteNonQuery();
            }
        }

        public DataSet GetUsersDataSet()
        {
            DataSet usersDataSet = new DataSet();
            using (var db = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                NpgsqlDataAdapter usersDataAdapter = new NpgsqlDataAdapter("select id, login from users", db);
                usersDataAdapter.Fill(usersDataSet, "users");
            }
            return usersDataSet;
        }

        public static void DeleteAccount(int userId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (NpgsqlConnection db = new NpgsqlConnection(connectionString))
            {
                db.Open();

                string deleteAccountCommandString = "delete from users where id = @userId";
                NpgsqlCommand deleteAccountCommand = new NpgsqlCommand(deleteAccountCommandString, db);
                deleteAccountCommand.Parameters.Add("@userid", NpgsqlTypes.NpgsqlDbType.Integer);
                deleteAccountCommand.Parameters["@userid"].Value = userId;
                deleteAccountCommand.ExecuteNonQuery();
            }
        }
    }
}
