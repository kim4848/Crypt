using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LetMeCrypterU
{
    class crypt
    {
        private static List<string> keys = new List<string>();

        public crypt(string stiTilNøgle)
        {
            keys = new List<string>();
            readKey(stiTilNøgle);
            string keyString = keys[0].Trim();
        }

        public string Decrypt(string base64String)
        {
            string result = "Error";

        
            string ivStr = "";
            string dataStr ="";
            try
            {
                byte[] dataInput = Convert.FromBase64String(base64String);
                string decodedString = Encoding.Default.GetString(dataInput);
                dataStr = decodedString.Remove(decodedString.IndexOf("::"));
                ivStr = decodedString.Substring(decodedString.IndexOf("::") + 2);
            }
            catch
            {
                throw new UnknownFormatException("Ugyldigt format");

            }


            byte[] key = Convert.FromBase64String(keys[0].Trim());
            byte[] iv = Encoding.Default.GetBytes(ivStr);

            if (key.Length < 32)
            {
                var paddedkey = new byte[32];
                System.Buffer.BlockCopy(key, 0, paddedkey, 0, key.Length);
                key = paddedkey;
            }

            using (var rijndaelManaged = new RijndaelManaged { Padding = PaddingMode.PKCS7, Key = key, IV = iv, Mode = CipherMode.CBC })
            {
                rijndaelManaged.BlockSize = 128;
                rijndaelManaged.KeySize = 256;
                using (var memoryStream =
                       new MemoryStream(Convert.FromBase64String(dataStr)))
                using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    result = new StreamReader(cryptoStream, Encoding.UTF8).ReadToEnd();

                }
            }
            return result;
        }
        // Gammel kryptering skal udfases
        public string DecryptRJ256(string prm_text_to_decrypt)
        {

            var sEncryptedString = prm_text_to_decrypt;

            var myRijndael = new RijndaelManaged()
            {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 256
            };

            var key = Encoding.ASCII.GetBytes(keys[0]);
            var IV = Encoding.ASCII.GetBytes(keys[1]);

            var decryptor = myRijndael.CreateDecryptor(key, IV);

            var sEncrypted = Convert.FromBase64String(sEncryptedString);

            var fromEncrypt = new byte[sEncrypted.Length];

            var msDecrypt = new MemoryStream(sEncrypted);
            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            return (Encoding.UTF8.GetString(fromEncrypt));
        }

        public string Encrypt(string prm_text_to_encrypt)
        {
            var sToEncrypt = prm_text_to_encrypt;

            var myRijndael = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                KeySize = 256
            };

            myRijndael.GenerateIV();
            string keyString = keys[0].Trim();
            byte[] key = Convert.FromBase64String(keyString);

            if (key.Length < 32)
            {
                var paddedkey = new byte[32];
                System.Buffer.BlockCopy(key, 0, paddedkey, 0, key.Length);
                key = paddedkey;
            }

            var encryptor = myRijndael.CreateEncryptor(key, myRijndael.IV);

            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            var toEncrypt = Encoding.UTF8.GetBytes(sToEncrypt);

            //  var test=toEncrypt.conver


            System.Text.Encoding iso_8859_1 = System.Text.Encoding.UTF8;//.GetEncoding("ISO-8859-1");
         

            // Convert to ISO-8859-1 bytes.
            byte[] isoBytes = iso_8859_1.GetBytes(sToEncrypt);

            //  csEncrypt.Write(isoBytes, 0, isoBytes.Length);
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            var encrypted = msEncrypt.ToArray();

            var encryptedAndCBC = Convert.ToBase64String(encrypted) + "::" + Encoding.Default.GetString(myRijndael.IV);

            return (Convert.ToBase64String(Encoding.Default.GetBytes(encryptedAndCBC)));
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
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
