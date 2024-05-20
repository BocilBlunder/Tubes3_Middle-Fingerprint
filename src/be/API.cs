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
    public static FingerprintOwner SearchFingerprint(Bitmap b, bool isBM)
    {
        dbController = new DatabaseManager("localhost", "tubes3", "root", "root");
        try
        {
            // Load input fingerprint image
            Bitmap inputFingerprint = b;
            // Segment the input image to ASCII
            string inputAscii = ImageProcessing.SegmentToAscii(inputFingerprint, 0, 0, inputFingerprint.Width, inputFingerprint.Height);
            // Load fingerprint images from the database
            List<FingerprintOwner> databaseOwner = dbController.getImageFromDB();

            // Convert database images to ASCII
            List<string> databaseAscii = new List<string>();
            foreach (var owner in databaseOwner)
            {
                string ascii = ImageProcessing.SegmentToAscii(owner.image, 0, 0, owner.image.Width, owner.image.Height);
                databaseAscii.Add(ascii);
            }
            
            if(databaseAscii.Count != 0){
                Console.WriteLine(databaseAscii.Count);
            }

            // Search for a match using KMP and Boyer-Moore algorithms
            bool matchFound = false;
            int j = 0;
            for (int i = 0; i < databaseAscii.Count; i++)
            {
                if (KMP.Search(databaseAscii[i], inputAscii) && !isBM)
                {
                    Console.WriteLine($"KMP: Match found in fingerprint {i + 1}");
                    matchFound = true;
                    j = i;
                    break;
                }

                if (BM.Search(databaseAscii[i], inputAscii) && isBM)
                {
                    Console.WriteLine($"BM: Match found in fingerprint {i + 1}");
                    matchFound = true;
                    j = i;
                    break;
                }
            }

            if(matchFound){
                return databaseOwner[j];
            }

            if (!matchFound)
            {
                int x = 0;
                Console.WriteLine("Tet");
                List<int> dist = new List<int>();
                foreach (string ascii in databaseAscii)
                {
                    x += 1;
                    Console.WriteLine(x);
                    dist.Add(StringDistance.LevenshteinDistance(ascii, inputAscii));
                }
                int i = dist.IndexOf(dist.Min());
                double percentage = StringDistance.CalculateSimilarityPercentage(inputAscii, databaseAscii[i]);
                if (percentage > 0)
                {
                    return databaseOwner[i];
                }


                Console.WriteLine("No match found.");
                return new FingerprintOwner(null, "", "");
            }
        } catch (Exception e){
            Console.WriteLine(e.Message);
            return new FingerprintOwner(null, "", ""); ;
        }    
        return new FingerprintOwner(null, "", "");
    }


    public static void Main(string[] args){
        dbController = new DatabaseManager("localhost", "tubes3", "root", "root");
        try{
            // Load input fingerprint image
            Bitmap inputFingerprint = new Bitmap("..\\..\\test\\1__M_Left_index_finger.BMP");
            Console.WriteLine("TES");
            // Segment the input image to ASCII
            string inputAscii = ImageProcessing.SegmentToAscii(inputFingerprint, 0, 0, inputFingerprint.Width, inputFingerprint.Height);
            Console.WriteLine("TES1");
            // Load fingerprint images from the database
            List<FingerprintOwner> databaseOwner = dbController.getImageFromDB();

            // Convert database images to ASCII
            List<string> databaseAscii = new List<string>();
            foreach (var owner in databaseOwner)
            {
                string ascii = ImageProcessing.SegmentToAscii(owner.image, 0, 0, owner.image.Width, owner.image.Height);
                databaseAscii.Add(ascii);
            }
            
            if(databaseAscii.Count != 0){
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

            if(matchFound){
                Console.WriteLine(databaseOwner[j].nama);
            }

            if (!matchFound)
            {
                Console.WriteLine("No match found.");
            }
        } catch (Exception e){
            Console.WriteLine(e.Message);
        }    
    }

    public static List<string> getOwnerBiodata (string name){
        List<string> alayname = dbController.getAllAlayNames();
        string finalName;
        foreach (string nama in alayname){
            if (RegexChecker.IsValidWord(name, nama)){
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
        double percentage =  StringDistance.CalculateSimilarityPercentage(name, alayname[i]);
        if(percentage > 0)
        {
            finalName = alayname[i];
            return dbController.getBiodata(finalName);
        }
        return new List<string>();
    }
}

#pragma warning restore