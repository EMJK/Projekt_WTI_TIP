//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Ninject;

//namespace RegistrarSipApi
//{
//    public class RegistrarSipServer : IDisposable
//    {
//        public RegistrarSipServer(string host, int port, IKernel kernel)
//        {
//            Console.WriteLine($"SIP server started at {host}:{port}");
//        }

//        public void Dispose()
//        {
//            Console.WriteLine("SIP server stopped");
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Data;
using Npgsql;
using System.Linq;

namespace RegistrarSipApi
{
    enum Headers
    {
        Method,
        To,
        From,
        MaxForwards,
        Via,
        Contact,
        CallId,
        CSeq,
        Expires,
        Allow,
        UserAgent,
        ContentLength,
        Authorization,
        RecordRoute,
        Route,
        Server,
        WWWAuthenticate
    }

    public class DBpart
    {
        public NpgsqlConnection connection;

        public void CreateConnection()
        {
            string connstring = String.Format("Server=127.0.0.1;Port=5432;" +
                                "User Id=postgres;Password=EmiSUser;Database=Registrar_DB;");
            connection = new NpgsqlConnection(connstring);
            connection.Open();
        }

        public DataTable GetDataFromDB(string command, string paramName, string paramValue)
        {
            CreateConnection();
            DataTable dt = new DataTable(); // DataTable instance named dt
            NpgsqlDataReader dr; // NpgsqlDataReader object declaration named dr
            NpgsqlCommand sqlc; // NpgsqlCommand object declaration
            sqlc = new NpgsqlCommand(command); // NpgsqlCommand instance

            NpgsqlParameter par = new NpgsqlParameter(paramName, paramValue);
            sqlc.Parameters.Add(par);

            sqlc.Connection = this.connection; // connection to DB
            dr = sqlc.ExecuteReader(); // query execution and creation of dr pointer
            dt.Load(dr); //load data to dataTable object
            connection.Close();
            return dt; // return data
        }

        public int WriteDataToDB(string command, string paramName, object paramValue)
        {
            CreateConnection();
            NpgsqlCommand sqlc; // Declaration of NpgsqlCommand object
            sqlc = new NpgsqlCommand(command); // NpgsqlCommand instance to execute query

            sqlc.Connection = this.connection; // connection to DB
            NpgsqlParameter par = new NpgsqlParameter(paramName, paramValue);
            sqlc.Parameters.Add(par);
            int result = sqlc.ExecuteNonQuery();
            connection.Close();
            return result;
        }

