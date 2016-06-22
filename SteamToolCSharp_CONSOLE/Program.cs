using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolCSharp_CONSOLE
{
    class Program
    {
        static void Main(string[] args)
        {
            string s;
            string s2;

            string[] terms = { "foo", "bar" };

            s = getElement("C:\\Users\\Nicky\\Documents\\testXML.xml", false, "foo");
            Console.WriteLine(s);

            s2 = getElement("C:\\Users\\Nicky\\Documents\\testXML.xml", terms);
            Console.WriteLine(s2);

        }

        public static string getElement(string xmlPath, bool isString, string element)
        {
            StringBuilder sb;

            string openingTag;
            string closingTag;

            bool readLines;

            string[] apiData = null; // HERE

            if (isString == false)
                apiData = File.ReadAllLines(xmlPath);

            // Not quite sure what you're doing here. If you're assigning a "fallback" if isString is false, just set it to null in the declaration seen above
            //else
            //    apiData = xmlPath;

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
                    sb.Append(line);
                    sb.Replace(openingTag, "");
                    sb.Replace(closingTag, "");
                    break;
                }

                if (line.Contains(openingTag))
                {
                    sb.Append(line);
                    sb.Replace(openingTag, "");
                    readLines = true;
                }

                if (line.Contains(closingTag))
                {
                    sb.Append(line);
                    sb.Replace(closingTag, "");
                    readLines = true;
                }

            }

            // Check to see if there is anything in the storage variable
            if (sb.Length > 0)
            {
                sb = sb.Replace(" ", ""); // Is this efficient? No.
                return sb.ToString();
            }

            else
            {
                // I feel like this isn't going to work if you actually tried, but you get the point. ATM I'm writing this in notepad++ cause I dunno
                return "";
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
    }
}
