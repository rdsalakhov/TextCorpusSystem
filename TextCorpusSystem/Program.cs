using System;
using System.Configuration;
using Npgsql;
using System.IO;
namespace TextCorpusSystem
{
    internal class Program
    {
        
            public static void Main(string[] args)
            {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (NpgsqlConnection dbConnection = new NpgsqlConnection(connectionString))
            {
                dbConnection.Open();
                StreamReader annotationStream = new StreamReader("ABl_25_1-new.ann");
                StreamReader textStream = new StreamReader("text1.txt");
                TextManager.UploadText(textStream, annotationStream);

                //NpgsqlCommand command = new NpgsqlCommand();
                //command.CommandText = "insert into tagnames (tagname) values ('Preposition')";
                //command.Connection = dbConnection;
                //int n = command.ExecuteNonQuery();
                //Console.WriteLine($"Added {n} items");
                //command.CommandText = "select * from tagnames";
                //var reader = command.ExecuteReader();
                //while (reader.Read())
                //{
                //    Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)}");
                //}
            }

        }
    }
}