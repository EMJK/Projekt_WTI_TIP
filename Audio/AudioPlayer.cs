using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NAudio.Wave;

namespace Audio
{
    public class AudioPlayer
    {
        public IEnumerable<DeviceInfo> GetDevices()
        {
            foreach (var id in Enumerable.Range(0, WaveOut.DeviceCount))
            {
                var info = WaveOut.GetCapabilities(id);
                yield return new DeviceInfo()
                {
                    Name = info.ProductName,
                    ID = id
                };
            }
        }

        public IDisposable PlayAudioPacketStream(int deviceID, IAudioStream stream)
        {
            var waveOut = new WaveOutEvent();
            waveOut.DesiredLatency = 50;
            waveOut.DeviceNumber = deviceID;
            waveOut.NumberOfBuffers = 2;
            var waveProvider = new ObservableWaveProvider(stream);
            var cleanup = Disposable.Create(() =>
            {
                waveOut.Stop();
                waveOut.Dispose();
            });
            waveOut.Init(waveProvider);
            
            return Disposable.Create(() =>
            {
                waveOut.Dispose();
                cleanup.Dispose();
            });
        }

        private Stream ObservableToStream<T>(IObservable<T> observable) where T : IList<byte>
        {
            var stream = new MemoryStreamFromObservable();
            var subscription = observable.Subscribe(
                packet => stream.Write(packet.ToArray(), 0, packet.Count));
            stream.BeforeDispose = subscription.Dispose;
            return stream;
        }

        private class MemoryStreamFromObservable : MemoryStream
        {
            public Action BeforeDispose { get; set; }

            protected override void Dispose(bool disposing)
            {
                BeforeDispose?.Invoke();
                base.Dispose(disposing);
            }
        }

        private class ObservableWaveProvider : IWaveProvider
        {
            private readonly IAudioStream _stream;
            private IEnumerable<byte> _byteSource;
            private IEnumerator<byte> _enumerator;
            public WaveFormat WaveFormat { get; }

            public ObservableWaveProvider(IAudioStream stream)
            {
                _stream = stream;
                WaveFormat = WaveFormat.CreateALawFormat(8000, 1);
                _byteSource = stream.PacketSource.ToEnumerable().SelectMany(x => x);
                _enumerator = _byteSource.GetEnumerator();
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                int i = 0;
                while(i < count && _enumerator.MoveNext())
                {
                    buffer[i + offset] = _enumerator.Current;
                }
                return i;
            }
        }
    }
}