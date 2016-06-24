﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolCSharp_CONSOLE
{
    class Program
    {
        // STEAM API KEY: 3CB2326C319D80429445D852BDCC01C4
        // ResolveVanityURL: http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=3CB2326C319D80429445D852BDCC01C4&vanityurl=USERVANITYNAME

        static string metaLocation = AppDomain.CurrentDomain.BaseDirectory;
        static string APIKey = "3CB2326C319D80429445D852BDCC01C4";
        static string ResolveVanityURL = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=3CB2326C319D80429445D852BDCC01C4&format=xml&vanityurl=";

        static void Main(string[] args)
        {
            //string s;
            //string s2;

            //string[] terms = { "foo", "bar" };

            //s = getElement("C:\\Users\\Nicky\\Documents\\testXML.xml", false, "foo");
            //Console.WriteLine(s);

            //s2 = getElement("C:\\Users\\Nicky\\Documents\\testXML.xml", terms);
            //Console.WriteLine(s2);

            getAPIData(ResolveVanityURL + "metherul", metaLocation + "temp.xml"); //Grabs the XML file that contains the user's steam numeric ID.
            //Process.Start("notepad", metaLocation + "temp.xml");
            string s = getElement(metaLocation + "temp.xml", false, "steamid"); //steamid is actually inside the response tag, but its all one line so it doesn't matter anyway.
            Console.WriteLine(s); //Output

        }

        public static string getElement(string xmlPath, bool isString, string element) //isString is here in case you want to get an element from a string.
        {
            StringBuilder sb;

            string openingTag;
            string closingTag;

            bool readLines;

            string[] apiData = null; // HERE

            if (isString == false)
                apiData = File.ReadAllLines(xmlPath);

            openingTag = "<" + element + ">";
            closingTag = "</" + element + ">";
            sb = new StringBuilder();
            readLines = false;

            // Then run through each line and search for the tags I want
            foreach (string line in apiData)
            {
                if ((readLines == true))
                {
                    sb.Append(line);
                }

                if (line.Contains(openingTag) && line.Contains(closingTag))
                {
                    sb.Append(line.Substring(line.IndexOf(openingTag), line.IndexOf(closingTag) - 1)); //Remove surrounding data.
                    sb.Replace(openingTag, "");
                    sb.Replace(closingTag, "");
                    break;
                }

                if (line.Contains(openingTag))
                {
                    sb.Append(line.Substring(line.IndexOf(openingTag), line.Length - 1)); //Remove data from beginning to open tag
                    sb.Replace(openingTag, "");
                    readLines = true;
                }

                if (line.Contains(closingTag))
                {
                    sb.Append(line.Substring(0, line.IndexOf(closingTag) - 1)); //Remove data from close tag to end.
                    sb.Replace(closingTag, "");
                    readLines = true;
                }

            }

            // Check to see if there is anything in the storage variable
            if (sb.Length > 0)
            {
                sb = sb.Replace(" ", "");
                return sb.ToString();
            }

            else
            {
                // I feel like this isn't going to work if you actually tried, but you get the point. 
                throw new System.ArgumentException("Didn't find any data for that tag!", element); //Throw exception!
            }
        }
        
        //Or if it's a while down, just use a list of strings to get down there.
        public static string getElement(string xmlPath, string[] elementPath)
        {
            string data;

            data = getElement(xmlPath, false, elementPath[0]);

            if (elementPath.Length == 1)
                return data;

            foreach (string elementName in elementPath)
            {
                data = getElement(xmlPath, true, elementPath[0]);
            }

            return data;
        }

        public static bool getAPIData(string apiURL, string downloadLocation)
        {
            using (WebClient web = new WebClient())
            {
                web.DownloadFile(apiURL, downloadLocation);
            }

            // Use these returns 

            if (File.Exists(downloadLocation))
                return true;

            else
                return false;
        }
    }
}