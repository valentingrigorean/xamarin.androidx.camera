using System;
using System.Collections.Generic;
using System.Linq;
using Androidx.Camera.Core;
using Sample.Extensions;

namespace Sample
{
    public class LuminosityAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
    {
        private readonly ILumaListener _lumaListener;

        private readonly Queue<DateTime> _frameTimestamps = new Queue<DateTime>();

        private int _frameRateWindow = 8;
        private double _framesPerSecond = -1f;

        public LuminosityAnalyzer(ILumaListener lumaListener)
        {
            _lumaListener = lumaListener;
        }
            

        public void Analyze(IImageProxy image)
        {
            var currentTime = DateTime.Now;
            _frameTimestamps.Enqueue(currentTime);
            while (_frameTimestamps.Count >= _frameRateWindow)
            {
                _frameTimestamps.Dequeue();
            }

            var timestampFirst = _frameTimestamps.First();
            var timestampLast = _frameTimestamps.Dequeue();

            var framePerSecond = 1.0 / ((timestampFirst - timestampLast).TotalMilliseconds) * 1000f;

            var buffer = image.GetPlanes()[0].Buffer;

            var data = buffer.ToByteArray();

            var pixels = data!.Select(b => b & 0xFf)
                .ToArray();

            var luma = pixels.Average();
            _lumaListener.OnFrameAnalyzed(luma);
            image.Close();
        }


        public interface ILumaListener
        {
            void OnFrameAnalyzed(double luma);
        }
    }
}