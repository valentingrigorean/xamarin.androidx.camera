using System;
using Android.Runtime;
using Java.Nio;

namespace Sample.Extensions
{
    public static class ByteArrayExtensions
    {
        private static IntPtr _byteBufferClassRef;
        private static IntPtr _byteBufferGetBii;

        public static byte[] ToByteArray(this ByteBuffer self)
        {
            if (_byteBufferClassRef == IntPtr.Zero)
            {
                _byteBufferClassRef = JNIEnv.FindClass("java/nio/ByteBuffer");
            }

            if (_byteBufferGetBii == IntPtr.Zero)
            {
                _byteBufferGetBii = JNIEnv.GetMethodID(_byteBufferClassRef, "get", "([BII)Ljava/nio/ByteBuffer;");
            }

            var dst = new byte[self.Capacity()];
            var dstArray = JNIEnv.NewArray(dst);
            JNIEnv.CallNonvirtualObjectMethod(
                self.Handle,
                _byteBufferClassRef,
                _byteBufferGetBii,
                new JValue(dstArray),
                new JValue(0),
                new JValue(dst.Length));
            JNIEnv.CopyArray(dstArray, dst);
            JNIEnv.DeleteLocalRef(dstArray);

            return dst;
        }
    }
}