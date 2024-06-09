using System;
using System.Drawing;  
using System.Text;

public class ImageProcessing {

    public static string ImageToAscii(Bitmap bmp, int startX, int startY, int width, int height)
    {
    StringBuilder binaryStringBuilder = new StringBuilder();
    for (int i = startY; i < startY + height && i < bmp.Height; i++)
    {
        for (int j = startX; j < startX + width && j < bmp.Width; j++)
        {
            Color pixel = bmp.GetPixel(j, i);
            int value = (pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11) > 128 ? 0 : 1;
            binaryStringBuilder.Append(value);
        }
    }
    return BinaryToAscii(binaryStringBuilder.ToString());
    }

    public static string ImageToBinary(Bitmap bmp, int startX, int startY, int width, int height)
    {
    StringBuilder binaryStringBuilder = new StringBuilder();
    for (int i = startY; i < startY + height && i < bmp.Height; i++)
    {
        for (int j = startX; j < startX + width && j < bmp.Width; j++)
        {
            Color pixel = bmp.GetPixel(j, i);
            int value = (pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11) > 128 ? 0 : 1;
            binaryStringBuilder.Append(value);
        }
    }
    return binaryStringBuilder.ToString();
    }

    public static string BinaryToAscii(string binaryStr)
    {
        StringBuilder asciiStr = new StringBuilder();
        for (int i = 0; i < binaryStr.Length; i += 8)
        {
            if (i + 8 <= binaryStr.Length)
            {
                string byteString = binaryStr.Substring(i, 8);
                asciiStr.Append((char)Convert.ToInt32(byteString, 2));
            }
        }
        return asciiStr.ToString();
    }

    private static int CountTransitions(string binaryString)
    {
        int transitions = 0;
        for (int i = 0; i < binaryString.Length - 1; i++)
        {
            if (binaryString[i] != binaryString[i + 1])
                transitions++;
        }
        return transitions;
    }
    
    public static string FindMaxTransitionsPixels(string binaryString, int blockSize)
    {
        int maxTransitionCount = 0;
        string bestSubstring = binaryString.Substring(0, Math.Min(blockSize, binaryString.Length));

        for (int i = 0; i < binaryString.Length - blockSize; i += blockSize)
        {
            string substring = binaryString.Substring(i, blockSize);
            int transitionCount = CountTransitions(substring);
            if (transitionCount > maxTransitionCount)
            {
                maxTransitionCount = transitionCount;
                bestSubstring = substring;
            }
        }
        return bestSubstring;
    }

    public static List<string> FindOptimalTransitionsRatioPixels(string binaryString, int blockSize, double threshold)
    {
        List<string> result = new List<string>();

        for (int i = 0; i < binaryString.Length - blockSize; i += 1)
        {
            string substring = binaryString.Substring(i, blockSize);
            int transitionCount = CountTransitions(substring);
            if ((double)transitionCount / blockSize > threshold)
            {
                result.Add(substring);
            }
        }
        return result;
    }

    // Fungsi utama untuk memproses gambar dan mengekstrak blok terbaik
    public static List<string> ReadBestPixelFromImage(Bitmap bmp, int blockSize)
    {
        List<string> bestPixelStrings = new List<string>();
        string binaryString = ImageToBinary(bmp, 0, 0, bmp.Width, bmp.Height);


        // Strategi 1. Memilih string dengan transisi terbesar
        bestPixelStrings.Add(BinaryToAscii(FindMaxTransitionsPixels(binaryString, blockSize)));


        // Strategi 2.
        // Memilih string-string dengan rasio transisi > 20%
        List<string> bestPixelsBinary = FindOptimalTransitionsRatioPixels(binaryString, blockSize, 0.15);
        foreach (string bestPixels in bestPixelsBinary)
        {
            bestPixelStrings.Add(BinaryToAscii(bestPixels));
        }


        // String-stringnya dishuffle sehingga tidak ada bias dalam pemilihan pattern yang digunakan
        // sehingga proses pencarian jadi lebih cepat
        Shuffle(ref bestPixelStrings);

        Console.WriteLine("Returning the patterns");
        return bestPixelStrings;
    }

    public static void Shuffle(ref List<string> list)
    {
        if (list.Count <= 1) return;

        Random random = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            string temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
