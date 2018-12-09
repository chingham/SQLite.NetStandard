using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SQLite.NetStandard.iOS {
    internal static class Extensions {
        static readonly Encoding encoding = Encoding.UTF8;
        
        public static byte[] ToUTF8Bytes(this string str) {
            if (str == null)
                return null;

            var length = encoding.GetByteCount(str) + 1;

            var bytes = new byte[length];
            encoding.GetBytes(str, 0, str.Length, bytes, 0);
            return bytes;
        }

        public static unsafe void UsingUTF8Pointer(this string str, Action<IntPtr, int> action) {
            var bytes = ToUTF8Bytes(str);
            if (bytes == null || bytes.Length == 0) {
                action?.Invoke(IntPtr.Zero, 0);
                return;
            }

            fixed (byte* ptr = bytes) {
                action?.Invoke((IntPtr)ptr, bytes.Length);
            }
        }

        static int GetNativeUTF8Size(IntPtr ptr) {
            if (ptr == IntPtr.Zero)
                return 0;

            var offset = 0;
            while (Marshal.ReadByte(ptr, offset) > 0)
                offset++;

            return offset + 1;
        }

        internal static string ToUTF8String(this IntPtr ptr) {
            if (ptr == IntPtr.Zero)
                return null;
            
            var size = GetNativeUTF8Size(ptr);
            var array = new byte[size - 1];
            Marshal.Copy(ptr, array, 0, size - 1);
            return Encoding.UTF8.GetString(array, 0, array.Length);
        }
    }
}