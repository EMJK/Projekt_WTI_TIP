using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Julas.Utils;
using Microsoft.AspNet.SignalR.Client;
using ChatClient;
using WebApiClient;
using WebApiClient.Models.Account;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }
    }
}
