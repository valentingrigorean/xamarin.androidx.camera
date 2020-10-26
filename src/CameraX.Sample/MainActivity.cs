using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Androidx.Camera.Core;
using Androidx.Camera.Lifecycle;
using Androidx.Camera.View;
using AndroidX.Core.Content;
using Java.Lang;
using Xamarin.Essentials;
using Preview = Androidx.Camera.Core.Preview;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Sample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ImageCapture.IOnImageSavedCallback
    {
        private const string TAG = nameof(MainActivity);

        private PreviewView _previewView;

        private Preview _preview;
        private ImageCapture _imageCapture;
        private ICamera _camera;

        private ProcessCameraProvider _cameraProvider;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            _previewView = FindViewById<PreviewView>(Resource.Id.preview_view);

            if (AllPermissionsGranted())
            {
                StartCamera();
            }
            else
            {
                var status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    Toast.MakeText(this,
                        "Permissions not granted by the user.", ToastLength.Short)?.Show();
                }
                else
                {
                    StartCamera();
                }
            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        void ImageCapture.IOnImageSavedCallback.OnError(ImageCaptureException p0)
        {
        }

        void ImageCapture.IOnImageSavedCallback.OnImageSaved(ImageCapture.OutputFileResults p0)
        {
        }

        private bool AllPermissionsGranted()
        {
            return ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted;
        }

        private void StartCamera()
        {
            var cameraProviderFuture = ProcessCameraProvider.GetInstance(this);
            cameraProviderFuture.AddListener(new Runnable(() =>
            {
                _cameraProvider = (ProcessCameraProvider) cameraProviderFuture.Get();

                _previewView.PostDelayed(BindCameraUseCases, 16);
            }), ContextCompat.GetMainExecutor(this));
        }

        private void BindCameraUseCases()
        {
            var metrics = new DisplayMetrics();
            _previewView.Display!.GetRealMetrics(metrics);


            var rotation = GetRotation(_previewView.Display.Rotation);

            var cameraSelector = new CameraSelector.Builder().RequireLensFacing(CameraSelector.LensFacingBack)
                .Build();


            _preview = new Preview.Builder()
                .SetTargetRotation(rotation)
                .Build();

            _imageCapture = new ImageCapture.Builder()
                .SetCaptureMode(ImageCapture.CaptureModeMinimizeLatency)
                .SetTargetRotation(rotation)
                .Build();


            _cameraProvider.UnbindAll();
            try
            {
                _camera = _cameraProvider.BindToLifecycle(this, cameraSelector, _preview, _imageCapture);
                _preview.SetSurfaceProvider(_previewView.SurfaceProvider);
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(TAG, "Use case binding failed", ex);
            }
        }


        private static int GetRotation(SurfaceOrientation rotation)
        {
            switch (rotation)
            {
                case SurfaceOrientation.Rotation0:
                    return 0;
                case SurfaceOrientation.Rotation180:
                    return 180;
                case SurfaceOrientation.Rotation270:
                    return 270;
                case SurfaceOrientation.Rotation90:
                    return 90;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
            }
        }
    }
}