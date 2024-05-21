using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Accelib.Utility
{
    public static class ByteHandler
    {
        public static byte[] StructToBytes(object obj)
        {
            var size = Marshal.SizeOf(obj);
            var arr = new byte[size];
            var ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(size);

                Marshal.StructureToPtr(obj, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            
            return arr;
        }
        
        public static T BytesToStruct<T>(byte[] buffer) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(buffer, 0, ptr, size);
                return (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return default;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}