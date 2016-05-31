using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Julas.Utils;
using Julas.Utils.Reactive;
using NAudio.MediaFoundation;
using NAudio.Wave;

namespace Audio
{
    public class AudioRecorder
    {
        public IEnumerable<DeviceInfo> GetDevices()
        {
            return WaveIn.DeviceCount
                .Map(x => Enumerable.Range(0, x))
                .Select(WaveIn.GetCapabilities)
                .Select((x, i) => new DeviceInfo()
                {
                    ID = i,
                    Name = x.ProductName
                });
        }

        public IAudioStream GetAudioPacketStream(int deviceID)
        {
            var waveIn = new WaveIn();
            waveIn.BufferMilliseconds = 100;
            waveIn.WaveFormat = Utils.GetRawFormat();
            return new AudioPacketStream(waveIn);
        }

        private class AudioPacketStream : IAudioStream
        {
            private readonly Stream _rawAudioStream;
            private readonly WaveIn _waveIn;
            private readonly Subject<IList<byte>> _subject;
            private CancellationTokenSource _source;

            public IObservable<IList<byte>> PacketSource => _subject;
            

            public AudioPacketStream(WaveIn waveIn)
            {
                _subject = new Subject<IList<byte>>();
                _source = new CancellationTokenSource();
                var provider = new WaveInProvider(waveIn);
                Task.Factory.StartNew(() =>
                {
                    byte[] buf = 
                });
            }

            private void WaveInOnDataAvailable(object sender, WaveInEventArgs e)
            {
                _rawAudioStream.Write(e.Buffer, 0, e.BytesRecorded);
            }

            private async Task TaskLoop(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    if (_device.AvailableSamples < 160 || !_device.IsRunning)
                    {
                        await Task.Delay(10);
                    }
                    else
                    {
                        var buffer = new byte[320];
                        _device.ReadSamples(buffer,160);
                        var encoded = new byte[160];
                        for (int i = 0; i < 160; i++)
                        {
                            var sample = BitConverter.ToInt16(buffer, i*2);
                            encoded[i] = Pcma.Encode(sample);
                        }
                        _packetSubject.OnNext(encoded);
                    }
                }
            }

            public void Dispose()
            {
                _device.Stop();
                _source.Cancel();
            }

            
        }
    }
}