        public int WriteDataToDB(string command, Dictionary<string, object> parameters)
        {
            CreateConnection();
            NpgsqlCommand sqlc; // Declaration of NpgsqlCommand object
            sqlc = new NpgsqlCommand(command); // NpgsqlCommand instance to execute query

            sqlc.Connection = this.connection; // connection to DB
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                NpgsqlParameter par = new NpgsqlParameter(parameter.Key, parameter.Value);
                sqlc.Parameters.Add(par);
            }
            int result = sqlc.ExecuteNonQuery();
            connection.Close();
            return result;
        }

    }

    public class RegistrarSipServer
    {
        private readonly int _basePort;
        UdpClient _socket;
        bool _stopTask = false;
        string _realm;
        readonly string _server = "SipServer EM&JK v0.9";

        public RegistrarSipServer(int basePort, string realm)
        {
            _basePort = basePort;
            _realm = realm;
        }
        public void Start()
        {
            _stopTask = false;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, _basePort);
            _socket = new UdpClient(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine($"SIP server started (SIP port {_basePort})");
            Task.Factory.StartNew(() => Listen(sender));
        }

        private void Listen(IPEndPoint sender)
        {
            // Receive Packet
            while (!_stopTask)
            {
                try
                {
                    byte[] data = _socket.Receive(ref sender);
                    string str = Encoding.ASCII.GetString(data);
                    string[] packet = str.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (packet.Length == 0) continue;
                    string[] parsed = ParsePacket(packet);
                    string method = parsed[(int)Headers.Method].Split(' ')[0];
                    switch (method)
                    {
                        case "REGISTER":
                            Console.WriteLine("REGISTER");
                            RegisterHandler(parsed, sender);
                            break;
                        default:
                            Console.WriteLine("dupa");
                            break;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to parse the packet! :-(");
                }

            }
        }

        private string[] ParsePacket(string[] packet)
        {
            string[] parsed = new string[20];
            int index = 99;
            for (int i = 0; i < packet.Length; i++)
            {
                if (packet[i].Length < 10)
                    continue;
                if (i == 0)
                {
                    parsed[(int)Headers.Method] = packet[i];
                    continue;
                }

                index = packet[i].IndexOf("To: ", 0, 4);
                if (index == 0)
                {
                    parsed[(int)Headers.To] = packet[i].Substring(4, packet[i].Length - 4);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("From: ", 0, 6);
                if (index == 0)
                {
                    parsed[(int)Headers.From] = packet[i].Substring(6, packet[i].Length - 6);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Authorizat", 0, 10);
                if (index == 0)
                {
                    parsed[(int)Headers.Authorization] = packet[i].Substring(15, packet[i].Length - 15);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Max-Forwar", 0, 10);
                if (index == 0)
                {
                    parsed[(int)Headers.MaxForwards] = packet[i].Substring(14, packet[i].Length - 14);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Via: ", 0, 5);
                if (index == 0)
                {
                    parsed[(int)Headers.Via] = packet[i].Substring(5, packet[i].Length - 5);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Contact: ", 0, 9);
                if (index == 0)
                {
                    parsed[(int)Headers.Contact] = packet[i].Substring(9, packet[i].Length - 9);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Call-ID: ", 0, 9);
                if (index == 0)
                {
                    parsed[(int)Headers.CallId] = packet[i].Substring(9, packet[i].Length - 9);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("CSeq: ", 0, 6);
                if (index == 0)
                {
                    parsed[(int)Headers.CSeq] = packet[i].Substring(6, packet[i].Length - 6);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Expires: ", 0, 9);
                if (index == 0)
                {
                    parsed[(int)Headers.Expires] = packet[i].Substring(9, packet[i].Length - 9);
                    index = 99;
                    continue;
                }

                index = 99;

                index = packet[i].IndexOf("Allow: ", 0, 7);
                if (index == 0)
                {
                    parsed[(int)Headers.Allow] = packet[i].Substring(7, packet[i].Length - 7);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("User-Agent", 0, 10);
                if (index == 0)
                {
                    parsed[(int)Headers.UserAgent] = packet[i].Substring(12, packet[i].Length - 12);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Content-Le", 0, 10);
                if (index == 0)
                {
                    parsed[(int)Headers.ContentLength] = packet[i].Substring(16, packet[i].Length - 16);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Record-Rou", 0, 10);
                if (index == 0)
                {
                    parsed[(int)Headers.RecordRoute] = packet[i].Substring(14, packet[i].Length - 14);
                    index = 99;
                    continue;
                }
                index = 99;

                index = packet[i].IndexOf("Route: ", 0, 7);
                if (index == 0)
                {
                    parsed[(int)Headers.Route] = packet[i].Substring(7, packet[i].Length - 7);
                    index = 99;
                    continue;
                }
                index = 99;

            }
            return parsed;
        }

        private string CalculateMD5Hash(string input)

        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private void RegisterHandler(string[] packet, IPEndPoint sender)
        {
            if (packet[(int)Headers.Authorization] != null)
            {
                string[] auth = packet[(int)Headers.Authorization].Split(',');
                string username = auth[0].Split('=')[1];
                username = username.Substring(1, username.Length - 2);
                string nonce = auth[2].Split('=')[1];
                nonce = nonce.Substring(1, nonce.Length - 2);
                string uri = auth[3].Split('=')[1];
                uri = uri.Substring(1, uri.Length - 2);
                string response = auth[4].Split('=')[1];
                response = response.Substring(1, response.Length - 3);

                //Get user's password from db
                string command = "select password from \"Users\" where username = @user and status = 'A';";
                DBpart oobject = new DBpart();
                DataTable table = oobject.GetDataFromDB(command, "@user", username);
                if (table.Rows.Count > 0 && table.Rows[0].ItemArray.Any())
                {
                    string pass = table.Rows[0].ItemArray[0].ToString();
                    //Check auth
                    string a1 = CalculateMD5Hash(username + ":" + _realm + ":" + pass);
                    string a2 = CalculateMD5Hash(packet[(int)Headers.Method].Split(' ')[0] + ":" + uri);
                    string result = CalculateMD5Hash(a1 + ":" + nonce + ":" + a2);
                    if (result == response)
                    {
                        Console.WriteLine("Register Accepted");


                        //Save location to DB and reply with 200ok ////// I O TUTAJ
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        string received = sender.Address.ToString() + ":" + sender.Port.ToString();
                        double seconds = Double.Parse(packet[(int)Headers.Expires]);
                        DateTime expires = DateTime.Now.AddSeconds(seconds);
                        parameters.Add("@username", username);
                        parameters.Add("@domain", _realm);
                        parameters.Add("@contact", packet[(int)Headers.Contact]);
                        parameters.Add("@received", received);
                        parameters.Add("@expires", expires);
                        parameters.Add("@user_agent", packet[(int)Headers.UserAgent]);

                        command = "insert into \"Registrar_table\" (username, domain, contact, received, expires, user_agent)"
                                  + " select @username, @domain, @contact, @received, @expires, @user_agent where not exists (select from \"Registrar_table\" where username = @username);";
                        oobject.WriteDataToDB(command, parameters);
                        return;
                    }
                }
            }


            string[] reply = new string[20];
            //Method
            reply[(int)Headers.Method] = "SIP/2.0 401 Unathorized\r\n";
            //Via
            reply[(int)Headers.Via] = "Via: " + packet[(int)Headers.Via] + '\n';
            //To
            var random = new Random();
            var tag = String.Format("{0:X6}", random.Next(0xFFFFFFF));
            reply[(int)Headers.To] = "To: " + packet[(int)Headers.To].Remove(packet[(int)Headers.To].Length - 1) + ";tag=" + tag.ToLower() + "\r\n";
            //From
            reply[(int)Headers.From] = "From: " + packet[(int)Headers.From] + '\n';
            //CallId
            reply[(int)Headers.CallId] = "Call-ID: " + packet[(int)Headers.CallId] + '\n';
            //CSeq
            reply[(int)Headers.CSeq] = "CSeq: " + packet[(int)Headers.CSeq] + '\n';
            //WWW-Auth
            random = new Random();
            string newnonce = random.Next().ToString();
            newnonce = CalculateMD5Hash(newnonce);
            reply[(int)Headers.WWWAuthenticate] = "WWW-Authenticate: Digest realm=\"" + _realm + "\", nonce=\"" + newnonce + "\"\r\n";
            //Server
            reply[(int)Headers.Server] = "Server: " + _server + "\r\n";
            //Content length
            reply[(int)Headers.ContentLength] = "Content-Length: 0\r\n";

            string connectedreply = "";
            for (int i = 0; i < reply.Length; i++)
                connectedreply += reply[i];
            connectedreply += "\r\n";
            byte[] bytereply = Encoding.ASCII.GetBytes(connectedreply);
            _socket.Send(bytereply, bytereply.Length, sender);
        }

        public void Stop()
        {
            _socket.Close();
            _stopTask = true;
            Console.WriteLine($"SIP server stopped");
        }
    }
}


