using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberStringConverter {

    static string[] prefixes = new string[] { "K", "M", "B", "T", "q", "Q", "s", "S", "O", "N", "D" };

	public static string convert(double n)
    {
        if (n < 10000) // 10K
            return n + "";
        else
        {
            // Get exponent (X in 1E+X)
            int exponent = (int)System.Math.Floor(System.Math.Log10(n));

            // Get first three significant digits
            int firstThreeSignificantDigits = (int)System.Math.Floor(n / System.Math.Pow(10, exponent - 2));
            string[] significantDigits = new string[3];
            significantDigits[2] = (firstThreeSignificantDigits % 10) + "";
            significantDigits[1] = ((firstThreeSignificantDigits / 10) % 10) + "";
            significantDigits[0] = ((firstThreeSignificantDigits / 100) % 10) + "";

            // Return scientific notation for really high numbers
            if (exponent >= 36)
                return significantDigits[0] 
                    + ((significantDigits[1] == "0" && significantDigits[2] == "0") ? 
                    "" : "." + significantDigits[1] + significantDigits[2])
                    + "E" + exponent;

            // Starts to count from K, so start by substracting 3
            int prefixIndex = exponent - 3;
            prefixIndex = prefixIndex / 3; // K -> 0, M -> 1, B -> 2...
            string prefix = prefixes[prefixIndex];

            switch (exponent % 3)
            {
                default:
                case 0: // 1.23T, 1q
                    return significantDigits[0] 
                        + ((significantDigits[1] == "0" && significantDigits[2] == "0") ? "" : ".") 
                        + ((significantDigits[1] == "0" && significantDigits[2] == "0") ? "" : significantDigits[1])
                        + ((significantDigits[2] == "0" && significantDigits[1] == "0") ? "" : significantDigits[2])
                        + prefix;
                case 1: // 12.3T, 10q
                    return significantDigits[0] + significantDigits[1] 
                        + ((significantDigits[2] == "0") ? "" : ".") 
                        + ((significantDigits[2] == "0") ? "" : significantDigits[2]) 
                        + prefix;
                case 2: // 123T, 100q
                    return firstThreeSignificantDigits + prefix;
            }
        }
    }

}
