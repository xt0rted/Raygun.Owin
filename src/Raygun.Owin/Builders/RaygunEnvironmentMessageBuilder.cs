namespace Raygun.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    using Raygun.Messages;

    public static class RaygunEnvironmentMessageBuilder
    {
        public static RaygunEnvironmentMessage Build()
        {
            var message = new RaygunEnvironmentMessage();

            try
            {
                var now = DateTime.Now;
                message.UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(now).TotalHours;
                message.Locale = CultureInfo.CurrentCulture.DisplayName;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error retrieving time and locale: {0}", ex.Message);
            }

            try
            {
                message.ProcessorCount = Environment.ProcessorCount;
                message.OSVersion = Environment.OSVersion.VersionString;
                message.PackageVersion = Environment.Version.ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error retrieving environment info: {0}", ex.Message);
            }

            try
            {
                //message.TotalVirtualMemory
                //message.AvailableVirtualMemory
                message.DiskSpaceFree = GetDiskSpace();
                //message.TotalPhysicalMemory
                //message.AvailablePhysicalMemory
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error retrieving environment info: {0}", ex.Message);
            }

            try
            {
                message.Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error retrieving environment info: {0}", ex.Message);
            }

            return message;
        }

        private static List<double> GetDiskSpace()
        {
            var results = new List<double>();

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    results.Add(drive.AvailableFreeSpace / 0x40000000); // in GB
                }
            }

            return results;
        }
    }
}