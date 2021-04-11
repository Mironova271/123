using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace register
{
    class Database
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public Database()
        {
            Initialize();
        }

        private void Initialize()
        {
            server = "localhost";
            database = "users";
            uid = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
            }
        }

        private void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private MySqlConnection GetMySqlConnection()
        {
            return connection;
        }
    
        public void authorization(string log, string pass)
        {
            var conn = GetMySqlConnection();

            string req = $"Select * from users where u_log = @l and u_pass = @p";
            string req2 = $"Select * from users where u_log = @l";
            

            MySqlCommand msql = new MySqlCommand(req, conn);

            msql.Parameters.AddWithValue("@l", log);
            msql.Parameters.AddWithValue("@p", pass);

            OpenConnection();

            MySqlDataReader reader = msql.ExecuteReader();

            if (reader.HasRows)
            {
                MessageBox.Show("Вы вошли успешно!");
                reader.Close();
                CloseConnection();
            }
            else
            {
                reader.Close();
                CloseConnection();

                msql = new MySqlCommand(req2, conn);

                msql.Parameters.AddWithValue("@l", log);

                OpenConnection();

                reader = msql.ExecuteReader();
                if(reader.HasRows)
                    MessageBox.Show("Вы ввели неверный пароль");
                else
                    MessageBox.Show("Такого аккаунта не существует!");

                reader.Close();
                CloseConnection();
            }

        }

        public bool usertrue(string login)
        {
            var conn = GetMySqlConnection();

            string req = $"Select * from users where u_log = @l";

            MySqlCommand msql = new MySqlCommand(req, conn);
            msql.Parameters.AddWithValue("@l", login);
            OpenConnection();

            MySqlDataReader reader = msql.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                CloseConnection();
                return true;
            }
            else
            {
                reader.Close();
                CloseConnection();
                return false;
            }
        }

        public void register(string log, string pass,string repass)
        {
            var conn = GetMySqlConnection();

            if(pass!=repass)
            {
                 MessageBox.Show("Пароли не совпадают!");
                 return;
            }

            if (usertrue(log))
            {
                MessageBox.Show("Такой логин уже зарегистрирован!");
                return;
            }

            string req = $"Insert into users (u_log, u_pass) values(@l,@p)";

            MySqlCommand msql = new MySqlCommand(req, conn);

            msql.Parameters.AddWithValue("@l", log);
            msql.Parameters.AddWithValue("@p", pass);

            OpenConnection();

            if(msql.ExecuteNonQuery()==1)
                MessageBox.Show("Вы успешно зарегистрировались");
            else
                MessageBox.Show("Увы, что-то пошло не так!");

            CloseConnection();
        }
    }
}
