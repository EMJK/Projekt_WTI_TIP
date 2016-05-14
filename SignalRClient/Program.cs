using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Audio;
using Microsoft.AspNet.SignalR.Client;
using RegistrarChatApiClient;
using RegistrarWebApiClient;
using RegistrarWebApiClient.Models.Account;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var recorder = new AudioRecorder();
            var player = new AudioPlayer();

            var rd = recorder.GetDevices().ToArray();
            var pd = player.GetDevices().ToArray();

            var stream = recorder.GetAudioPacketStream(0);
            player.PlayAudioPacketStream(0, stream);
            Console.ReadLine();
            return;
            Application.Run(new MainForm());
        }
    }
}
