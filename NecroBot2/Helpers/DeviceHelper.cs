using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using PoGo.NecroBot.Logic.Forms_Gui.Utils;

namespace NecroBot2.Helpers
{
    public class DeviceHelper
    {
        private readonly string _deviceSourcePath = Directory.GetCurrentDirectory() + @"\Resources\device info.csv";
        public List<DeviceInfo> DeviceBucket;

        public DeviceHelper()
        {
            var deviceInfoStrings = new List<string[]>();
            if (File.Exists(_deviceSourcePath))
            {
                using (var reader = new StreamReader(_deviceSourcePath))
                {
                    string stringLine;
                    while ((stringLine = reader.ReadLine()) != null)
                    {
                        deviceInfoStrings.Add(stringLine.Split(';'));
                    }
                }
                DeviceBucket = deviceInfoStrings.Select(info => new DeviceInfo
                {
                    DeviceId = info[0],
                    AndroidBoardName = info[1],
                    AndroidBootloader = info[2],
                    DeviceBrand = info[3],
                    DeviceModel = info[4],
                    DeviceModelIdentifier = info[5],
                    DeviceModelBoot = info[6],
                    HardwareManufacturer = info[7],
                    HardwareModel = info[8],
                    FirmwareBrand = info[9],
                    FirmwareTags = info[10],
                    FirmwareType = info[11],
                    FirmwareFingerprint = info[12]
                }).ToList();
            }
            //also add the list from Necro
            foreach (var necroSet in DeviceInfoHelper.DeviceInfoSets.Values)
            {
                DeviceBucket.Add(new DeviceInfo
                {
                    DeviceId = necroSet["DeviceId"],
                    AndroidBoardName = necroSet["AndroidBoardName"],
                    AndroidBootloader = necroSet["AndroidBootloader"],
                    DeviceBrand = necroSet["DeviceBrand"],
                    DeviceModel = necroSet["DeviceModel"],
                    DeviceModelIdentifier = necroSet["DeviceModelIdentifier"],
                    DeviceModelBoot = necroSet["DeviceModelBoot"],
                    HardwareManufacturer = necroSet["HardwareManufacturer"],
                    HardwareModel = necroSet["HardwareModel"],
                    FirmwareBrand = necroSet["FirmwareBrand"],
                    FirmwareTags = necroSet["FirmwareTags"],
                    FirmwareType = necroSet["FirmwareType"],
                    FirmwareFingerprint = necroSet["FirmwareFingerprint"]
                });
            }
        }

        public int GetRandomIndex(int max)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            return rand.Next(0, max);
        }

        public string RandomString(int length, string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            var outOfRange = byte.MaxValue + 1 - (byte.MaxValue + 1)%alphabet.Length;

            return string.Concat(
                Enumerable
                    .Repeat(0, int.MaxValue)
                    .Select(e => RandomByte())
                    .Where(randomByte => randomByte < outOfRange)
                    .Take(length)
                    .Select(randomByte => alphabet[randomByte%alphabet.Length])
                );
        }

        private static byte RandomByte()
        {
            using (var randomizationProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[1];
                randomizationProvider.GetBytes(randomBytes);
                return randomBytes.Single();
            }
        }
    }

    public class DeviceInfo
    {
        public string DeviceI { get; set; }
        public string DeviceId { get; set; }
        public string AndroidBoardName { get; set; }
        public string AndroidBootloader { get; set; }
        public string DeviceBrand { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceModelIdentifier { get; set; }
        public string DeviceModelBoot { get; set; }
        public string HardwareManufacturer { get; set; }
        public string HardwareModel { get; set; }
        public string FirmwareBrand { get; set; }
        public string FirmwareTags { get; set; }
        public string FirmwareType { get; set; }
        public string FirmwareFingerprint { get; set; }
    }
}