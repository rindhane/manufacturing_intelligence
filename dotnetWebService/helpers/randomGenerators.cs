using System.Security.Cryptography;

namespace helpers{
    
    public static class RandomGenerators{
        
        private static string Alphabets = "abcdefghijklmnoqprstuvwyzxABCDEFGHIJKLMNOQPRSTUYWVZX";

        private static char CharacterProvider(int pos){
            return Alphabets[pos];
        } 

        public static string RandomStringProvider(int length)
        {
            var randInt = new System.Random();
            if (length >50) {
                length =50;
                System.Console.WriteLine("Length considered only 50 chars, Length more than 50 is not allowed");
            }
            string result="";
            for (int i=0; i<length; i++)
            {
                int temp= randInt.Next();
                int randVal= temp%52;
                result=result + CharacterProvider(randVal).ToString();
            }
            return result;
        }

        public static byte[] RandomByteGenerators(int size) {
            byte[] randomInput = new byte[size];
            var rngCsp = RandomNumberGenerator.Create();
            rngCsp.GetBytes(randomInput);
            rngCsp.Dispose();
            return randomInput;
        }
    }
}

