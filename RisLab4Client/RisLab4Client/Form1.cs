using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.IO;

namespace RisLab4Client {
    public partial class Form1 : Form {
        private bool isExitButton = false;

        private const int TCP_PORT = 8888;
        private const string TCP_ADDRESS = "127.0.0.1";
        private TcpClient client = null;
        private NetworkStream stream;


        // Form Events

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.CreateConnsection();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (!this.isExitButton) {
                e.Cancel = true;
                isExitButton = true;
                this.CloseClient();
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            CreateForm createForm = new CreateForm(this);
            createForm.Show();
        }

        private void button2_Click(object sender, EventArgs e) {
            FormShow formShow = new FormShow(this.ReceiveTextbooks());
            formShow.Show();
        }

        private void button3_Click(object sender, EventArgs e) {
            Item testTextbook;
            if ((testTextbook = this.FindById()) != null) {
                CreateForm createForm = new CreateForm(this, testTextbook);
                createForm.Show();
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            Item testTextbook;
            if ((testTextbook = this.FindById()) != null) {
                this.SendRemove(testTextbook.Id);
                MessageBox.Show("Запись удалена", "Готово", MessageBoxButtons.OK);
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            FilterForm filterForm = new FilterForm(this);
            filterForm.Show();
        }

        private void button7_Click(object sender, EventArgs e) {
            isExitButton = true;
            this.CloseServer();
            this.Close();
        }


        // TCP Connection

        private Item FindById() {
            Item textbook = null;
            string stringId = textBox1.Text;
            if (Form1.isInt(stringId)) {
                int id = 0;
                Int32.TryParse(stringId, out id);
                if((textbook = this.GetById(id)) == null) {
                    MessageBox.Show("ИД некорректен!", "Error", MessageBoxButtons.OK);
                }
            } else {
                MessageBox.Show("Ожидалось целое число!", "Error", MessageBoxButtons.OK);
            }
            return textbook;
        }

        private void CreateConnsection() {
            try {
                client = new TcpClient(Form1.TCP_ADDRESS, Form1.TCP_PORT);
                stream = client.GetStream();
            } catch (Exception ex) {
                this.isExitButton = true;
                MessageBox.Show("Ошибка соединения!", "Error", MessageBoxButtons.OK);
                this.Close();
            }
        }

        private string SendQuery(string message) {
            try {
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                data = new byte[1024];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (stream.DataAvailable);
                return builder.ToString();
            } catch(Exception ex) {
                this.isExitButton = true;
                MessageBox.Show("Сервер завершил работу!", "Error", MessageBoxButtons.OK);
                this.Close();
                Environment.Exit(0);
                return "EXIT";
            }
        }

        public List<Item> ReceiveTextbooks() {
            string answer = this.SendQuery("GET_ALL|");
            return DejsonizeList(answer);
        }

        public List<Item> ReceiveTextbooks(string sortField, string findField, string findValue) {
            string answer = this.SendQuery("GET_FILTER|" + sortField + "|" + findField + "|" + findValue);
            return DejsonizeList(answer);
        }

        public bool SendUppend(Item textbook, bool isCreate) {
            string answer = this.SendQuery("UPDATE|" + (isCreate ? "true" : "false") + "|" + this.JsonizeObject(textbook));
            return answer == "true";
        }

        private bool SendRemove(int id) {
            string answer = this.SendQuery("REMOVE|" + id.ToString());
            return answer == "true";
        }

        private Item GetById(int id) {
            string answer = this.SendQuery("GET_ONE|" + id.ToString());
            return this.DejsonizeObject(answer);
        }

        private void CloseServer() {
            this.SendQuery("DIE_SERVER|");
            if (client != null) {
                client.Close();
            }
        }

        private void CloseClient() {
            this.SendQuery("DIE|");
            if (client != null) {
                client.Close();
            }
        }


        // Services: JSON, Format

        public static bool isInt(string value) {
            int result;
            return Int32.TryParse(value, out result);
        }

        public static bool isDouble(string value) {
            double result;
            return Double.TryParse(value, out result);
        }

        private string JsonizeList(List<Item> toJsonTextbooks) {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Item>));

            using (MemoryStream stream = new MemoryStream()) {
                jsonFormatter.WriteObject(stream, toJsonTextbooks);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private List<Item> DejsonizeList(string value) {

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Item>));

            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(value))) {
                return (List<Item>)jsonFormatter.ReadObject(stream);
            }
        }

        private string JsonizeObject(Item toJsonTextbook) {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Item));

            using (MemoryStream stream = new MemoryStream()) {
                jsonFormatter.WriteObject(stream, toJsonTextbook);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private Item DejsonizeObject(string value) {

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Item));

            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(value))) {
                return (Item)jsonFormatter.ReadObject(stream);
            }
        }
      
    }
}