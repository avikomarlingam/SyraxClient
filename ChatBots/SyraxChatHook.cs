using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MinecraftClient.ChatBots
{
    /// <summary>
    /// Example of message receiving.
    /// </summary>

    public class SyraxChatHook : ChatBot
    {
        string[] dictionary = new string[0];

        string public_chat_url = "";

        public override void Initialize()
        {

            dictionary = LoadEntriesFromFile("config.ini");

            public_chat_url = Regex.Split(dictionary[3], @"=")[1];


        }

        public override void GetText(string text)
        {

            text = GetVerbatim(text);

            if (text.Length > 1)
            {

                string jsonStuff = "{\"content\" : \"``" + text + "``\"}";
                SendWebReq(Settings.serverchaturl, jsonStuff);

            }


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
