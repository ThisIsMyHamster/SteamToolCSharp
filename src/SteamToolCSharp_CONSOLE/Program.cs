using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SteamToolCSharp_CONSOLE
{
    class Program
    {
        // STEAM API KEY: 3CB2326C319D80429445D852BDCC01C4

        static string metaLocation = AppDomain.CurrentDomain.BaseDirectory;
        static string ResolveVanityURL = "http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=3CB2326C319D80429445D852BDCC01C4&format=xml&vanityurl=";
        static string GetPlayerItems = "http://api.steampowered.com/IEconItems_440/GetPlayerItems/v0001/?key=3CB2326C319D80429445D852BDCC01C4&format=xml&SteamId=";
        static string IGetPrices = "https://backpack.tf/api/IGetPrices/v4?key=56ce0410b98d88892ef9dd60";

        public static void getIGetPrices()
        {
            // Download the IGetPrices API file
            getAPIData(IGetPrices, metaLocation + "iGetPrices_temp.json");
        }

        static void Main(string[] args)
        {
            string vanityUsername;
            string numericalUsername;

            // List of files produced by the program, used by cleanDirectory to remove all temp files
            string[] createdFiles = { "numericID_temp.xml", "backpackContents_temp.xml", "iGetPrices_temp.json" };

            List<string> backpackItems = new List<string>();

            ThreadStart itemsStart = new ThreadStart(getIGetPrices);
            Thread itemsThread = new Thread(itemsStart);

            itemsThread.Start();

            // Get the users vanity Steam username
            Console.Write("Username: ");
            vanityUsername = Console.ReadLine();

            Console.WriteLine("Fetching numeric steam ID");

            // Download the XML file containing the user's numerical Steam ID
            getAPIData(ResolveVanityURL + vanityUsername, metaLocation + "numericID_temp.xml");

            // Parse and set the numericalUsername - NOTE: Since multipleNodes = false, the response will be stored in the first([0]) index of the returning list
            numericalUsername = getXMLElement(metaLocation + "numericID_temp.xml", "response/steamid", false)[0]; 

            Console.WriteLine("ID Found: {0}", numericalUsername);
            Console.WriteLine("Fetching backpack contents");

            // Download the XML file containing all of the user's current backpack items
            getAPIData(GetPlayerItems + numericalUsername, metaLocation + "backpackContents_temp.xml");

            Console.WriteLine("Backpack found");

            // Parse and set the backpack items to a list - NOTE: Since multipleNodes = true, the response will be stored in a list with a varying size (depending on the query)
            backpackItems = getXMLElement(metaLocation + "backpackContents_temp.xml", "result/items/item/defindex", true);

            foreach(string item in backpackItems)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("Backpack Size: {0} items", backpackItems.Count);

          

            //Cleanup any files that were created throughout the process of the program       
            //cleanDirectory(createdFiles);
            
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

        public static List<string> getXMLElement(string xmlPath, string nodePath, bool multipleNodes)
        {
            // README
            // THIS METHOD RETURNS LIST<STRING>. IF multipleNodes = false, THEN THE RESPONSE WILL ALWAYS BE IN THE FIRST INDEX ([0]).
            // xmlPath = The path of the XML doc you are parsing
            // elementPath = The path of the node you are choosing. If you wanted to select the <steamid> node, you would use this: "response/steamid".
            // multipleNodes = Used to indicate whether or not you are finding multiple nodes of the same id.

            XmlDocument xDoc = new XmlDocument();

            List<string> nodeStorage = new List<string>();

            xDoc.Load(xmlPath);

            if (!multipleNodes)
                nodeStorage.Add(xDoc.SelectSingleNode(nodePath).InnerText);

            else if (multipleNodes)
            {
                foreach (XmlNode node in xDoc.SelectNodes(nodePath))
                {
                    nodeStorage.Add(node.InnerText);
                }
            }

            return nodeStorage;
        } // Look inside for more information

        public static void cleanDirectory(string[] filesToDelete)
        {
            foreach (string file in filesToDelete)
            {
                if (File.Exists(metaLocation + file))
                    File.Delete(metaLocation + file);
            }
        }

        // The "Homebrew" XML parsing methods (use the shortcut: Ctrl + K + U to uncomment large blocks of text)

        //public static string getElement(string xmlPath, bool isString, string element) // isString is here in case you want to get an element from a string.
        //{
        //    StringBuilder sb;

        //    string openingTag;
        //    string closingTag;

        //    bool readLines;

        //    string[] apiData = null; // HERE

        //    if (isString == false)
        //        apiData = File.ReadAllLines(xmlPath);

        //    openingTag = "<" + element + ">";
        //    closingTag = "</" + element + ">";
        //    sb = new StringBuilder();
        //    readLines = false;

        //    // Then run through each line and search for the tags I want
        //    foreach (string line in apiData)
        //    {
        //        if ((readLines == true))
        //        {
        //            sb.Append(line);
        //        }

        //        if (line.Contains(openingTag) && line.Contains(closingTag))
        //        {
        //            sb.Append(line.Substring(line.IndexOf(openingTag), line.IndexOf(closingTag) - 1)); //Remove surrounding data.
        //            sb.Replace(openingTag, "");
        //            sb.Replace(closingTag, "");
        //            break;
        //        }

        //        if (line.Contains(openingTag))
        //        {
        //            sb.Append(line.Substring(line.IndexOf(openingTag), line.Length - 1)); //Remove data from beginning to open tag
        //            sb.Replace(openingTag, "");
        //            readLines = true;
        //        }

        //        if (line.Contains(closingTag))
        //        {
        //            sb.Append(line.Substring(0, line.IndexOf(closingTag) - 1)); //Remove data from close tag to end.
        //            sb.Replace(closingTag, "");
        //            readLines = true;
        //        }

        //    }

        //    // Check to see if there is anything in the storage variable
        //    if (sb.Length > 0)
        //    {
        //        return Regex.Replace(sb.ToString(), @"\s+", "");
        //    }

        //    else
        //    {
        //        // I feel like this isn't going to work if you actually tried, but you get the point. 
        //        throw new System.ArgumentException("Didn't find any data for that tag!", element); //Throw exception!
        //    }
        //}

        //public static string getElement(string xmlPath, string[] elementPath)
        //{
        //    //Or if it's a while down, just use a list of strings to get down there.

        //    string data;

        //    data = getElement(xmlPath, false, elementPath[0]);

        //    if (elementPath.Length == 1)
        //        return data;

        //    foreach (string elementName in elementPath)
        //    {
        //        data = getElement(xmlPath, true, elementPath[0]);
        //    }

        //    return data;
        //}
    }

}
