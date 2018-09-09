using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace libMPSSE_Project {
    

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FT_DEVICE_LIST_INFO_NODE {
        public uint Flags;
        public uint Type;
        public uint ID;
        public uint LocId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst =16)]
        public string SerialNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Description;
        public IntPtr ftHandle;
    }

    public enum I2C_SPI_CLOCKRATE : uint {
        I2C_CLOCK_STANDARD_MODE = 100000,                           // 100kb/sec
        I2C_CLOCK_FAST_MODE = 400000,                               // 400kb/sec
        I2C_CLOCK_FAST_MODE_PLUS = 1000000,                         // 1000kb/sec
        I2C_CLOCK_HIGH_SPEED_MODE = 3400000                         // 3.4Mb/sec
    }
    public enum FT_STATUS : uint {
        FT_OK = 0,
        FT_INVALID_HANDLE = 1,
        FT_DEVICE_NOT_FOUND = 2,
        FT_DEVICE_NOT_OPENED = 3,
        FT_IO_ERROR = 4,
        FT_INSUFFICIENT_RESOURCES = 5,
        FT_INVALID_PARAMETER = 6,
        FT_INVALID_BAUD_RATE = 7,
        FT_DEVICE_NOT_OPENED_FOR_ERASE = 8,
        FT_DEVICE_NOT_OPENED_FOR_WRITE = 9,
        FT_FAILED_TO_WRITE_DEVICE = 10,
        FT_EEPROM_READ_FAILED = 11,
        FT_EEPROM_WRITE_FAILED = 12,
        FT_EEPROM_ERASE_FAILED = 13,
        FT_EEPROM_NOT_PRESENT = 14,
        FT_EEPROM_NOT_PROGRAMMED = 15,
        FT_INVALID_ARGS = 16,
        FT_NOT_SUPPORTED = 17,
        FT_OTHER_ERROR = 18

    }
}