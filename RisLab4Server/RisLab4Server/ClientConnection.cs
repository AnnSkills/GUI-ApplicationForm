using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RisLab4Server {
    class ClientConnection {
        private TcpClient client;
        private Program service;
        private bool isExit = false;
        private static int currentId = 0;
        public int clientId;
        private bool isDie = false;

        public ClientConnection(TcpClient client, Program service) {
            ClientConnection.currentId++;
            this.clientId = ClientConnection.currentId;
            Console.WriteLine("Клиент #" + this.clientId + "!...");
            this.client = client;
            this.service = service;
        }

        public void Process() {
            NetworkStream stream = null;
            try {
                stream = client.GetStream();
                this.startJob(stream);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                Console.WriteLine("Клиент #" + this.clientId + " вышел!...");
                if (stream != null) {
                    stream.Close();
                }
                if (client != null) {
                    client.Close();
                }
                if (this.isDie)
                    service.killServer();
            }
        }

        private void startJob(NetworkStream stream) {
            byte[] data = new byte[1024];
            do {
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (stream.DataAvailable);
                string message = doOperation(builder.ToString());
                if (this.isExit) break;
                data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            } while (true);
        }

        private string doOperation(string query) {
            string[] queryParams = query.Split('|');
            switch (queryParams[0]) {
                case "GET_ALL":
                    return this.executeGetAll();
                case "GET_FILTER":
                    return this.executeGetFilter(queryParams);
                case "UPDATE":
                    return this.executeUpdate(queryParams);
                case "REMOVE":
                    return this.executeRemove(queryParams);
                case "GET_ONE":
                    return this.executeGetOne(queryParams);
                case "DIE":
                    return (isExit = true).ToString();
                case "DIE_SERVER":
                    return executeDieServer();
                default: return "";
            }
        }

        private string executeGetAll() {
            Console.WriteLine("Клиент #" + this.clientId + " запросил получение списка!...");
            return service.Read();
        }

        private string executeGetFilter(string[] queryParams) {
            Console.WriteLine("Клиент #" + this.clientId + " запросил получение списка по фильтрам!...");
            return service.Filter(queryParams[1], queryParams[2], queryParams[3]);
        }

        private string executeGetOne(string[] queryParams) {
            Console.WriteLine("Клиент #" + this.clientId + " запросил получение одной сущности!...");
            int id;
            Int32.TryParse(queryParams[1], out id);
            return service.JsonizeObject(service.GetById(id));
        }

        private string executeUpdate(string[] queryParams) {
            Console.WriteLine("Клиент #" + this.clientId + " запросил обновление\\добавление сущности!...");
            if (queryParams[1] != "true") {
                return service.Update(service.DejsonizeObject(queryParams[2])) ? "true" : "false";
            } else {
                return service.Create(service.DejsonizeObject(queryParams[2])) ? "true" : "false";
            }
        }

        private string executeRemove(string[] queryParams) {
            Console.WriteLine("Клиент #" + this.clientId + " запросил обновление\\добавление сущности!...");
            int id;
            Int32.TryParse(queryParams[1], out id);
            return service.Delete(id) ? "true" : "false";
        }

        private string executeDieServer() {
            this.isDie = true;
            return (isExit = true).ToString();
        }
    }
}
