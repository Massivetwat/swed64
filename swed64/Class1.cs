using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



namespace swed64
{
    public class swed
    {

        #region imports

        [DllImport("Kernel32.dll")]

        static extern bool ReadProcessMemory(
           IntPtr hProcess,
           long lpBaseAddress,
           [Out] byte[] lpBuffer,
           int nSize,
           IntPtr lpNumberOfBytesRead
           );

        [DllImport("kernel32.dll")]

        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            long lpBaseAddress,
            byte[] lpBuffer,
            int size,
            IntPtr lpNumberOfBytesWritten
            );




        #endregion


        #region globals


        public static Process proc; 


        #endregion


        public Process GetProcess(string procname)
        {
            proc = Process.GetProcessesByName(procname)[0];
            return proc;
        }
        public IntPtr GetModuleBase(string modulename)
        {
            if (modulename.Contains(".exe"))
                return proc.MainModule.BaseAddress; // check if 0 before doing any mem read/write

            foreach (ProcessModule module in proc.Modules)
            {
                if (module.ModuleName == modulename)
                    return module.BaseAddress;
            }
            return IntPtr.Zero;
        }


        // Main methods 
        public long ReadPointer(long addy)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(proc.Handle, addy, buffer, buffer.Length, IntPtr.Zero);
            return BitConverter.ToInt64(buffer);
        }
        public long ReadPointer(long addy, long offset)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero);
            return BitConverter.ToInt64(buffer);
        }

        public static byte[] ReadBytes(long addy, long bytes)
        {
            byte[] buffer = new byte[bytes];
            ReadProcessMemory(proc.Handle, addy, buffer, buffer.Length, IntPtr.Zero);
            return buffer;
        }

        public byte[] ReadBytes(long addy, long offset, long bytes)
        {
            byte[] buffer = new byte[bytes];
            ReadProcessMemory(proc.Handle, addy + offset, buffer, buffer.Length, IntPtr.Zero);
            return buffer;
        }

        public bool WriteBytes(long address, byte[] newbytes)
        {
            return WriteProcessMemory(proc.Handle, address, newbytes, newbytes.Length, IntPtr.Zero);
        }
        public bool WriteBytes(long address, long offset, byte[] newbytes)
        {
            return WriteProcessMemory(proc.Handle, address + offset, newbytes, newbytes.Length, IntPtr.Zero);
        }

        // different return types 
        public int ReadInt(long address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4));
        }
        public int ReadInt(long address, long offset)
        {
            return BitConverter.ToInt32(ReadBytes(address + offset, 4));
        }

        public long ReadLong(long address)
        {
            return BitConverter.ToInt64(ReadBytes(address, 8));
        }
        public long ReadLong(long address, long offset)
        {
            return BitConverter.ToInt64(ReadBytes(address + offset, 8));
        }

        public float ReadFloat(long address)
        {
            return BitConverter.ToSingle(ReadBytes(address, 4));
        }
        public float ReadFloat(long address, long offset)
        {
            return BitConverter.ToSingle(ReadBytes(address + offset, 4));
        }

        public double ReadDouble(long address)
        {
            return BitConverter.ToDouble(ReadBytes(address, 8));
        }
        public double ReadDouble(long address, long offset)
        {
            return BitConverter.ToDouble(ReadBytes(address + offset, 4));
        }


        public Vector3 ReadVec(long address)
        {
            var bytes = ReadBytes(address, 12);
            return new Vector3
            {
                X = BitConverter.ToSingle(bytes, 0),
                Y = BitConverter.ToSingle(bytes, 4),
                Z = BitConverter.ToSingle(bytes, 8)
            };
        }

        public Vector3 ReadVec(long address, long offset)
        {
            var bytes = ReadBytes(address + offset, 12);
            return new Vector3
            {
                X = BitConverter.ToSingle(bytes, 0),
                Y = BitConverter.ToSingle(bytes, 4),
                Z = BitConverter.ToSingle(bytes, 8)
            };
        }

        public float[] ReadMatrix(long address)
        {
            var bytes = ReadBytes(address, 4 * 16);
            var matrix = new float[bytes.Length];

            matrix[0] = BitConverter.ToSingle(bytes, 0 * 4);
            matrix[1] = BitConverter.ToSingle(bytes, 1 * 4);
            matrix[2] = BitConverter.ToSingle(bytes, 2 * 4);
            matrix[3] = BitConverter.ToSingle(bytes, 3 * 4);

            matrix[4] = BitConverter.ToSingle(bytes, 4 * 4);
            matrix[5] = BitConverter.ToSingle(bytes, 5 * 4);
            matrix[6] = BitConverter.ToSingle(bytes, 6 * 4);
            matrix[7] = BitConverter.ToSingle(bytes, 7 * 4);

            matrix[8] = BitConverter.ToSingle(bytes, 8 * 4);
            matrix[9] = BitConverter.ToSingle(bytes, 9 * 4);
            matrix[10] = BitConverter.ToSingle(bytes, 10 * 4);
            matrix[11] = BitConverter.ToSingle(bytes, 11 * 4);

            matrix[12] = BitConverter.ToSingle(bytes, 12 * 4);
            matrix[13] = BitConverter.ToSingle(bytes, 13 * 4);
            matrix[14] = BitConverter.ToSingle(bytes, 14 * 4);
            matrix[15] = BitConverter.ToSingle(bytes, 15 * 4);

            return matrix;

        }
    }
}