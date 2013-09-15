using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Raygun.Messages
{
    public class RaygunEnvironmentMessage
    {
        private List<double> _diskFreeSpace = new List<double>();

        public RaygunEnvironmentMessage()
        {
            UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours;
            Locale = CultureInfo.CurrentCulture.DisplayName;
            ProcessorCount = Environment.ProcessorCount;
            OSVersion = Environment.OSVersion.VersionString;
            Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            PackageVersion = Environment.Version.ToString();

            GetDiskSpace();
        }

        public string PackageVersion { get; private set; }
        public int ProcessorCount { get; private set; }
        public string OSVersion { get; private set; }
        public string Architecture { get; private set; }

        public List<double> DiskSpaceFree
        {
            get { return _diskFreeSpace; }
            set { _diskFreeSpace = value; }
        }

        public double UtcOffset { get; private set; }
        public string Locale { get; private set; }

        private void GetDiskSpace()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    DiskSpaceFree.Add(drive.AvailableFreeSpace / 0x40000000); // in GB
                }
            }
        }
    }
}