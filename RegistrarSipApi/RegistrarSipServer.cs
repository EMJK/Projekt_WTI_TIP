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
using Ozeki.Network;
using Ozeki.VoIP;

namespace RegistrarSipApi
{
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

            try
            {
                sqlc.Connection = this.connection; // connection to DB
                dr = sqlc.ExecuteReader(); // query execution and creation of dr pointer
                dt.Load(dr); //load data to dataTable object
                connection.Close();
                return dt; // return data
            }
            catch
            { return null; }
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

    public class SipServer : PBXBase
    {
        Dictionary<string, NpgsqlTypes.NpgsqlDateTime> CallStart = new Dictionary<string, NpgsqlTypes.NpgsqlDateTime>();

        string _localAddress;
        DBpart DBobject;

        public SipServer(string localAddress, int minPortRange, int maxPortRange)
            : base(minPortRange, maxPortRange)
        {
            _localAddress = localAddress;
            Console.WriteLine("SipServer starting...");
            Console.WriteLine("Local address: " + localAddress);

            DBobject = new DBpart();
        }

        protected override void OnStart()
        {
            Console.WriteLine("SipServer started.");
            SetListenPort(_localAddress, 5060, Ozeki.Network.TransportType.Udp);

            Console.WriteLine("Listened port: 5060(UDP)");

            base.OnStart();
        }

        protected override AuthenticationResult OnAuthenticationRequest(ISIPExtension extension, RequestAuthenticationInfo authInfo)
        {
            Console.WriteLine("Authentication request received from: " + extension.ExtensionID);

            AuthenticationResult result = new AuthenticationResult();

            string user = extension.ExtensionID;
            //Get user's password from db
            string command = "select password_hash from users where username = @user and status = 'A';";
            DataTable table = DBobject.GetDataFromDB(command, "@user", user);
            if (table.Rows != null && table.Rows.Count > 0 && table.Rows[0].ItemArray.Any())
            {
                string pass = table.Rows[0].ItemArray[0].ToString();
                //UserInfo userInfo;
                result = extension.CheckPassword(user, pass, authInfo);
            }
            else
            {
                Console.WriteLine("Cannot find extension. UserName: " + extension.ExtensionID);
            }

            if (result != null && result.AuthenticationAccepted)
            {
                Console.WriteLine("Authentication accepted. UserName: " + extension.ExtensionID);
            }
            else
            {
                Console.WriteLine("Authentication denied. UserName: " + extension.ExtensionID);
            }

            return result;
        }

        protected override RegisterResult OnRegisterReceived(ISIPExtension extension, SIPAddress from, int expires)
        {

            Console.WriteLine("Register received from: " + extension.ExtensionID);
            //Save location to DB
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string received = "sip:" + extension.InstanceInfo.Transport.RemoteEndPoint.Address.ToString() + ":" + extension.InstanceInfo.Transport.RemoteEndPoint.Port.ToString();
            double seconds = expires;
            DateTime expiredate = DateTime.Now.AddSeconds(seconds);
            parameters.Add("@username", extension.AuthName);
            parameters.Add("@domain", from.Address);
            parameters.Add("@contact", extension.InstanceInfo.Contact.ToString());
            parameters.Add("@received", received);
            parameters.Add("@expires", expiredate);

            string command = "UPDATE registrar_table  SET domain = @domain, contact = @contact, received = @received, expires = @expires WHERE username = @username;"
                      + "insert into registrar_table (username, domain, contact, received, expires)"
                      + " select @username, @domain, @contact, @received, @expires where not exists (select from registrar_table where username = @username);";
            DBobject.WriteDataToDB(command, parameters);
            return base.OnRegisterReceived(extension, from, expires);
        }

        protected override void OnUnregisterReceived(ISIPExtension extension)
        {
            Console.WriteLine("Unregister received from: " + extension.ExtensionID);
            base.OnUnregisterReceived(extension);
        }

        protected override void OnCallRequestReceived(ISessionCall call)
        {
            Console.WriteLine("Call request received. Caller: " + call.DialInfo.CallerID + " callee: " + call.DialInfo.Dialed);
            call.CallStateChanged += Call_CallStateChanged;
            base.OnCallRequestReceived(call);
        }

        private void Call_CallStateChanged(object sender, CallStateChangedArgs e)
        {
            SIPCall call1 = (SIPCall)sender;
            if (e.State == CallState.Answered)
            {
                CallStart.Add(call1.CallID, NpgsqlTypes.NpgsqlDateTime.Now);
            }

            if (e.State == CallState.Completed)
            {
                NpgsqlTypes.NpgsqlDateTime startValue;
                if (CallStart.TryGetValue(call1.CallID, out startValue))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("@calling_user_id", call1.CallerIDAsCaller);
                    parameters.Add("@called_user_id", call1.DialInfo.Dialed);
                    parameters.Add("@source_ip", call1.BasicInfo.Owner.InstanceInfo.Transport.RemoteEndPoint.ToString());
                    parameters.Add("@start_billing", startValue);
                    parameters.Add("@stop_billing", NpgsqlTypes.NpgsqlDateTime.Now);
                    parameters.Add("@call_id", call1.CallID);
                    string command = "insert into billing (calling_user_id, called_user_id, source_ip, start_billing, stop_billing, call_id)"
                                 + "values(@calling_user_id, @called_user_id, @source_ip, @start_billing, @stop_billing, @call_id)";

                    DBobject.WriteDataToDB(command, parameters);

                    CallStart.Remove(call1.CallID);
                }
            }            
        }
    }
}
