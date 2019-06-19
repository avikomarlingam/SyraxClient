using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;


namespace MinecraftClient.ChatBots
{
    /// <summary>
    /// Example of message receiving.
    /// </summary>

    public class SyraxWalls : ChatBot
    {
        float lastChecked = 0.0f;
        float wallchecktimer = 0.0f;
        int raidtimer = 0;



        public override void GetText(string text)
        {
            
            string message = "";
            string username = "";
            text = GetVerbatim(text);

            string[] split = text.Split(null);
            bool isDotCheck = false;
            bool isWeeWoo = false;
            try{
                isDotCheck = split[4].Equals(".check");
                isWeeWoo = split[4].Equals(".weewoo");
            }
            catch(IndexOutOfRangeException){
                
            }

            if (IsPrivateMessage(text, ref message, ref username))
            {
                if(message.Contains(".check")){
                    
                    if(Settings.wallcheck_players.Contains(username)){
                        
                        BroadcastFactionMessage(username + " marked the walls as clear by pm'ing me .check!");
                        if(Settings.raided){
                            Settings.raided = false;
                            raidtimer = 0;
                            BroadcastFactionMessage("The raid has been marked as ended by " + username);

                        }

                        lastChecked = 0;

                        string checkedJSON = GetWallCheckJson(username, Settings.botname, Settings.iconurl);
                        SendWebReq(Settings.wallchecksurl, checkedJSON);

                        return;

                    }
                }
                else if(message.Contains(".weewoo")){

                    if (Settings.wallcheck_players.Contains(username))
                    {

                        BroadcastFactionMessage(username + " executed .weewoo. WE ARE BEING RAIDED");
                        Settings.raided = true;

                    }
                    return;
                }


            }
            if(isDotCheck){

                if(Settings.wallcheck_players.Contains(split[3])){
                    username = split[1].Remove(0);
                    BroadcastFactionMessage(username + " marked the walls as clear by pm'ing me .check!");
                    if (Settings.raided)
                    {
                        Settings.raided = false;
                        raidtimer = 0;
                        BroadcastFactionMessage("The raid has been marked as ended by " + username);

                    }

                    lastChecked = 0;

                    string checkedJSON = GetWallCheckJson(username, Settings.botname, Settings.iconurl);
                    SendWebReq(Settings.wallchecksurl, checkedJSON);

                    return;
                }

            }
            else if(isWeeWoo){
                if(Settings.wallcheck_players.Contains(split[3])){
                    username = split[1].Remove(0);
                    BroadcastFactionMessage(username + " executed .weewoo. WE ARE BEING RAIDED");
                    Settings.raided = true;

                }
            }
        }

        public override void Update()
        {
            
            lastChecked = lastChecked + 100.0f;
            wallchecktimer = wallchecktimer + 100.0f;


            if(Settings.raided){
                raidtimer = raidtimer + 100;
            }

            if(Settings.raided && raidtimer > 10000){

                string raidJSON = GetRaidedJSON(Settings.botname, Settings.iconurl);
                SendWebReq(Settings.wallchecksurl, raidJSON);

                BroadcastFactionMessage("WE ARE BEING RAIDED!!!!!!!");
                raidtimer = 0;


            }

            if(wallchecktimer > Settings.wallchecktimer){

                BroadcastFactionMessage("Time to check walls @" + Settings.botname + "!!!");
                float foo = lastChecked / 1000.0f / 60.0f;
                decimal bar = Convert.ToDecimal(foo);
                bar = Math.Round(bar, 1);
                BroadcastFactionMessage("It's been " + bar + " minutes since the walls were checked!");
                wallchecktimer = 0;
                string time = "" + bar;

                string wallsJSON = GetWallReminderJson(time, Settings.botname, Settings.iconurl);

                SendWebReq(Settings.wallchecksurl, wallsJSON);


            }


        }

        public void BroadcastFactionMessage(string message)
        {

            SendText("/f c f");
            SendText(message);
            SendText("/f c p");

        }

        public void sendToWebhook(String text){
            WebRequest request = WebRequest.Create(Settings.wallchecksurl);
            request.Method = "POST";  
            byte[] buf;
            buf = Encoding.UTF8.GetBytes("{\"content\" : \"" + text + "\"}");
            request.ContentLength = buf.Length;  

            request.ContentType = "application/json";  

            Stream dataStream = request.GetRequestStream();  
            dataStream.Write(buf, 0, buf.Length);  

            dataStream.Close();  

            WebResponse response = request.GetResponse();  

        }

        public string GetWallReminderJson(string TimeNotChecked, string FactionName, string IconURL)
        {

            String JsonStuff =
                "{" +
                "\"embeds\": [{" +
                "\"title\": \"**Check Walls Now!**\"," +
                "\"description\": \":exclamation: The walls haven't been checked for **__" + TimeNotChecked + "__** minutes!. Check now with ``.check`` ingame\"," +
                "\"footer\": {" +
                "\"icon_url\": \"" + IconURL + "\"," +
                "\"text\": \"" + FactionName + " » Wall Checks\"" +
                "}," +
                "\"color\": \"" + 0xFFFF + "\"" +
                "}]" +
                "}";

            return JsonStuff;

        }

        public string GetWallCheckJson(string name, string FactionName, string IconURL)
        {

            String JsonStuff =
                "{" +
                "\"embeds\": [{" +
                "\"title\": \"**Walls Marked as Clear**\"," +
                "\"description\": \":white_check_mark: **" + name + "** has marked the walls as **clear!**\"," +
                "\"footer\": {" +
                "\"icon_url\": \"" + IconURL + "\"," +
                "\"text\": \"" + FactionName + " » Wall Checks\"" +
                "}," +
                "\"color\": \"" + 0xFFFF + "\"" +
                "}]" +
                "}";

            return JsonStuff;

        }

        public string GetRaidedJSON(string FactionName, string IconURL){
            String JsonStuff =
                "{" +
                "\"content\": \"@everyone\"," +
                "\"embeds\": [{" +
                "\"title\": \"**WEEWOO WE ARE BEING RAIDED**\"," +
                "\"description\": \"GET ON WE ARE BEING **RAIDED**\"," +
                "\"footer\": {" +
                "\"icon_url\": \"" + IconURL + "\"," +
                "\"text\": \"" + FactionName + " » Raid Alerts\"" +
                "}," +
                "\"color\": \"" + 0xF20303 + "\"" +
                "}]" +
                "}";

            return JsonStuff;
        }



        public void SendWebReq(string hookurl, string content)
        {
            WebRequest request = WebRequest.Create(hookurl);
            request.Method = "POST";
            byte[] buf;
            buf = Encoding.UTF8.GetBytes(content);
            request.ContentLength = buf.Length;

            request.ContentType = "application/json";

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(buf, 0, buf.Length);

            dataStream.Close();

            try
            {
                WebResponse response = request.GetResponse();
            }
            catch (WebException)
            {
                LogToConsole("You are being rate limited!");
            }


        }




    }
}
