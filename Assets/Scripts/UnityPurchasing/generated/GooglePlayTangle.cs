// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("QDG9ZEE/k4Xv5hRW2t+julTSqVa7ALyMW6eikERafI3b70lzjt45QO6HNYcxmJrjamaN0H1ZmrLSj4JMIRxki4kqsC44Pio3SS8BWRjjJMsD+tM3b1QUEjp2AIfQU3kTZ8SiZWTn6ebWZOfs5GTn5+ZJyEY2hxn8YK7KC+1qmDtJsB50slbaR0KLk0zWZOfE1uvg78xgrmAR6+fn5+Pm5WXv5+gEm0iF1V1scpspECGlO71uI9qWsoOQQIK6VZ8uUHqp1ZY62h4etU2G8b0KMMf7PyxGs4brJRKDbxr2w3EvGiXrswEbRZ0Fth7hP9/x2YMvwF8QD5P24bWpmtpUwUbmPoVq77I/Sn7omJ5VuEs8RBeAsF3JIfCW1COszsQv9eTl5+bn");
        private static int[] order = new int[] { 11,5,7,7,13,5,13,11,10,11,13,13,12,13,14 };
        private static int key = 230;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
