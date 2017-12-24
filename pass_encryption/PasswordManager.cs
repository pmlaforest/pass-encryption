using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;



namespace WebTP1

{
    class PasswordManager
    {
        private byte[] AESkey = null;
        private byte[] IV = null;
        public string[] hashPassword(string password)
        {
/*#if (DEBUG)
            Console.WriteLine("Starting hashPassword() arg: "+password);
#endif*/
            //Used to store the hashed password and the salt (0: Hashed password 1: Salt)
            string[] hashArray = new string [2];

            byte[] salt = new byte[16];

            //Create a random salt
            var rand = RandomNumberGenerator.Create();
            rand.GetBytes(salt);

//#if (DEBUG)
//            Console.WriteLine("SALT:"+Convert.ToBase64String(salt));
//#endif

            string saltString = Convert.ToBase64String(salt);
            hashArray[1] = saltString;

            //Hashing password
            var hashedPass = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password+salt));
//#if (DEBUG)
//            Console.WriteLine("HashedPass:"+hashedPass);
//#endif
            hashArray[0] = Convert.ToBase64String(hashedPass);

            return hashArray;
        }

        public string[] hashPassword(string password,string salts)
        {
//#if (DEBUG)
//            Console.WriteLine("Starting hashPassword() arg: " + password);
//#endif
            //Used to store the hashed password and the salt (0: Hashed password 1: Salt)
            string[] hashArray = new string[2];

            byte[] salt = Convert.FromBase64String(salts);

//#if (DEBUG)
//            Console.WriteLine("SALT:" + Convert.ToBase64String(salt));
//#endif

            string saltString = Convert.ToBase64String(salt);
            hashArray[1] = saltString;

            //Hashing password
            hashArray[1] = saltString;

            //Hashing password
            var hashedPass = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password + salt));
//#if (DEBUG)
//            Console.WriteLine("HashedPass:" + hashedPass);
//#endif
            hashArray[0] = Convert.ToBase64String(hashedPass);

            return hashArray;
        }

        public string cryptTagPassword(string uncryptedPassword, string[] passAndSalt)
        {
            //Generating the AES key
            IV = Convert.FromBase64String(passAndSalt[1]);
            AESkey = GenerateAESKey(passAndSalt[0], Convert.FromBase64String(passAndSalt[1]));
            
//#if (DEBUG)
//            Console.WriteLine("Starting cryptTagPassword() arg: " + uncryptedPassword);
//            Console.WriteLine("Current AES: " + Convert.ToBase64String(AESkey) + "\nCurrent IV: " + Convert.ToBase64String(IV));
//#endif
            if(AESkey == null || IV == null)
            {
                Console.WriteLine("ERROR");
                return null;
            }
            /*
            var encryptor = Aes.Create().CreateEncryptor(AESkey, IV);

            byte[] passTemp = Encoding.ASCII.GetBytes(uncryptedPassword);
            byte[] passCrypt = new byte[passTemp.Length];

            encryptor.TransformBlock(passTemp, 0, passTemp.Length, passCrypt, 0);
#if(DEBUG)
            Console.WriteLine("Crypted password: " + Convert.ToBase64String(passCrypt));
#endif
            return Convert.ToBase64String(passCrypt);
            */
            var algorithm = Aes.Create();
            algorithm.KeySize = 256;
            algorithm.BlockSize = 128;
            var encryptor = algorithm.CreateEncryptor(AESkey,IV);
            byte[] passTemp = Encoding.ASCII.GetBytes(uncryptedPassword);
            byte[] output = new byte[encryptor.OutputBlockSize];
            int inoff = 0;
            int inputBlockSize = encryptor.InputBlockSize;

            if(!encryptor.CanTransformMultipleBlocks)
            {
                while(passTemp.Length - inoff > inputBlockSize)
                {
                    encryptor.TransformBlock(passTemp, inoff, passTemp.Length - inoff, output, 0);
                    inoff += encryptor.InputBlockSize;
                }
            }
            output = encryptor.TransformFinalBlock(passTemp, inoff, passTemp.Length - inoff);
            return Convert.ToBase64String(output);
        }

        public string decryptTagPassword(string cryptedPassword, string[] passAndSalt)
        {
            //Generating the AES key
            IV = Convert.FromBase64String(passAndSalt[1]);
            AESkey = GenerateAESKey(passAndSalt[0], Convert.FromBase64String(passAndSalt[1]));
            
            if (AESkey == null || IV == null)
            {
                Console.WriteLine("ERROR");
                return null;
            }

            var algorithm = Aes.Create();
            algorithm.KeySize = 256;
            algorithm.BlockSize = 128;
            var decryptor = algorithm.CreateDecryptor(AESkey, IV);
            byte[] passTemp = Convert.FromBase64String(cryptedPassword);
            byte[] output = new byte[decryptor.OutputBlockSize];
            int inoff = 0;
            int inputBlockSize = decryptor.InputBlockSize;

            if (!decryptor.CanTransformMultipleBlocks)
            {
                while (passTemp.Length - inoff > inputBlockSize)
                {
                    decryptor.TransformBlock(passTemp, inoff, passTemp.Length - inoff, output, 0);
                    inoff += decryptor.InputBlockSize;
                }

                
            }
            output = decryptor.TransformFinalBlock(passTemp, inoff, passTemp.Length - inoff);
            return Encoding.ASCII.GetString(output);
        }
        
        public bool compareHash(string firstHash, string secondHash)
        {
            return firstHash == secondHash;
        }

        private byte[] GenerateAESKey(string hashedPassword, byte[] salt)
        {
            try
            {
                var key = KeyDerivation.Pbkdf2(hashedPassword, salt, KeyDerivationPrf.HMACSHA256, 10000, 32);
            

                return key;

            }

            catch (Exception)
            {
                Console.WriteLine("ERROR");
            }

            return null;
        }
    }

}