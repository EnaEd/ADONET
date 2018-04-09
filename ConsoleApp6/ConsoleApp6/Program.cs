using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//для работы с БД подключаем 
using System.Data;
using System.Data.SqlClient;
//для работы с конфигурационным файлом
using System.Configuration;
namespace ConsoleApp6
{
    class Program
    {
        SqlConnection conn = null;


        static void Main(string[] args)
        {
            Program pr = new Program();
            // pr.InsertQuery();
            //pr.ReadData();
            // pr.UseSP();
            pr.CreateTable();

        }

        //метод добавления в таблицу данных
        public void InsertQuery() {
            //using (conn = new SqlConnection(@"Data Source=ENA-ED\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True;Pooling=False;")) {
            //    conn.Open();
            //    using (SqlCommand SqlCommand = new SqlCommand()) {
            //        SqlCommand.Connection = conn;
            //        SqlCommand.CommandText = @"INSERT INTO Authors(FirstName,LastName)VALUES(N'Роджер',N'Желязный')";
            //        SqlCommand.ExecuteNonQuery();
            //    }
            //}
            using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString)) {
                conn.Open();
                using (SqlCommand SqlCommand = new SqlCommand())
                {
                    SqlCommand.Connection = conn;
                    SqlCommand.CommandText = @"INSERT INTO Authors(FirstName,LastName)VALUES(N'Ричард',N'Бах')";
                    SqlCommand.ExecuteNonQuery();
                }
            }
        }
        //метод чтения данных
        #region ReadData(читаем как обычный массив)
        //public void ReadData() {
        //    using (conn = new SqlConnection(@"Data Source=ENA-ED\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True;Pooling=False;")) {//создаем обьект подключения и передаем в него строку подключения
        //        conn.Open();//открываем подключение
        //        SqlDataReader SqlDataReader = null;//создаем обект класса для обработки нашего запроса(для возможности его отображения)
        //        using (SqlCommand cmd = new SqlCommand(@"SELECT*FROM Authors",conn)) {//создаем обькт содержащий нашу команду
        //            SqlDataReader = cmd.ExecuteReader();//передаем в обьект данные запроса
        //            while (SqlDataReader.Read()) {//метод read когда доходит до конца передает false
        //                Console.WriteLine(SqlDataReader[1]+" "+SqlDataReader[2]);//вывод полученных данных. SqlDataReadr хранит полученные данные в массиве
        //            }
        //        } 
        //    }
        //}
        #endregion
        #region ReadData(используем дополнительные сво-ва класса)
        //public void ReadData() {
        //    using (conn = new SqlConnection(@"Data Source=ENA-ED\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True;Pooling=False;")) {
        //        conn.Open();
        //        SqlDataReader rdr = null;
        //        using (SqlCommand cmd=new SqlCommand(@"SELECT*FROM Authors",conn)) {
        //            rdr = cmd.ExecuteReader();
        //            int line = 0;//счетчик полей
        //            while (rdr.Read()) {
        //                if (line == 0) {
        //                    for (int i = 0; i < rdr.FieldCount; i++)
        //                    {
        //                        Console.Write(rdr.GetName(i).ToString()+" ");
        //                    }
        //                }
        //                Console.WriteLine();
        //                line++;
        //                Console.WriteLine(rdr[0]+" "+rdr[1]+" "+rdr[2]);
        //            }
        //            Console.WriteLine("Всего строк выведено: "+line.ToString());
        //        }
        //    }
        //}
        #endregion
        #region ReadData(с возможностью обработки нескольких запросов)
        public void ReadData() {
            using (conn = new SqlConnection(@"Data Source=ENA-ED\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True;Pooling=False;")) {
                conn.Open();
                SqlDataReader rdr = null;
                using (SqlCommand cmd = new SqlCommand("SELECT*FROM Authors;SELECT*FROM Books;SELECT*FROM Authors WHERE FirstName LIKE '[РГ]%'", conn)) {//проставляем множественные запросы через ;
                    rdr = cmd.ExecuteReader();
                    int line = 0;
                    do//добавляем цикл для перехода к следующему запросу
                    {
                        while (rdr.Read()) {
                            if (line == 0) {
                                for (int i = 0; i < rdr.FieldCount; i++)
                                {
                                    Console.Write(rdr.GetName(i).ToString() + " ");
                                }
                                Console.WriteLine();
                            }
                            line++;
                            Console.WriteLine(rdr[0] + " " + rdr[1] + " " + rdr[2]);
                        }
                        Console.WriteLine($"Всего строк обработано: {line.ToString()}");

                    } while (rdr.NextResult());//переход к следующему запросу
                }

            }
        }
        #endregion
        #region Используем хранимые процедуры
        public void UseSP()
        {
            using (conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString))
            {//строка подключения из конфиг файла
                conn.Open();//открываем БД
                using (SqlCommand cmd = new SqlCommand("GetBooksNumber", conn))
                {//передаем в команду ХП
                    cmd.CommandType = CommandType.StoredProcedure;//в типе команды указываем что передали именно ХП
                    cmd.Parameters.Add("@IdAuthors", System.Data.SqlDbType.Int).Value = 1;//Задаем параметры данных входящие
                    SqlParameter outparam = new SqlParameter("@QuantBooks", System.Data.SqlDbType.Int);//Задаем параметры данных выходящие
                    outparam.Direction = ParameterDirection.Output;//задаем не значение а указываем что это выходной параметр
                    cmd.Parameters.Add(outparam);//добавляем в коллекцию 
                    cmd.ExecuteNonQuery();//выполняем запрос
                    Console.WriteLine(cmd.Parameters["@QuantBooks"].Value.ToString());//вытаскиваем выходной параметр
                }
            }
        }
        #endregion
        #region Задание 2(написать код который создает в базе данных таблицу группа)
        //Создаем базу данных из запроса CREATE DATABASE Students
        //Вносим дополнительные изменения в конфиг. файл
        public void CreateTable() {//
            using (conn=new SqlConnection(ConfigurationManager.ConnectionStrings["ConnToStudents"].ConnectionString)) {//задаем строку подключения из измененного конфиг файла
                conn.Open();//открыли подключение
                using (SqlCommand cmd=new SqlCommand(@"CREATE TABLE Group1(Id int PRIMARY KEY IDENTITY(1,1),Name nvarchar(20)NOT NULL UNIQUE)",conn) ) {//передаем команду создания таблицы
                    cmd.ExecuteNonQuery();//выполняем пакет 
                }
            }
        }
#endregion
    }
}
