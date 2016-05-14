using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Julas.Utils;
using Julas.Utils.Reactive;
using NAudio.Wave;

namespace Audio
{
    public class AudioRecorder
    {
        public IEnumerable<DeviceInfo> GetDevices()
        {
            foreach (var id in Enumerable.Range(0, WaveIn.DeviceCount))
            {
                var info = WaveIn.GetCapabilities(id);
                yield return new DeviceInfo()
                {
                    Name = info.ProductName,
                    ID = id
                };
            }
        }

        public IAudioStream GetAudioPacketStream(int deviceID)
        {
            var waveIn = new WaveInEvent();
            waveIn.BufferMilliseconds = 100;
            waveIn.DeviceNumber = deviceID;
            waveIn.NumberOfBuffers = 2;
            var stream = waveIn.GetStream();

            return new AudioPacketStream(stream, 160, TimeSpan.FromSeconds(1));
        }

        private class AudioPacketStream : IAudioStream
        {
            private readonly Stream _audioStream;
            private readonly IObjectReader<IList<byte>> _reader;
            public IObservable<IList<byte>> PacketSource { get; }

            public AudioPacketStream(Stream stream, int bytesPerPacket, TimeSpan delay = default(TimeSpan))
            {
                _audioStream = stream;
                _reader = ObjectReader.Create<IList<byte>>(stream, new Func<Stream, Task<Option<IList<byte>>>>(
                    async str =>
                    {
                        var buffer = new byte[bytesPerPacket];
                        var bytesRead = await str.ReadAsync(buffer, 0, buffer.Length);
                        return Option.Some<IList<byte>>(bytesRead == buffer.Length ? buffer : buffer.Take(bytesRead).ToArray());
                    }));
                PacketSource = _reader.ToObservable();
                if (delay > TimeSpan.Zero)
                {
                    PacketSource = PacketSource.Delay(delay);
                }
            }
            
            public void Dispose()
            {
                _reader.Dispose();
                _audioStream.Dispose();
            }
        }
    }
}
