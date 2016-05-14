using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Audio
{
    static class NAudioExtensions
    {
        public static WaveStream GetStream(this IWaveIn waveIn)
        {
            var rawData = new MemoryStream();
            var baseWaveStream = new RawSourceWaveStream(rawData, waveIn.WaveFormat);
            var dataSource = Observable.FromEventPattern<WaveInEventArgs>(
                action => waveIn.DataAvailable += action,
                action => waveIn.DataAvailable -= action);
            var subscription = dataSource.Subscribe(
                args => rawData.Write(args.EventArgs.Buffer, 0, args.EventArgs.BytesRecorded));
            var convertingStream = new CustomWaveFormatConversionStream(WaveFormat.CreateALawFormat(8000, 1), baseWaveStream);
            convertingStream.AfterDispose = () =>
            {
                subscription.Dispose();
                baseWaveStream.Dispose();
                rawData.Dispose();
            };

            return convertingStream;
        }

        private class CustomWaveFormatConversionStream : WaveFormatConversionStream
        {
            public Action BeforeDispose { get; set; }
            public Action AfterDispose { get; set; }

            public CustomWaveFormatConversionStream(WaveFormat targetFormat, WaveStream sourceStream) : base(targetFormat, sourceStream)
            {
            }

            protected override void Dispose(bool disposing)
            {
                BeforeDispose();
                base.Dispose(disposing);
                AfterDispose();
            }
        }
    }
}
