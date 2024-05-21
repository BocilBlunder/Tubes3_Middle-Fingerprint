using System;
using System.Drawing;  
using System.Text;

public class ImageProcessing {

    public static string SegmentToAscii(Bitmap bmp, int startX, int startY, int width, int height)
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
}
