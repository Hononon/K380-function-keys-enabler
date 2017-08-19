using HidLibrary;
using System;
using System.Threading;

namespace ConsoleAppUsbTest
{
    class Program
    {
        static int HID_VENDOR_ID_LOGITECH = 0x046d;
        static int HID_DEVICE_ID_K380 = 0xb342;


        static byte[] k380_seq_fkeys_on = { 0x10, 0xff, 0x0b, 0x1e, 0x00, 0x00, 0x00 };
        static byte[] k380_seq_fkeys_off = { 0x10, 0xff, 0x0b, 0x1e, 0x01, 0x00, 0x00 };

        static void SetFunctionKeys(HidDevice device, bool enable)
        {
            device.OpenDevice();
            byte[] data;
            device.ReadFeatureData(out data);

            bool r;

            if (enable)
                r = device.Write(k380_seq_fkeys_on);
            else
                r = device.Write(k380_seq_fkeys_off);

            device.CloseDevice();

            if (!r)
                throw new Exception("Could not active function keys");
        }

        static void Main(string[] args)
        {
            var devicePath = "\\\\?\\hid#{00001124-0000-1000-8000-00805f9b34fb}_vid&0002046d_pid&b342&col06#9&3af9a3fe&0&0005#{4d1e55b2-f16f-11cf-88cb-001111000030}";

            //var dev = HidDevices.Enumerate(HID_VENDOR_ID_LOGITECH, HID_DEVICE_ID_K380).First(d => d.DevicePath == devicePath);
            //SetFunctionKeys(dev, true);

            var connected = false;

            while (true)
            {
                Thread.Sleep(10 * 1000);
                if (connected != HidDevices.IsConnected(devicePath))
                {
                    connected = !connected;
                    Console.WriteLine("Changed to " + (connected ? "connected" : "disconnected"));

                    if (connected)
                    {
                        var dev = HidDevices.GetDevice(devicePath);
                        SetFunctionKeys(dev, true);
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
