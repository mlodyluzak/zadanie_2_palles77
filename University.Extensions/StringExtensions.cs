using System;

namespace University.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidPESEL(this string input)
        {
            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

            if (input.Length != 11)
            {
                return false;
            }

            int controlSum = CalculateControlSum(input, weights);
            int controlNum = (10 - (controlSum % 10)) % 10;

            int lastDigit = int.Parse(input[input.Length - 1].ToString());

            return controlNum == lastDigit;
        }

        private static int CalculateControlSum(string input, int[] weights, int offset = 0)
        {
            int controlSum = 0;

            for (int i = 0; i < input.Length - 1; i++)
            {
                controlSum += weights[i + offset] * int.Parse(input[i].ToString());
            }

            return controlSum;
        }
    }
}
