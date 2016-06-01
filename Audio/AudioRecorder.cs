using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Julas.Utils;
using Julas.Utils.Reactive;
using NAudio.Codecs;
using NAudio.MediaFoundation;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.Compression;

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

        public IAudioStream GetAudioPacketStream(int deviceID, int samplesPerPacket)
        {
            var waveIn = new WaveInEvent
            {
                DeviceNumber = deviceID,
                BufferMilliseconds = 100,
                WaveFormat = new WaveFormat(8000, 16, 1)
            };
            return new AudioPacketStream(waveIn, samplesPerPacket);
        }

        private class AudioPacketStream : IAudioStream
        {
            private readonly WaveInEvent _waveIn;
            private readonly int _samplesPerPacket;
            private readonly Subject<byte[]> _subject;
            private readonly List<byte> _buffer; 

            public IObservable<byte[]> PacketSource => _subject;

            public AudioPacketStream(WaveInEvent waveIn, int samplesPerPacket)
            {
                _buffer = new List<byte>(samplesPerPacket);
                _subject = new Subject<byte[]>();
                _waveIn = waveIn;
                _samplesPerPacket = samplesPerPacket;
                _waveIn.DataAvailable += WaveInOnDataAvailable;
                _waveIn.StartRecording();
            }

            private void WaveInOnDataAvailable(object sender, WaveInEventArgs e)
            {
                for (int i = 0; i < e.BytesRecorded; i+= 2)
                {
                    var pcm16Sample = BitConverter.ToInt16(e.Buffer, i);
                    var alawSample = ALawEncoder.LinearToALawSample(pcm16Sample);
                    _buffer.Add(alawSample);
                    if (_buffer.Count == _samplesPerPacket)
                    {
                        var toSend = _buffer.ToArray();
                        _buffer.Clear();
                        _subject.OnNext(toSend);
                    }
                }
            }

            public void Dispose()
            {
                _waveIn.StopRecording();
                _waveIn.DataAvailable -= WaveInOnDataAvailable;
                _waveIn.Dispose();
                _subject.OnCompleted();
            }
        }
    }
}
