using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Julas.Utils;
using NAudio.Codecs;
using NAudio.Wave;

namespace Audio
{
    public class AudioPlayer
    {
        public IEnumerable<DeviceInfo> GetDevices()
        {
            return WaveOut.DeviceCount
                .Map(x => Enumerable.Range(0, x))
                .Select(WaveOut.GetCapabilities)
                .Select((x, i) => new DeviceInfo()
                {
                    ID = i,
                    Name = x.ProductName
                });
        }

        public IDisposable PlayAudioPacketStream(int deviceID, IObservable<byte[]> packetSource)
        {
            var waveOut = new WaveOutEvent()
            {
                DeviceNumber = deviceID,
                DesiredLatency = 100
            };

            var provider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            var subscription = packetSource.Subscribe(data =>
            {
                for (int i = 0; i < data.Length; i++)
                {
                    var pcmSample = BitConverter.GetBytes(ALawDecoder.ALawToLinearSample(data[i]));
                    provider.AddSamples(pcmSample, 0, pcmSample.Length);
                }
            });
            waveOut.Init(provider);
            waveOut.Play();
            return Disposable.Create(() =>
            {
                waveOut.Stop();
                subscription.Dispose();
                waveOut.Dispose();
            });
        }
    }
}