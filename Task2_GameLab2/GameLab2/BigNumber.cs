using System;
using System.Collections.Generic;
using System.Text;

namespace GameLab2
{


    public class BigNumber
    {

        private int[] number;
        private const int Base = 1000;
        public int ArrayLength => number.Length;

        public BigNumber(string numberString)
        {
            if (string.IsNullOrWhiteSpace(numberString))
                numberString = "0";

            numberString = numberString.Replace(" ", "").TrimStart('0');
            if (numberString == "") numberString = "0";

            foreach (char c in numberString)
            {
                if (!char.IsDigit(c)) throw new ArgumentException("Строка должна содержать только цифры.");
            }

            int length = numberString.Length;
            int blockCount = (length + 2) / 3;
            number = new int[blockCount];

            int blockIdx = blockCount - 1;
            for (int i = length; i > 0; i -= 3)
            {
                int start = Math.Max(0, i - 3);
                int len = i - start;
                string part = numberString.Substring(start, len);
                number[blockIdx--] = int.Parse(part);
            }
        }


        private BigNumber(int[] arr)
        {
            number = TrimLeadingZeros(arr);
        }


        public BigNumber Clone()
        {
            int[] clonedArray = new int[this.number.Length];
            Array.Copy(this.number, clonedArray, this.number.Length);
            return new BigNumber(clonedArray);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < number.Length; i++)
            {
                if (i == 0)
                    sb.Append(number[i].ToString());
                else
                    sb.Append(number[i].ToString("D3"));
            }
            return sb.ToString();
        }


        private int[] TrimLeadingZeros(int[] arr)
        {
            int startIdx = 0;
            while (startIdx < arr.Length - 1 && arr[startIdx] == 0)
            {
                startIdx++;
            }

            if (startIdx == 0) return arr;

            int[] trimmed = new int[arr.Length - startIdx];
            Array.Copy(arr, startIdx, trimmed, 0, trimmed.Length);
            return trimmed;
        }


        private int CompareTo(BigNumber bnum)
        {
            if (this.number.Length != bnum.number.Length)
                return this.number.Length.CompareTo(bnum.number.Length);

            for (int i = 0; i < this.number.Length; i++)
            {
                if (this.number[i] != bnum.number[i])
                    return this.number[i].CompareTo(bnum.number[i]);
            }
            return 0;
        }


        private BigNumber Add(BigNumber bnum)
        {
            int maxLen = Math.Max(this.number.Length, bnum.number.Length);
            int[] tempResult = new int[maxLen + 1];
            int carry = 0;

            int i = this.number.Length - 1;
            int j = bnum.number.Length - 1;
            int k = tempResult.Length - 1;

            while (i >= 0 || j >= 0 || carry > 0)
            {
                int sum = carry;
                if (i >= 0) sum += this.number[i--];
                if (j >= 0) sum += bnum.number[j--];

                tempResult[k--] = sum % Base;
                carry = sum / Base;
            }

            return new BigNumber(tempResult);
        }


        private BigNumber Substruct(BigNumber bnum)
        {
            if (this.CompareTo(bnum) < 0)
                throw new InvalidOperationException("Результат вычитания не может быть отрицательным.");

            int[] tempResult = new int[this.number.Length];
            int borrow = 0;

            int i = this.number.Length - 1;
            int j = bnum.number.Length - 1;
            int k = tempResult.Length - 1;

            while (i >= 0)
            {
                int diff = this.number[i--] - borrow;
                if (j >= 0) diff -= bnum.number[j--];
                if (diff < 0)
                {
                    diff += Base;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                tempResult[k--] = diff;
            }

            return new BigNumber(tempResult);
        }


        private BigNumber Multiply(double multiplier)
        {
            if (multiplier < 0) throw new ArgumentException("Множитель не должен быть отрицательным.");
            if (multiplier == 0) return new BigNumber("0");

            double[] intermediate = new double[this.number.Length];
            double fractionCarry = 0;

            for (int i = 0; i < this.number.Length; i++)
            {
                double current = this.number[i] * multiplier + fractionCarry;
                double blockValue = Math.Floor(current);
                intermediate[i] = blockValue;

                fractionCarry = Math.Round((current - blockValue) * Base, 2);

                if (fractionCarry >= Base)
                {
                    intermediate[i] += Math.Floor(fractionCarry / Base);
                    fractionCarry %= Base;
                }
            }
            int maxLen = this.number.Length + 5;
            int[] tempResult = new int[maxLen];
            int offset = maxLen - this.number.Length;
            long carry = 0;

            for (int i = this.number.Length - 1; i >= 0; i--)
            {
                long total = (long)intermediate[i] + carry;
                tempResult[offset + i] = (int)(total % Base);
                carry = total / Base;
            }

            int k = offset - 1;
            while (carry > 0 && k >= 0)
            {
                tempResult[k--] = (int)(carry % Base);
                carry /= Base;
            }

            return new BigNumber(tempResult);
        }


        private BigNumber Divide(double divisor)
        {
            if (divisor <= 0) throw new DivideByZeroException("Деление на ноль или отрицательное число.");

            int[] tempResult = new int[this.number.Length];
            double remainder = 0;

            for (int i = 0; i < this.number.Length; i++)
            {
                double current = remainder * Base + this.number[i];
                double divResult = current / divisor;
                tempResult[i] = (int)Math.Floor(divResult);

                remainder = current - (tempResult[i] * divisor);
            }

            return new BigNumber(tempResult);
        }


        public static BigNumber operator +(BigNumber a, BigNumber b) => a.Add(b);
        public static BigNumber operator -(BigNumber a, BigNumber b) => a.Substruct(b);
        public static BigNumber operator *(BigNumber a, double b) => a.Multiply(b);
        public static BigNumber operator /(BigNumber a, double b) => a.Divide(b);
        public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
        public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;

    }
}
