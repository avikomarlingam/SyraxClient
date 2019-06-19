using System;
using System.Text;
using System.Text.RegularExpressions;
namespace MinecraftClient.ChatBots
{
    public class SyraxSettings : ChatBot
    {

        public override void Initialize()
        {


            LogToConsole("Syrax Settings Loaded");
            string[] dictionary = LoadEntriesFromFile("config.ini");

            string botname = Regex.Split(dictionary[0], @"=")[1];
            string icon_url = Regex.Split(dictionary[1], @"=")[1];
            string fbank_webhook_url = Regex.Split(dictionary[2], @"=")[1];
            string serverchaturl = Regex.Split(dictionary[3], @"=")[1];
            string[] withdrawplayers = Regex.Split(Regex.Split(dictionary[4], @"=")[1], @",");
            string fbankenabled = Regex.Split(dictionary[5], @"=")[1];
            string serverchatenabled = Regex.Split(dictionary[6], @"=")[1];
            string wallchecksenabled = Regex.Split(dictionary[7], @"=")[1];
            int wallchecktimer = 0;
            string[] wallcheckplayers = Regex.Split(Regex.Split(dictionary[10], @"=")[1], @",");
            string wallchecksurl = Regex.Split(dictionary[11], @"=")[1];
            try{
                wallchecktimer = System.Convert.ToInt32(Regex.Split(dictionary[9], @"=")[1]);
            }

            catch(FormatException){
                LogToConsole("Wall Check Timer value is not VALID!");
            }

            Settings.botname = botname;
            Settings.iconurl = icon_url;
            Settings.fbankurl = fbank_webhook_url;
            Settings.serverchaturl = serverchaturl;
            Settings.withdraw_players = withdrawplayers;
            Settings.wallchecktimer = 1000 * wallchecktimer;
            Settings.wallcheck_players = wallcheckplayers;
            Settings.wallchecksurl = wallchecksurl;

            if(fbankenabled.Equals("true")){
                Settings.fbankenabled = true;
            }
            else{
                Settings.fbankenabled = false;
            }

            if (serverchatenabled.Equals("true"))
            {
                Settings.serverchatenabled = true;
            }
            else
            {
                Settings.serverchatenabled = false;
            }


            if (wallchecksenabled.Equals("true"))
            {
                Settings.wallsenabled = true;
            }
            else
            {
                Settings.wallsenabled = false;
            }


            LogToConsole("Faction Bank is enabled? = " + Settings.fbankenabled);
            LogToConsole("Server Chat to Discord is enabled? = " + Settings.serverchatenabled);
            LogToConsole("Wall checks is enabled? = " + Settings.serverchatenabled);

        }


    }
}
