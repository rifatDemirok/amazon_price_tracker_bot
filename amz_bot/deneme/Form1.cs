using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.IO;
using System.Data.SqlClient;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace deneme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region
        SQLiteCommand add_asin_cmd;
        string asin;
        string sql_conn_text = @"Data Source=C:\Users\Rıfat DEMİROK\Desktop\amz_bot\amz_bot.db";
        List<string> asins_list = new List<string>();
        int count_asins;
        SQLiteCommand add_price_cmd;
        SQLiteConnection connection;
        char split_price = ' ';
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            string get_compare_cmd = "SELECT ASIN, price_old, price_new FROM prices";
            SQLiteConnection conn = new SQLiteConnection(sql_conn_text);
            SQLiteCommand compare_price_ = new SQLiteCommand(get_compare_cmd, conn);
            conn.Open();
            SQLiteDataReader take_data = compare_price_.ExecuteReader();
            while (take_data.Read())
            {
                string asin = Convert.ToString(take_data["ASIN"]);
                listBox1.Items.Add(asin);
            }
        


            //using (StreamReader sr = new StreamReader("C:\\Users\\Rıfat DEMİROK\\Desktop\\amz_bot\\amz_bot.txt"))
            //{
            //    asin = "";
            //    while ((asin = sr.ReadLine()) != null)
            //    {


            //    }
            //}

        }

        private void asin_add_bt_Click(object sender, EventArgs e)
        {

            SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_text);
            sql_conn.Open();
            using (StreamReader sr = new StreamReader("C:\\Users\\Rıfat DEMİROK\\Desktop\\amz_bot\\amz_bot.txt"))
            {
                asin = "";
                while ((asin = sr.ReadLine()) != null)
                {
                    asins_list.Add(asin);

                    string add_asin = "INSERT INTO prices(\n";
                    add_asin += "ASIN)\n";
                    add_asin += "VALUES(\n";
                    add_asin += "\t'" + asin + "'\n";
                    add_asin += ")";

                    add_asin_cmd = new SQLiteCommand(add_asin, sql_conn);
                    add_asin_cmd.ExecuteNonQuery();
                }
            }
            sql_conn.Close();

        }
        private void button1_Click(object sender, EventArgs e)
        {

            string asins_count = "SELECT COUNT (ASIN) FROM prices";
            using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_text))
            {
                SQLiteCommand asins_counter = new SQLiteCommand(asins_count, sql_conn);
                sql_conn.Open();
                int countDecimal = Convert.ToInt32(asins_counter.ExecuteScalar());
                count_asins = (int)countDecimal;
            }

            for (int i = 0; i <= 49; i++)
            {
                ChromeDriverService servis = ChromeDriverService.CreateDefaultService();
                servis.HideCommandPromptWindow = true;
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless");
                IWebDriver driver = new ChromeDriver(servis);
                driver.Navigate().GoToUrl("https://www.amazon.com/dp/" + listBox1.Items[i]);
                Thread.Sleep(500);

                driver.FindElement(By.Id("glow-ingress-line2")).Click();
                Thread.Sleep(1500);

                driver.FindElement(By.CssSelector("input[type='text'][aria-label='or enter a US zip code']")).SendKeys("87010");
                Thread.Sleep(500);

                driver.FindElement(By.Id("GLUXZipUpdate")).Click();
                Thread.Sleep(500);

                driver.Navigate().Refresh();
                Thread.Sleep(1000);

                IWebElement price_whole = driver.FindElement(By.ClassName("a-price-whole"));
                IWebElement price_fraction = driver.FindElement(By.ClassName("a-price-fraction"));
                Thread.Sleep(500);

                double price = 0.0;
                if (double.TryParse(price_whole.Text + "." + price_fraction.Text, out price))
                {
                    // price değeri başarıyla dönüştürüldü
                }
                else
                {
                    IWebElement price_2nd = driver.FindElement(By.Id("sns-base-price"));
                    Thread.Sleep(500);
                    string price2nd = price_2nd.Text;
                    string[] price_split_index = price2nd.Split(split_price);
                    string sbstr_price = price_split_index[0];
                    price = Convert.ToDouble(sbstr_price.Substring(1, sbstr_price.Length - 1));


                }

                SQLiteConnection connection = new SQLiteConnection(sql_conn_text);
                connection.Open();

                string add_price = "UPDATE prices SET price_old =" + price + " WHERE ASIN = " + "'" + listBox1.Items[i] + "'";
                SQLiteCommand add_price_cmd = new SQLiteCommand(add_price, connection);
                add_price_cmd.ExecuteNonQuery();
                driver.Quit();





            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            string asins_count = "SELECT COUNT (ASIN) FROM prices";
            using (SQLiteConnection sql_conn = new SQLiteConnection(sql_conn_text))
            {
                SQLiteCommand asins_counter = new SQLiteCommand(asins_count, sql_conn);
                sql_conn.Open();
                int countDecimal = Convert.ToInt32(asins_counter.ExecuteScalar());
                count_asins = (int)countDecimal;
            }

            for (int i = 0; i <= listBox1.Items.Count; i++)
            {
                ChromeDriverService servis = ChromeDriverService.CreateDefaultService();
                servis.HideCommandPromptWindow = true;
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless");
                IWebDriver driver = new ChromeDriver(servis);
                driver.Navigate().GoToUrl("https://www.amazon.com/dp/" + listBox1.Items[i]);
                Thread.Sleep(500);

                driver.FindElement(By.Id("glow-ingress-line2")).Click();
                Thread.Sleep(1500);

                driver.FindElement(By.CssSelector("input[type='text'][aria-label='or enter a US zip code']")).SendKeys("87010");
                Thread.Sleep(500);

                driver.FindElement(By.Id("GLUXZipUpdate")).Click();
                Thread.Sleep(500);

                driver.Navigate().Refresh();
                Thread.Sleep(1000);

                IWebElement price_whole = driver.FindElement(By.ClassName("a-price-whole"));
                IWebElement price_fraction = driver.FindElement(By.ClassName("a-price-fraction"));
                Thread.Sleep(500);

                double price = 0.0;
                if (double.TryParse(price_whole.Text + "." + price_fraction.Text, out price))
                {
                    // price değeri başarıyla dönüştürüldü
                }
                else
                {
                    IWebElement price_2nd = driver.FindElement(By.Id("sns-base-price"));
                    Thread.Sleep(500);
                    string price2nd = price_2nd.Text;
                    string[] price_split_index = price2nd.Split(split_price);
                    string sbstr_price = price_split_index[0];
                    price = Convert.ToDouble(sbstr_price.Substring(1, sbstr_price.Length - 1));
                }



                string add_price = "UPDATE prices SET price_new =" + price + " WHERE ASIN = " + "'" + listBox1.Items[i] + "'";
                SQLiteCommand add_price_cmd = new SQLiteCommand(add_price, connection);
                add_price_cmd.ExecuteNonQuery();
                driver.Quit();
            }



            for (int i = 0; i <= listBox1.Items.Count; i++)
            {
                SQLiteConnection connection = new SQLiteConnection(sql_conn_text);


                string get_compare_cmd = "SELECT ASIN, price_old, price_new FROM prices WHERE ASIN=" + "\"" + listBox1.Items[i] + "\" ";
                SQLiteConnection conn = new SQLiteConnection(sql_conn_text);
                SQLiteCommand compare_price_ = new SQLiteCommand(get_compare_cmd, conn);
                conn.Open();
                SQLiteDataReader take_data = compare_price_.ExecuteReader();
                take_data.Read();
                double new_price = Convert.ToDouble(take_data["price_new"]);
                string asin = Convert.ToString(take_data["ASIN"]);

                double old_price = Convert.ToDouble(take_data["price_old"]);

                if (old_price > new_price)
                {
                    MailMessage message = new MailMessage();
                    SmtpClient sc = new SmtpClient("smtp.outlook.com");
                    sc.Credentials = new NetworkCredential("xxxexample@outlook.com", "xxxPASSWORDxxx.");
                    sc.Port = 587;
                    sc.Host = "smtp.outlook.com";
                    sc.EnableSsl = true;
                    SmtpClient server = new SmtpClient("outlook.com");
                    message.From = new MailAddress("xxxexample@outlook.com");
                    message.To.Add("xxxexm@gmail.com");
                    message.Subject = asin + " ASİN KODLU ÜRÜNÜN FİYATI DÜŞTÜ";
                    message.Body = "FİYATI " + old_price + " OLAN" + asin + " ÜRÜNÜNÜN FİYATI " + new_price + " E DÜŞTÜ. LİNK ; " + "https://www.amazon.com/dp/" + asin;
                    sc.Send(message);
                }
            }
        }





    }
}

