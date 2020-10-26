using System;
using Android.App;
using Android.Runtime;
using Androidx.Camera.Camera2;
using Androidx.Camera.Core;

namespace Sample
{
    [Application]
    public class DroidApplication : Application, CameraXConfig.IProvider
    {
        public DroidApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public CameraXConfig CameraXConfig { get; } = Camera2Config.DefaultConfig();
    }
}
