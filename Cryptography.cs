using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic

public partial class SIFRELEME
{

    // Change this with your key

    private static byte[] KEY_64 = new byte[] { 52, 16, 93, 156, 78, 4, 218, 32 };
    private static byte[] IV_64 = new byte[] { 55, 103, 246, 79, 36, 99, 167, 3 };

    // TripleDES için 24 byte ve 192 bit anahtar  
    private static byte[] KEY_192 = new byte[] { 42, 16, 93, 156, 78, 4, 218, 32, 15, 167, 44, 80, 26, 250, 155, 112, 2, 94, 11, 204, 119, 35, 184, 197 };
    private static byte[] IV_192 = new byte[] { 55, 103, 246, 79, 36, 99, 167, 3, 42, 5, 62, 83, 184, 7, 209, 13, 145, 23, 200, 58, 173, 10, 121, 222 };

    public static string Encode3DES(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = String2HEX(value);
            var DESKey = new[] { 200, 5, 78, 232, 9, 6, 0, 4 };
            var DESInitializationVector = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            using (var cryptoProvider = new DESCryptoServiceProvider())
            { 
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(DESKey, DESInitializationVector), CryptoStreamMode.Write))
                    {
                        using (var writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(value);
                            writer.Flush();
                            cryptoStream.FlushFinalBlock();
                            writer.Flush();
                            return String2HEX(Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));
                        }
                    }
                }
            }
        }
        else
        {
            return value;
        }
    }


    // 3 DES çözme
    public static string Decode3DES(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = HttpContext.Current.Server.UrlEncode(value);
            value = value.Replace(" ", "+");
            int mod4 = value.Length % 4;
            if (mod4 > 0)
            {
                value += new string('=', 4 - mod4);
            }

            value = HEXString2ASCII(value);
            var DESKey = new[] { 200, 5, 78, 232, 9, 6, 0, 4 };
            var DESInitializationVector = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            using (var cryptoProvider = new DESCryptoServiceProvider())
            {
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(value)))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(DESKey, DESInitializationVector), CryptoStreamMode.Read))
                    {
                        using (var reader = new StreamReader(cryptoStream))
                        {
                            return HEXString2ASCII(reader.ReadToEnd());
                        }
                    }
                }
            }
        }
        else
        {
            return "";
        }
    }

    public static string String2HEX(string str)
    {
        try
        {
            string sHEX = "";
            str = HttpContext.Current.Server.UrlEncode(str);
            for (int i = 0, loopTo = str.Length - 1; i <= loopTo; i++)
                sHEX = sHEX + Conversion.Hex(Strings.Asc(str[i]));
            return sHEX;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static string HEXString2ASCII(string hexString)
    {
        try
        {
            var sb = new StringBuilder();
            for (int i = 0, loopTo = hexString.Length - 2; i <= loopTo; i += 2)
                sb.Append(Convert.ToString(Convert.ToChar(int.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber))));
            return HttpContext.Current.Server.UrlDecode(sb.ToString());
        }
        catch (Exception ex)
        {
            return "";
        }
    }
}
