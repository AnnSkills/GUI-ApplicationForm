using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace RisLab4Server {
    class Program {
        public const string FILE_NAME = "items.txt";
        private const int TCP_PORT = 8888;
        private const string TCP_ADDRESS = "127.0.0.1";
        private static TcpListener listener;
        private List<Thread> clients = new List<Thread>();


        static void Main(string[] args) {
            Program app = new Program();
            app.StartProject();
        }


        private List<Item> items = null;

        private void StartProject() {
            Console.WriteLine("Приложение учёта предметов в университете!");
            ReadData();
            StartListing();
            WriteData();
            Console.WriteLine("Программа завершила работу!");
            //Console.ReadKey();
        }

        private void StartListing() {
            try {
                listener = new TcpListener(IPAddress.Parse(Program.TCP_ADDRESS), Program.TCP_PORT);
                listener.Start();
                Console.WriteLine("Слушаем соединения.......");
                while (true) {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientConnection clientConnection = new ClientConnection(client, this);
                    Thread clientThread = new Thread(new ThreadStart(clientConnection.Process));
                    this.clients.Add(clientThread);
                    clientThread.Start();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                if (listener != null) {
                    listener.Stop();
                }
                Environment.Exit(0);
            }
        }

        public void killServer() {
            foreach (Thread clientThread in this.clients) {
                clientThread.Interrupt();
            }
            listener.Stop();
        }

        private void ReadData() {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Item>));

            using (FileStream stream = new FileStream(Program.FILE_NAME, FileMode.OpenOrCreate)) {
                try {
                    object tempObject = jsonFormatter.ReadObject(stream);
                    this.items = (List<Item>)tempObject;
                } catch (Exception ex) {
                    this.items = new List<Item>();
                }
            }
            Console.WriteLine("Данные получены!");
        }

        private void WriteData() {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Item>));

            using (FileStream stream = new FileStream(Program.FILE_NAME, FileMode.OpenOrCreate)) {
                jsonFormatter.WriteObject(stream, this.items);
            }
            Console.WriteLine("Данные сохранены!");
        }

        public String Read() {
            return this.JsonizeList(this.items);
        }

        public bool Create(Item newItem) {
            if (!isIdUnique(newItem.Id)) {
                return false;
            } else {
                this.items.Add(newItem);
                return true;
            }
        }

        private bool isIdUnique(int id) {
            foreach (Item item in this.items) {
                if (item.Id == id) {
                    return false;
                }
            }
            return true;
        }

        public bool Delete(int id) {
            foreach (Item item in this.items) {
                if (item.Id == id) {
                    this.items.Remove(item);
                    return true;
                }
            }
            return false;
        }

        public bool Update(Item updatedItem) {
            foreach (Item item in this.items) {
                if (item.Id == updatedItem.Id) {
                    item.Year = updatedItem.Year;
                    item.Faculty = updatedItem.Faculty;
                    item.Form = updatedItem.Form;
                    item.Specialty = updatedItem.Specialty;
                    item.Subject = updatedItem.Subject;
                    item.Hour = updatedItem.Hour;
                    return true;
                }
            }
            return false;
        }

        public Item GetById(int id) {
            foreach (Item item in this.items) {
                if (item.Id == id) {
                    return item;
                }
            }
            return null;
        }

        public string Filter(string fieldSort, string fieldFind, string valueFind) {

            switch (fieldSort) {
                case "ИД":
                    this.items = this.items.OrderBy(o => o.Id).ToList(); break;
                case "Год":
                    this.items = this.items.OrderBy(o => o.Year).ToList(); break;
                case "Факультет":
                    this.items = this.items.OrderBy(o => o.Faculty).ToList(); break;
                case "Форма":
                    this.items = this.items.OrderBy(o => o.Form).ToList(); break;
                case "Специальность":
                    this.items = this.items.OrderBy(o => o.Specialty).ToList(); break;
                case "Предмет":
                    this.items = this.items.OrderBy(o => o.Subject).ToList(); break;
                case "Часы":
                    this.items = this.items.OrderBy(o => o.Hour).ToList(); break;
            }
            return this.Find(fieldFind, valueFind);
        }


        private string Find(string field, string value) {
            if (value.Length <= 0) return this.JsonizeList(this.items);
            switch (field) {
                case "ИД":
                    return this.JsonizeList(this.items.Where(o => value == o.Id.ToString()).ToList()); break;
                case "Год":
                    return this.JsonizeList(this.items.Where(o => value == o.Year.ToString()).ToList()); break;
                case "Факультет":
                    return this.JsonizeList(this.items.Where(o => value == o.Faculty).ToList()); break;
                case "Форма":
                    return this.JsonizeList(this.items.Where(o => value == o.Form).ToList()); break;
                case "Специальность":
                    return this.JsonizeList(this.items.Where(o => value == o.Specialty).ToList()); break;
                case "Предмет":
                    return this.JsonizeList(this.items.Where(o => value == o.Subject).ToList()); break;
                case "Часы":
                    return this.JsonizeList(this.items.Where(o => value == o.Hour.ToString()).ToList()); break;
            }
            return this.JsonizeList(this.items);
        }

        private string JsonizeList(List<Item> toJsonItems) {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Item>));

            using (MemoryStream stream = new MemoryStream()) {
                jsonFormatter.WriteObject(stream, toJsonItems);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public string JsonizeObject(Item toJsonItem) {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Item));
            using (MemoryStream stream = new MemoryStream()) {
                jsonFormatter.WriteObject(stream, toJsonItem);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public Item DejsonizeObject(string value) {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Item));
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(value))) {
                return (Item)jsonFormatter.ReadObject(stream);
            }
        }
    }
}
