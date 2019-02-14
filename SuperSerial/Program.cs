using Microsoft.Win32;
using MsrLib;
using System;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace SuperSerial
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] parameters)
        {
            int cancel = 10;
            foreach(string param in parameters)
            {
                cancel = int.Parse(param);
                break;
            }
            Console.WriteLine("SuperSerial: " + Assembly.GetEntryAssembly().GetName().Version);
            Console.WriteLine("Initializing...");
            MsrLibKernel.AutoAttachNewDevices = true;
            MsrLibKernel.Initialize();
            MsrDeviceLane lane = MsrLibKernel.GetLane(0, true);
            lane.AutoAttachNewDevices = true;
            lane.DeviceAttached += Lane_DeviceAttached;
            Console.WriteLine("Looking for a device... ");
            MsrLibKernel.FindDeviceAsync(true);
            while (cancel > 0)
            {
                Console.WriteLine(cancel--);
                Thread.Sleep(1000);
            }
            try
            {
                RegistryKey regKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).CreateSubKey(@"SOFTWARE\SuperSerial", true);
                regKey32.SetValue("AppProduct", "");
                regKey32.SetValue("AppSerialNumber", "");
                regKey32.SetValue("AppSerialNumber2", "");
                regKey32.Close();
            }
            catch (Exception) { }
            try
            {
                RegistryKey regKey64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey(@"SOFTWARE\SuperSerial", true);
                regKey64.SetValue("AppProduct", "");
                regKey64.SetValue("AppSerialNumber", "");
                regKey64.SetValue("AppSerialNumber2", "");
                regKey64.Close();
            }
            catch (Exception) { }
        }

        private static void Lane_DeviceAttached(object sender, EventArgs e)
        {
            Console.WriteLine("Found a device! Reading Data...");
            MsrDeviceLane lane = (MsrDeviceLane)sender;
            Console.WriteLine("AppProduct: " + lane.Device.Properties.AppProduct);
            Console.WriteLine("AppSerialNumber: " + lane.Device.Properties.AppSerialNumber);
            Console.WriteLine("AppSerialNumber2: " + lane.Device.Properties.AppSerialNumber2);
            try
            {
                RegistryKey regKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).CreateSubKey(@"SOFTWARE\SuperSerial", true);
                regKey32.SetValue("AppProduct", lane.Device.Properties.AppProduct);
                regKey32.SetValue("AppSerialNumber", lane.Device.Properties.AppSerialNumber);
                regKey32.SetValue("AppSerialNumber2", lane.Device.Properties.AppSerialNumber2);
                regKey32.Close();
            }
            catch (Exception) { }
            try
            {
                RegistryKey regKey64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey(@"SOFTWARE\SuperSerial", true);
                regKey64.SetValue("AppProduct", lane.Device.Properties.AppProduct);
                regKey64.SetValue("AppSerialNumber", lane.Device.Properties.AppSerialNumber);
                regKey64.SetValue("AppSerialNumber2", lane.Device.Properties.AppSerialNumber2);
                regKey64.Close();
            }
            catch (Exception) { }
            Process.GetCurrentProcess().Kill();
        }
    }
}
