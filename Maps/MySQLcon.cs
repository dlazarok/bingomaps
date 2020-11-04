using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps
{
    public class MySQLCon
    {
        MySqlConnection conn;
        public MySQLCon(string host, string database, string username, string password, string port = "")
        {
            // Connection String.


            String connString = "Server=" + host + ";Database=" + database +
                ";User Id=" + username + ";password=" + password + ";charset=utf8";
            if (port != "")
                connString += ";port=" + port;
            conn = new MySqlConnection(connString);
        }

        public string getValue(string query)
        {
            conn.Open();
            MySqlCommand command = new MySqlCommand(query, conn);
            string value = command.ExecuteScalar().ToString();
            conn.Close();
            return value;
        }
        public List<string[]> getValues(string query)
        {
            conn.Open();
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlDataReader reader = command.ExecuteReader();
            List<string[]> values = new List<string[]>();
            int n = 0;
            while (reader.Read())
            {
                values.Add(new string[reader.FieldCount]);
                for (int i = 0; i < reader.FieldCount; i++)
                    values[n][i] = reader[i].ToString();
                n++;
            }
            reader.Close();
            conn.Close();
            return values;
        }

        public void execute(string querry)
        {
            conn.Open();
            MySqlCommand command = new MySqlCommand(querry, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public MySqlConnection getDb() => conn;
    }
}
