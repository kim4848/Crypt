﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LetMeCrypterU
{
    class crypt2
    {
        private static List<string> keys = new List<string>();

        public crypt2(string stiTilNøgle)
        {
            keys = new List<string>();
            readKey(stiTilNøgle);
            string keyString = keys[0].Trim();
        }

        public string Decrypt(string base64String)
        {
            string result = "Error";

            byte[] dataInput = Convert.FromBase64String(base64String);
            string decodedString = Encoding.Default.GetString(dataInput);

            string dataStr = decodedString.Remove(decodedString.IndexOf("::"));
            string ivStr = decodedString.Substring(decodedString.IndexOf("::") + 2);

            byte[] key = Convert.FromBase64String(keys[0].Trim());
            byte[] iv = Encoding.Default.GetBytes(ivStr);

            if (key.Length < 32)
            {
                var paddedkey = new byte[32];
                System.Buffer.BlockCopy(key, 0, paddedkey, 0, key.Length);
                key = paddedkey;
            }

            using (var rijndaelManaged = new RijndaelManaged { Padding = PaddingMode.Zeros, Key = key, IV = iv, Mode = CipherMode.CBC })
            {
                rijndaelManaged.BlockSize = 128;
                rijndaelManaged.KeySize = 256;
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(dataStr)))
                using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    result = new StreamReader(cryptoStream, Encoding.Default).ReadToEnd();
                }
            }
            return result;
        }

        public string Encrypt(string prm_text_to_encrypt)
        {
            var sToEncrypt = prm_text_to_encrypt;

            var myRijndael = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                KeySize = 256,
                Padding = PaddingMode.Zeros
            };

            myRijndael.GenerateIV();
            string keyString = keys[0].Trim();
            byte[] key = Convert.FromBase64String(keyString);

            if (key.Length < 32)
            {
                int i = 0;
                var paddedkey = new byte[32];
                System.Buffer.BlockCopy(key, 0, paddedkey, 0, key.Length);
                key = paddedkey;
            }

            var encryptor = myRijndael.CreateEncryptor(key, myRijndael.IV);

            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            var toEncrypt = sToEncrypt.Select(Convert.ToByte).ToArray();// Encoding.UTF8.GetBytes(sToEncrypt);

            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            var encrypted = msEncrypt.ToArray();

            var encryptedAndCBC = Convert.ToBase64String(encrypted) + "::" + Encoding.Default.GetString(myRijndael.IV);

            return (Convert.ToBase64String(Encoding.Default.GetBytes(encryptedAndCBC)));
        }

        public void decryptFile(string inputPath)
        {
            FileInfo fi = new FileInfo(inputPath);

            if (fi.Extension.Equals(".LMC"))
            {
                string newfileName = fi.FullName.Replace(".LMC", "");
              string read = File.ReadAllText(inputPath);

            //    var temp = File.ReadAllBytes(inputPath);
            //    var read = System.Text.Encoding.Default.GetString(deCompress(temp));


                var result = Decrypt(read);
                result = result.TrimEnd('\0');
                var base64EncodedBytes = System.Convert.FromBase64String(result.Trim());

           


                using (var fs = new FileStream(newfileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(base64EncodedBytes, 0, base64EncodedBytes.Length);
                }
            }
            else
            {
                throw new Exception("Ikke korrekt fil format");
            }
        }

        public void cryptFile(string inputFilePath)
        {

           // FileStream fs = new FileStream(inputFilePath, FileMode.Open);
            FileInfo fi = new FileInfo(inputFilePath);
            Byte[] bytes = File.ReadAllBytes(inputFilePath);            

            String file = Convert.ToBase64String(bytes);
            var result = Encrypt(file);

         //   var output = Compress(GenerateStreamFromString(result));
         //   System.IO.File.WriteAllBytes(inputFilePath + ".LMC", output);

            System.IO.File.WriteAllText(inputFilePath + ".LMC", result);
        }


        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static byte[] Compress(Stream input)
        {
            using (var compressStream = new MemoryStream())
            using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress))
            {
                input.CopyTo(compressor);
                compressor.Close();
                return compressStream.ToArray();
            }
        }

        private static byte[] deCompress(Byte[] bytes)
        {

            var output = new MemoryStream();
            using (var compressStream = new MemoryStream(bytes))
            using (var compressor = new DeflateStream(compressStream, CompressionMode.Decompress))
            {
                compressor.CopyTo(output);
                compressor.Close();
                return output.ToArray();
            }
        }

        private static void readKey(string stiTilNøgle)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StreamReader sr = new StreamReader(stiTilNøgle + "256key"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int len = line.Length;
                        keys.Add(line);
                    }
                }
                string key = sb.ToString();
            }
            catch
            {
                throw new Exception("Nøglefil eller nøgle kunne ikke læses");
            }
        }
    }
}
