using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConvertRomanToInt
{
    class Program
    {
        static void Main()
        {
            foreach (var t in TestCases)
            {
                // the key will be the roman characters
                string romanNumeral = t.Key.ToUpper();
                // the value is the arabic number (0 means 'invalid')
                int arabicAnswer = t.Value;

                // call the method to see if it's valid
                var validRomanNumeral = IsValidRomanNumber(romanNumeral);

                // if it should be invalid but is valid (or vice-versa) log a bug
                if (Convert.ToBoolean(arabicAnswer) != validRomanNumeral)
                {
                    Console.WriteLine("BUG! BUG! BUG! {0} returned {1} but expected to be {2}",
                        romanNumeral,
                        validRomanNumeral,
                        Convert.ToBoolean(arabicAnswer));
                }

                if (validRomanNumeral) // it's a valid roman sequence, or we logged a bug - continuing either way to see what value we get
                {
                    var integerResult = ConvertValidRomanToInt(romanNumeral);

                    var passFail = "FAIL!!!";
                    if (integerResult == arabicAnswer)
                        passFail = "PASS!!!";

                    Console.WriteLine(passFail + " " + romanNumeral + ": " + integerResult);
                }
                else
                {
                    if (romanNumeral == string.Empty)
                        romanNumeral = "[EmptyString]";

                    Console.WriteLine("{0} is not a valid roman numeral.", romanNumeral);
                }
            }
            Console.ReadLine();
        }
        /// <summary>
        /// Takes in a valid RomanNumeral sequence and returns an int value
        /// </summary>
        /// <param name="romanNumeral"></param>
        /// <returns></returns>
        private static int ConvertValidRomanToInt(string romanNumeral)
        {
            // no zeros in roman numerals, so this is a safe starting point
            var integerResult = 0;

            // for each char in the sequence
            for (var i = 0; i < romanNumeral.Length; i++)
            {
                // get the key so you can get the value later
                var key = romanNumeral[i].ToString();

                // we will need to know if you matched two
                bool matchedTwo = false;

                if (i < romanNumeral.Length - 1) // we are not yet to the final character
                {
                    // get the key of the next char in the string
                    var tkey = key + romanNumeral[i + 1].ToString();
                    if (PossibleValues.ContainsKey(tkey)) // you have a subtraction match
                    {
                        // add the subtracted value from the current value
                        integerResult += PossibleValues[tkey];
                        // since you subtracted, skip that char in the next loop
                        i++;
                        matchedTwo = true;
                    }
                }

                if (!matchedTwo) // didn't match two chars
                {
                    // so if it's in the dictionary, add it
                    if (PossibleValues.ContainsKey(key))
                    {
                        integerResult += PossibleValues[key];
                    }
                }
            }

            return integerResult;
        }
        /// <summary>
        /// Takes in a string and determines if it is a valid roman numeral
        /// </summary>
        /// <param name="romanNumeral"></param>
        /// <returns></returns>
        private static bool IsValidRomanNumber(string romanNumeral)
        {
            // nothing null or empty shall pass
            if (string.IsNullOrEmpty(romanNumeral))
                return false;

            // convert it to upper, sheesh
            romanNumeral = romanNumeral.ToUpper();

            // make sure they only contain the 7 roman characters
            if (!new Regex(@"^[" + GetRegExValidator + "]+$").IsMatch(romanNumeral))
                return false;

            // for each char in the string, check it against the next two (or three if necessary)
            for (var i = 0; i < romanNumeral.Length - 1; i++)
            {
                // there are no 0's in roman numerals, so safe default assignment
                var secondValue = 0;
                var thirdValue = 0;

                // get the values of the roman characters
                // there has to be a first value because length is not 0
                var firstValue = PossibleValues[romanNumeral[i].ToString()];

                if (i < romanNumeral.Length - 1) // there might be a value after the first
                {
                    secondValue = PossibleValues[romanNumeral[i + 1].ToString()];

                    if (i < romanNumeral.Length - 2) // there might be a value after the second
                    {
                        thirdValue = PossibleValues[romanNumeral[i + 2].ToString()];
                    }
                }

                if (firstValue < secondValue) // subtraction case
                {
                    if (IsAFive(firstValue)) // VI is valid, VX is invalid
                    {
                        return false;
                    }
                    if (firstValue <= thirdValue) // XLI or XLV are valid, IXV or CMM or XLX are invalid
                    {
                        return false;
                    }
                    if (firstValue * 10 < secondValue) // IX is valid, IC is invalid
                    {
                        return false;
                    }
                }

                else if (firstValue == secondValue) // repeating case
                {
                    if (IsAFive(firstValue)) // VVI is invalid - cannot have two fives in a row
                    {
                        return false;
                    }
                    if (secondValue < thirdValue) // XXC is invalid - cannot have two of the same value before a subtraction
                    {
                        return false;
                    }
                    if (secondValue == thirdValue && i < romanNumeral.Length - 3) // confirm there is a 4th value
                    {
                        var fourthValue = PossibleValues[romanNumeral[i + 3].ToString()];
                        if (thirdValue == fourthValue) // cannot have 4 in a row
                        {
                            return false;
                        }
                    }
                }

                else if (firstValue > secondValue) // standard case
                {
                    if (firstValue < thirdValue) // LIX is valid, XIL is invalid
                    {
                        return false;
                    }
                    if ((IsAFive(firstValue)) && (firstValue == thirdValue)) // XIX is valid; VIV is not            
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Fives are special in Roman Numerals. This method tells you if you're dealing with one
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsAFive(int value)
        {
            if (value == 5 || value == 50 || value == 500)
                return true;

            return false;
        }
        /// <summary>
        /// Use the dictionary of possible roman characters to build a regular expression of roman characters
        /// </summary>
        private static string GetRegExValidator
        {
            get
            {
                string result = string.Empty;

                foreach (var p in PossibleValues)
                {
                    if (p.Key.Length == 1)
                    {
                        result += p.Key;
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// A dictionary of all roman characters and subtractions we care about
        /// </summary>
        private static readonly Dictionary<string, int> PossibleValues =
            new Dictionary<string, int>
        {
                { "M", 1000 },
                { "CM", 900 },
                { "D", 500 },
                { "CD", 400 },
                { "C", 100 },
                { "XC", 90 },
                { "L", 50 },
                { "XL", 40 },
                { "X", 10 },
                { "IX", 9 },
                { "V", 5 },
                { "IV", 4 },
                { "I", 1 }
        };
        /// <summary>
        /// A dictionary of test cases
        /// </summary>
        private static readonly Dictionary<string, int> TestCases =
            new Dictionary<string, int>
        {
                { "IIII", 0 },
                { "VIX", 0 },
                { "VIV", 0 },
                { "VX", 0 },
                { "VJK", 0 },
                { "VIJ", 0 },
                { "DCD", 0 },
                { "CCD", 0 },
                { "LXL", 0 },
                { "XXL", 0 },
                { "DD", 0 },
                { "LL", 0 },
                { "VVV", 0 },
                { "", 0 },
                { "XX XX", 0 },
                { "MMMM", 0 },
                { "IC", 0 },
                { "IVII", 0 },
                { "I", 1 },
                { "III", 3 },
                { "IV", 4 },
                { "V", 5 },
                { "VI", 6 },
                { "VII", 7 },
                { "IX", 9 },
                { "X", 10 },
                { "XX", 20 },
                { "XL", 40},
                { "XLIX", 49 },
                { "LIX", 59 },
                { "cC", 200 },
                { "CDXCIX", 499 },
                { "MXL", 1040 },
                { "ML", 1050},
                { "MCD", 1400 },
                { "MMMCMXCVIII", 3998 },
                { "MMMCMXCIX", 3999 }
        };
    }
}
