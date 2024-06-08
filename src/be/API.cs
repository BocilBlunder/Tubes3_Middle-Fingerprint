#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

public class API
{
    private static DatabaseManager dbController;
    private static List<string> asciiList = new List<string>();

    public static void getCache()
    {
        dbController = new DatabaseManager("localhost", "tubes3", "root", "pass");
        List<FingerprintOwner> databaseOwner = dbController.getImageFromDB();
        List<string> databaseAscii = new List<string>();
        foreach (var owner in databaseOwner)
        {
            string ascii = ImageProcessing.ImageToAscii(owner.image, 0, 0, owner.image.Width, owner.image.Height);
            databaseAscii.Add(ascii);
        }
        asciiList = databaseAscii;
    }
    
    public static Tuple<FingerprintOwner, double> SearchFingerprint(Bitmap b, bool isBM)
    {
        dbController = new DatabaseManager("localhost", "tubes3", "root", "pass");
        try
        {
            // Load input fingerprint image
            Bitmap inputFingerprint = b;
            // Segment the input image to ASCII
            List<string> inputAscii = ImageProcessing.ReadBestPixelFromImage(inputFingerprint, 64);
            // Load fingerprint images from the database
            List<FingerprintOwner> databaseOwner = dbController.getImageFromDB();
            List<string> databaseAscii = asciiList;

            if (asciiList.Count != 0)
            {
                Console.WriteLine("[DEBUG] Loaded " + asciiList.Count + " fingerprints");
            }
            foreach (string bestInputAscii in inputAscii)
            {
                // Search for a match using KMP and Boyer-Moore algorithms
                bool matchFound = false;
                int j = 0;
                for (int i = 0; i < asciiList.Count; i++)
                {
                    if (KMP.Search(asciiList[i], bestInputAscii) && !isBM)
                    {
                        matchFound = true;
                        j = i;
                        break;
                    }

                    if (BM.Search(asciiList[i], bestInputAscii) && isBM)
                    {
                        matchFound = true;
                        j = i;
                        break;
                    }
                }

                if (matchFound)
                {
                    string algo = isBM ? "BM" : "KMP";
                    Console.WriteLine("[DEBUG] " + algo + " match found: " + databaseOwner[j].path);
                    return new Tuple<FingerprintOwner, double>(databaseOwner[j], 100);
                }
            }
            Console.WriteLine("[DEBUG] Starting LD");
            string newinputAscii = ImageProcessing.ImageToAscii(b, 0, 0, b.Width, b.Height);
            List<int> dist = new List<int>();
            foreach (string ascii in asciiList)
            {
                dist.Add(StringDistance.LevenshteinDistance(ascii, newinputAscii));
            }
            int x = dist.IndexOf(dist.Min());
            double percentage = StringDistance.CalculateSimilarityPercentage(newinputAscii, asciiList[x]);
            if (percentage > 0)
            {
                Console.WriteLine("[DEBUG] LD match found: " + databaseOwner[x].path);
                return new Tuple<FingerprintOwner, double>(databaseOwner[x], percentage);
            }


            Console.WriteLine("[DEBUG] No match found.");
            return new Tuple<FingerprintOwner, double>(new FingerprintOwner(null, "", ""), percentage);
            
        }
        catch (Exception e)
        {
            Console.WriteLine("[DEBUG C# error] " + e.Message);
            return new Tuple<FingerprintOwner, double>(new FingerprintOwner(null, "", ""), 0); ;
        }
        return new Tuple<FingerprintOwner, double>(new FingerprintOwner(null, "", ""), 0);
    }


    public static void Main(string[] args)
    {
        dbController = new DatabaseManager("localhost", "tubes3", "root", "pass");
        try
        {
            // Load input fingerprint image
            Bitmap inputFingerprint = new Bitmap("..\\..\\test\\1__M_Left_index_finger.BMP");
            Console.WriteLine("TES");
            // Segment the input image to ASCII
            string inputAscii = ImageProcessing.ImageToAscii(inputFingerprint, 0, 0, inputFingerprint.Width, inputFingerprint.Height);
            Console.WriteLine("TES1");
            // Load fingerprint images from the database
            List<FingerprintOwner> databaseOwner = dbController.getImageFromDB();

            // Convert database images to ASCII
            List<string> databaseAscii = new List<string>();
            foreach (var owner in databaseOwner)
            {
                string ascii = ImageProcessing.ImageToAscii(owner.image, 0, 0, owner.image.Width, owner.image.Height);
                databaseAscii.Add(ascii);
            }

            if (databaseAscii.Count != 0)
            {
                Console.WriteLine(databaseAscii.Count);
            }

            // Search for a match using KMP and Boyer-Moore algorithms
            bool matchFound = false;
            int j = 0;
            for (int i = 0; i < databaseAscii.Count; i++)
            {
                if (KMP.Search(databaseAscii[i], inputAscii))
                {
                    Console.WriteLine($"KMP: Match found in fingerprint {i + 1}");
                    matchFound = true;
                    j = i;
                }

                if (BM.Search(databaseAscii[i], inputAscii))
                {
                    Console.WriteLine($"BM: Match found in fingerprint {i + 1}");
                    matchFound = true;
                    break;
                }
            }

            if (matchFound)
            {
                Console.WriteLine(databaseOwner[j].nama);
            }

            if (!matchFound)
            {
                Console.WriteLine("No match found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static List<string> getOwnerBiodata(string name)
    {
        List<string> alayname = dbController.getAllAlayNames();
        string finalName;
        foreach (string nama in alayname)
        {
            if (RegexChecker.IsValidWord(name, nama))
            {
                finalName = nama;
                return dbController.getBiodata(finalName);
            }
        }

        List<int> dist = new List<int>();
        foreach (string nama in alayname)
        {
            dist.Add(StringDistance.LevenshteinDistance(name, nama));
        }

        int i = dist.IndexOf(dist.Min());
        double percentage = StringDistance.CalculateSimilarityPercentage(name, alayname[i]);
        if (percentage > 0)
        {
            finalName = alayname[i];
            return dbController.getBiodata(finalName);
        }
        return new List<string>();
    }
}

#pragma warning restore