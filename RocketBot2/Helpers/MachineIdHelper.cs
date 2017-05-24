using System;
using System.Management;

namespace RocketBot2.Helpers
{
    public class MachineIdHelper
    {
        public static string GetCpuId()
        {
            var cpuInfo = string.Empty;
            var mc = new ManagementClass("win32_processor");
            var moc = mc.GetInstances();

            foreach (var o in moc)
            {
                var mo = (ManagementObject) o;
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }
            return cpuInfo;
        }

        public static string GetHardDriveId()
        {
            var drive = "C";
            var dsk = new ManagementObject(
                @"win32_logicaldisk.deviceid=""" + drive + @":""");
            dsk.Get();
            return dsk["VolumeSerialNumber"].ToString();
        }

        public static string GetMachineId()
        {
            string id;
            try
            {
                id = GetCpuId() + GetHardDriveId();
            }
            catch (Exception)
            {
                id = "BF00LIKELYVIRTUALMACHINE";
            }
            return id;
        }
    }
}