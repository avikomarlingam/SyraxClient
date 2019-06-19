using System;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
namespace MinecraftClient.ChatBots
{
    public class SyraxBot : ChatBot
    {

        string[] dictionary = new string[0];
        string[] withdraw_players = new string[0];

        string hook_url = "";
        string server_chat_url = "";
        string icon_url = "";
        string faction_name = "";

        public override void Initialize()
        {
            dictionary = LoadEntriesFromFile("config.ini");
            hook_url = Regex.Split(dictionary[2], @"=")[1];
            server_chat_url = Regex.Split(dictionary[3], @"=")[1];
            withdraw_players = Regex.Split(Regex.Split(dictionary[4], @"=")[1], @",");
            faction_name = Regex.Split(dictionary[0], @"=")[1];
            icon_url = Regex.Split(dictionary[1], @"=")[1];

            LogToConsole("Bot name : " + Settings.botname);


        }


        public override void GetText(string text)
        {
            text = GetVerbatim(text);
            string[] split = text.Split(null);


            string message = "";
            string username = "";

            if (IsPrivateMessage(text, ref message, ref username))
            {

                string[] splitted = message.Split(null);


                if (splitted[0].Equals("w"))
                {

                    if (splitted.Length != 2)
                    {
                        SendText("/pm " + username + " args: /pm <Bot> w <amount>");
                        return;
                    }

                    try
                    {

                        int amount = System.Convert.ToInt32(splitted[1]);
                        //wbank hookurl message name iconurl
                        if (withdraw_players.Contains(username))
                        {

                            SendText("/pay " + username + " " + amount);
                            BroadcastFactionMessage(username.ToUpper() + " WITHDREW $" + amount + ".00");

                            string jsonData = GetWithdrawedJson(username, splitted[1], Settings.botname, Settings.iconurl);
                            SendWebReq(Settings.fbankurl, jsonData);

                        }
                        else
                        {
                            SendPrivateMessage(username, "You don't have the required perms to withdraw");
                        }
                    }
                    catch (FormatException)
                    {
                        SendPrivateMessage(username, "Please provide a valid value to withdraw");
                    }


                }
            }

            if (split.Length == 6)
            {
                if (split[3].Equals("received"))
                {
                    LogToConsole("You were paid " + split[0]);

                    string name = split[5];
                    name = name.Remove(name.Length - 1);
                    text = text.Remove(text.Length - 1);
                    BroadcastFactionMessage(name.ToUpper() + " DEPOSITED " + split[0] + ".00");

                    string json = GetDepositedJson(name, split[0], Settings.botname, Settings.iconurl);
                    SendWebReq(Settings.fbankurl, json);


                }
            }
        }

        public void BroadcastFactionMessage(string message)
        {

            SendText("/f c f");
            SendText(message);
            SendText("/f c p");

        }

        public string GetWithdrawedJson(string Name, string Amount, string FactionName, string IconURL)
        {

            String JsonStuff =
                "{" +
                "\"embeds\": [{" +
                "\"title\": \"**Money Withdrawn**\"," +
                "\"description\": \"**:arrow_up: " + Name + "** withdrew **" + Amount + "**\"," +
                "\"footer\": {" +
                "\"icon_url\": \"" + IconURL + "\"," +
                "\"text\": \"" + FactionName + " | Faction Bank\"" +
                "}," +
                "\"color\": \"" + 0xFFFF + "\"" +
                "}]" +
                "}";

            return JsonStuff;

        }

        public string GetDepositedJson(string Name, string Amount, string FactionName, string IconURL)
        {

            String JsonStuff =
                "{" +
                "\"embeds\": [{" +
                "\"title\": \"**Money Deposited**\"," +
                "\"description\": \"**:arrow_down: " + Name + "** deposited **" + Amount + "**\"," +
                "\"footer\": {" +
                "\"icon_url\": \"" + IconURL + "\"," +
                "\"text\": \"" + FactionName + " | Faction Bank\"" +
                "}," +
                "\"color\": \"" + 0xFFFF + "\"" +
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

            try{
                WebResponse response = request.GetResponse();
            }
            catch(WebException){
                LogToConsole("You are being rate limited!");
            }

           
        }




    }
}