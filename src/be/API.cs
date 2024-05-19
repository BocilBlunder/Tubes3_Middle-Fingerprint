#pragma warning disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MySql.Data.MySqlClient;

public class API
{
    public static Bitmap SearchFingerprint(Bitmap b, bool isBM)
    {
        try{
            // Load input fingerprint image
            Bitmap inputFingerprint = b;
            Console.WriteLine("TES");
            // Segment the input image to ASCII
            string inputAscii = ImageProcessing.SegmentToAscii(inputFingerprint, 0, 0, inputFingerprint.Width, inputFingerprint.Height);
            Console.WriteLine("TES1");
            // Load fingerprint images from the database
            List<Bitmap> databaseFingerprints = LoadFingerprintsFromDatabase();
            Console.WriteLine("TES2");

            // Convert database images to ASCII
            List<string> databaseAscii = new List<string>();
            foreach (var bmp in databaseFingerprints)
            {
                string ascii = ImageProcessing.SegmentToAscii(bmp, 0, 0, bmp.Width, bmp.Height);
                databaseAscii.Add(ascii);
            }
            
            if(databaseAscii.Count != 0){
                Console.WriteLine(databaseAscii.Count);
            }

            // Search for a match using KMP and Boyer-Moore algorithms
            bool matchFound = false;
            for (int i = 0; i < databaseAscii.Count; i++)
            {
                if (KMP.Search(databaseAscii[i], inputAscii) && !isBM)
                {
                    Console.WriteLine($"KMP: Match found in fingerprint {i + 1}");
                    matchFound = true;
                    return databaseFingerprints[i];
                }

                if (BM.Search(databaseAscii[i], inputAscii) && isBM)
                {
                    Console.WriteLine($"BM: Match found in fingerprint {i + 1}");
                    matchFound = true;
                    return databaseFingerprints[i];
                }
            }

            if (!matchFound)
            {
                Console.WriteLine("No match found.");
            }
            return null;
        } catch (Exception e){
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public static void Main(string[] args){
        try{
            // Load input fingerprint image
            Bitmap inputFingerprint = new Bitmap("..\\..\\test\\1__M_Left_index_finger.BMP");
            Console.WriteLine("TES");
            // Segment the input image to ASCII
            string inputAscii = ImageProcessing.SegmentToAscii(inputFingerprint, 0, 0, inputFingerprint.Width, inputFingerprint.Height);
            Console.WriteLine("TES1");
            // Load fingerprint images from the database
            List<Bitmap> databaseFingerprints = LoadFingerprintsFromDatabase();
            Console.WriteLine("TES2");

            // Convert database images to ASCII
            List<string> databaseAscii = new List<string>();
            foreach (var bmp in databaseFingerprints)
            {
                string ascii = ImageProcessing.SegmentToAscii(bmp, 0, 0, bmp.Width, bmp.Height);
                databaseAscii.Add(ascii);
            }
            
            if(databaseAscii.Count != 0){
                Console.WriteLine(databaseAscii.Count);
            }

            // Search for a match using KMP and Boyer-Moore algorithms
            bool matchFound = false;
            for (int i = 0; i < databaseAscii.Count; i++)
            {
                if (KMP.Search(databaseAscii[i], inputAscii))
                {
                    Console.WriteLine($"KMP: Match found in fingerprint {i + 1}");
                    matchFound = true;
                }

                if (BM.Search(databaseAscii[i], inputAscii))
                {
                    Console.WriteLine($"BM: Match found in fingerprint {i + 1}");
                    matchFound = true;
                }
            }

            if (!matchFound)
            {
                Console.WriteLine("No match found.");
            }
        } catch (Exception e){
            Console.WriteLine(e.Message);
        }    
    }

    public static List<Bitmap> LoadFingerprintsFromDatabase()
    {
        string connectionString = "server=localhost;user=root;password=;database=tubes3";
        string query = "SELECT berkas_citra FROM sidik_jari";

        List<Bitmap> fingerprints = new List<Bitmap>();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string imageData = "..\\..\\..\\..\\..\\..\\" + (string)reader["berkas_citra"];
                        Bitmap bmp = new Bitmap(imageData);
                        fingerprints.Add(bmp);
                    }
                }
            }
        }

        return fingerprints;
    }
}

#pragma warning restore