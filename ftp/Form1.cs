using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;


namespace ftp
{
    public partial class Form1 : Form
    {
        //поле для хранения имени фтп-сервера
        private string _Host;

        //поле для хранения логина
        private string _UserName;

        //поле для хранения пароля
        private string _Password;

        //объект для запроса данных
        FtpWebRequest ftpRequest;

        //объект для получения данных
        FtpWebResponse ftpResponse;
        //поле для хранения пути
        private string path = "";

        public Form1()
        {
            InitializeComponent();
        }
        private void InitConnectData()
        {
            _Host = textHost.Text;
            _UserName = textUser.Text;
            _Password = textPassword.Text;
        }

        private void loadtree(FtpWebResponse ftpResponse)
        {
            StreamReader sr = new StreamReader(ftpResponse.GetResponseStream(), System.Text.Encoding.ASCII);
            string line;
            while ((line = sr.ReadLine()) != null) //читаем по одной линии(строке) пока не вычитаем все из потока (пока не достигнем конца файла)
            {
                treeView1.Nodes.Add(line);
            }
            sr.Close();
            ftpResponse.Close();
        }

        private void FtpConnect(string path)
        {
            label6.Text = path;
            treeView1.Nodes.Clear();
            //Создаем объект запроса
            ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + _Host + path);
            //логин и пароль
            ftpRequest.Credentials = new NetworkCredential(_UserName, _Password);
            //команда фтп LIST
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            ftpRequest.EnableSsl = false;
            //Получаем входящий поток
            try
            {
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                label5.Text = "Соединение установлено";
                loadtree(ftpResponse);
            }
            catch (WebException)
            {
                label5.Text = "Соединение не удалось";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textHost.Enabled = false;
            textUser.Enabled = false;
            textPassword.Enabled = false;
            button1.Enabled = false;
            InitConnectData();
            FtpConnect(path);
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(treeView1.SelectedNode.Text.Substring(24, 5) == "<DIR>")
            {
                string papka;
                papka = treeView1.SelectedNode.Text.Substring(39, treeView1.SelectedNode.Text.Length - 39);
                path += '/' + papka;
                FtpConnect(path);
            }
        }
    }
}
