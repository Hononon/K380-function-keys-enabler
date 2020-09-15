using HidLibrary;
using System;
using System.Linq;
using System.Threading;

namespace ConsoleAppUsbTest
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                foreach (var k in K380Keyboard.GetConnected())
                {
                    try
                    {
                        k.SetFunctionKeys(true);
                        Console.WriteLine($"Could enable function keys for {k.Device.DevicePath}");
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("Error " + e);
                    }
                }

                Thread.Sleep(10 * 1000);
            }
        }
    }

    class K380Keyboard
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
            {
                r = device.Write(k380_seq_fkeys_on);
            }
            else
            {
                r = device.Write(k380_seq_fkeys_off);
            }

            device.CloseDevice();

            if (!r)
            {
                throw new Exception("Could not active function keys");
            }
        }

        public static K380Keyboard[] GetConnected()
        {
            var devices = HidDevices.Enumerate(HID_VENDOR_ID_LOGITECH, HID_DEVICE_ID_K380).Where(d => d.IsConnected).Select(d => new K380Keyboard(d)).ToArray();
            return devices;
        }

        public HidDevice Device { get; }

        public K380Keyboard(HidDevice device)
        {
            Device = device;
        }

        public void SetFunctionKeys(bool enable)
        {
            SetFunctionKeys(Device, enable);
        }
    }
}
