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
    
    public static string FindBestPixel(string binaryString, int blockSize)
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

    // Fungsi utama untuk memproses gambar dan mengekstrak blok terbaik
    public static string ReadBestPixelFromImage(Bitmap bmp, int blockSize)
    {
        string binaryString = ImageToBinary(bmp, 0, 0, bmp.Width, bmp.Height);
        return BinaryToAscii(FindBestPixel(binaryString, blockSize));
    }
}
