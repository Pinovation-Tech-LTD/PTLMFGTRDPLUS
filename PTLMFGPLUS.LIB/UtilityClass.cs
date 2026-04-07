using PTLMFGPLUS.ENTITY;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using static PTLMFGPLUS.ENTITY.EnumHelpers;

namespace PTLMFGPLUS.LIB
{
    public class UtilityClass
    {
        public static string Left(string host, int index)
        {
            return host.Substring(0, index);

        }

        public static string Right(string host, int index)
        {
            return host.Substring(host.Length - index);
        }
        public static double StrNagative(string Stname)
        {
            double rval = Convert.ToDouble(Stname.ToString().Replace("-", "").Trim());
            double valsign = (Stname.Trim().Substring(0, 1) == "-" ? -1.00 : 1.00);
            return (rval * valsign);
        }

        public static double StrPosOrNagative(string Stname)
        {
            if (Stname.Length == 0)
                return 0.00;
            double rval = Convert.ToDouble(Stname.ToString().Replace("-", "").Trim());
            double valsign = (Stname.Trim().Substring(0, 1) == "-" ? -1.00 : 1.00);
            return (rval * valsign);
        }
        public static string ExprToValue(string cExpr)
        {
            string mExpr1 = cExpr.Trim().Replace(",", "");
            mExpr1 = mExpr1.Replace("/", " div ");
            XmlDocument xmlDoc = new XmlDocument();
            XPathNavigator xPathNavigator = xmlDoc.CreateNavigator();
            mExpr1 = xPathNavigator.Evaluate(mExpr1).ToString();
            return mExpr1;
        }
        public static double DecimaltwoDigit(double value)
        {
            return Convert.ToDouble(value.ToString("#,##0.00;-#,##0.00;"));
        }
        public static bool TransactionDateCon(DateTime bdate, DateTime date)
        {
            DateTime Sysdate = System.DateTime.Today;
            bool result = (bdate < date && date <= Sysdate) ? true : false;
            return result;

        }
        public static string EncodePassword(string originalPassword)
        {

            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);
            return BitConverter.ToString(encodedBytes);
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        

        public static string Trans(double XX1, int Index)
        {
            Index = (Index == 0 ? 1 : Index);
            string[] X1 = new string[101];
            string[] Y1 = new string[6];
            string[] Z1 = new string[3];

            X1[0] = "Zero ";
            X1[1] = "One ";
            X1[2] = "Two ";
            X1[3] = "Three ";
            X1[4] = "Four ";
            X1[5] = "Five ";
            X1[6] = "Six ";
            X1[7] = "Seven ";
            X1[8] = "Eight ";
            X1[9] = "Nine ";
            X1[10] = "Ten ";
            X1[11] = "Eleven ";
            X1[12] = "Twelve ";
            X1[13] = "Thirteen ";
            X1[14] = "Fourteen ";
            X1[15] = "Fifteen ";
            X1[16] = "Sixteen ";
            X1[17] = "Seventeen ";
            X1[18] = "Eighteen ";
            X1[19] = "Nineteen ";
            X1[20] = "Twenty ";
            X1[30] = "Thirty ";
            X1[40] = "Forty ";
            X1[50] = "Fifty ";
            X1[60] = "Sixty ";
            X1[70] = "Seventy ";
            X1[80] = "Eighty ";
            X1[90] = "Ninety ";

            for (int J1 = 20; J1 <= 90; J1 = J1 + 10)
                for (int I1 = 1; I1 <= 9; I1++)
                    X1[J1 + I1] = X1[J1] + X1[I1];

            Y1[1] = "Hundred ";
            Y1[2] = "Thousand ";
            Y1[3] = (Index >= 3 ? "Million " : "Lac ");
            Y1[4] = (Index >= 3 ? "Billion " : "Crore ");
            Y1[5] = "Trillion ";
            Z1[1] = "Minnus ";
            Z1[2] = "Zero ";
            long N_1 = System.Convert.ToInt64(Math.Floor(XX1));
            string N_2 = XX1.ToString();
            while (!(N_2.Length == 0))
            {
                if (N_2.Substring(0, 1) == ".")
                    break;
                N_2 = N_2.Substring(1);
            }
            N_2 = (N_2.Length == 0 ? " " : N_2);
            switch (Index)
            {
                case 1:
                case 3:
                    N_2 = ((N_2.Substring(0, 1) == ".") ? ((string)(N_2.Substring(1) + "00000")).Substring(0, 5) : "00000");
                    break;
                case 2:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    N_2 = ((N_2.Substring(0, 1) == ".") ? ((string)(N_2.Substring(1) + "00000")).Substring(0, 2) : "00");
                    break;
            }
            string S_GN = (Math.Sign(N_1) == -1 ? Z1[1] : "");
            string Z1_ER = (N_1 == 0 ? Z1[2] : "");
            string N_O = Right("00000000000000000" + Math.Abs(N_1).ToString(), 17);
            string[] L = new string[100];
            switch (Index)
            {
                case 1:
                case 2:
                    L[0] = "";
                    L[1] = ((Convert.ToInt32(N_O.Substring(0, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(0, 1))] + Y1[1]);
                    L[2] = ((Convert.ToInt32(N_O.Substring(1, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(1, 2))] + Y1[4]);
                    L[3] = ((Convert.ToInt32(N_O.Substring(3, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(3, 2))] + Y1[3]);
                    L[4] = ((Convert.ToInt32(N_O.Substring(5, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(5, 2))] + Y1[2]);
                    L[5] = ((Convert.ToInt32(N_O.Substring(7, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(7, 1))] + Y1[1]);
                    L[6] = ((Convert.ToInt32(N_O.Substring(8, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(0, 10))) == 0 ? "" : Y1[4]) : X1[Int32.Parse(N_O.Substring(8, 2))] + Y1[4]);
                    L[7] = ((Convert.ToInt32(N_O.Substring(10, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(10, 2))] + Y1[3]);
                    L[8] = ((Convert.ToInt32(N_O.Substring(12, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(12, 2))] + Y1[2]);
                    L[9] = ((Convert.ToInt32(N_O.Substring(14, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(14, 1))] + Y1[1]);
                    L[10] = (Convert.ToInt32(N_O.Substring(15, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(15, 2))];
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    L[0] = ((Convert.ToInt32(N_O.Substring(0, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(0, 2))] + Y1[2]);
                    L[1] = ((Convert.ToInt32(N_O.Substring(2, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(2, 1))] + Y1[1]);
                    L[2] = ((Convert.ToInt32(N_O.Substring(3, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(2, 1)) == 0) ? "" : Y1[5]) : X1[Int32.Parse(N_O.Substring(3, 2))] + Y1[5]);
                    L[3] = ((Convert.ToInt32(N_O.Substring(5, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(5, 1))] + Y1[1]);
                    L[4] = ((Convert.ToInt32(N_O.Substring(6, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(5, 1)) == 0) ? "" : Y1[4]) : X1[Int32.Parse(N_O.Substring(6, 2))] + Y1[4]);
                    L[5] = ((Convert.ToInt32(N_O.Substring(8, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(8, 1))] + Y1[1]);
                    L[6] = ((Convert.ToInt32(N_O.Substring(9, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(8, 1)) == 0) ? "" : Y1[3]) : X1[Int32.Parse(N_O.Substring(9, 2))] + Y1[3]);
                    L[7] = ((Convert.ToInt32(N_O.Substring(11, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(11, 1))] + Y1[1]);
                    L[8] = ((Convert.ToInt32(N_O.Substring(12, 2)) == 0) ? ((Convert.ToInt32(N_O.Substring(11, 1)) == 0) ? "" : Y1[2]) : X1[Int32.Parse(N_O.Substring(12, 2))] + Y1[2]);
                    L[9] = ((Convert.ToInt32(N_O.Substring(14, 1)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(14, 1))] + Y1[1]);
                    L[10] = (Convert.ToInt32(N_O.Substring(15, 2)) == 0) ? "" : X1[Int32.Parse(N_O.Substring(15, 2))];
                    break;
            }
            string O = S_GN + Z1_ER + L[0] + L[1] + L[2] + L[3] + L[4] + L[5] + L[6] + L[7] + L[8] + L[9] + L[10];
            string[] M = new string[100];
            string Q_ = "";
            string P = "";

            switch (Index)
            {
                case 1:
                case 3:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2.Substring(0, 1))] : "");
                    M[2] = ((Convert.ToInt32(N_2) >= 1 & Convert.ToInt32(N_2.Substring(1)) >= 1) ? X1[Int32.Parse(N_2.Substring(1, 1))] : "");
                    M[3] = ((Convert.ToInt32(N_2) >= 1 & Convert.ToInt32(N_2.Substring(2)) >= 1) ? X1[Int32.Parse(N_2.Substring(2, 1))] : "");
                    M[4] = ((Convert.ToInt32(N_2) >= 1 & Convert.ToInt32(N_2.Substring(3 - 1)) >= 1) ? X1[Int32.Parse(N_2.Substring(3, 1))] : "");
                    M[5] = ((Convert.ToInt32(N_2) >= 1 & Convert.ToInt32(N_2.Substring(4)) >= 1) ? X1[Convert.ToInt32(N_2.Substring(4, 1))] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "Point " : "");
                    P = M[6] + M[1] + M[2] + M[3] + M[4] + M[5];
                    Q_ = O + P;
                    break;
                case 2:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Paisa " : "");
                    //M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Fils " : "");
                    P = M[6] + M[1];
                    Q_ = "( Taka " + O + P + "Only )";

                    // Q_ = "( " + O + P + "Only )";
                    break;
                case 4:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Cent " : "");
                    P = M[6] + M[1];
                    //Q_ = "( Dollar " + O + P + "Only )";
                    Q_ = "( " + O + P + "Only )";
                    break;
                case 5:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Paisa " : "");
                    P = M[6] + M[1];
                    Q_ = "( " + O + P + "Only )";
                    //Q_ = "( Rupee " + O + P + "Only )";
                    break;
                case 6:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Cent " : "");
                    P = M[6] + M[1];
                    Q_ = "( " + O + P + "Only )";
                    //Q_ = "( Euro " + O + P + "Only )";
                    break;
                case 7:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Pence " : "");
                    P = M[6] + M[1];
                    Q_ = "( " + O + P + "Only )";
                    //Q_ = "( Pound " + O + P + "Only )";
                    break;
                case 8:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Sen " : "");
                    P = M[6] + M[1];
                    Q_ = "( " + O + P + "Only )";
                    //Q_ = "( Yen " + O + P + "Only )";
                    break;
                case 9:
                    M[1] = ((Convert.ToInt32(N_2) >= 1) ? X1[Int32.Parse(N_2)] : "");
                    M[6] = ((Convert.ToInt32(N_2) > 0) ? "And Sen " : "");
                    P = M[6] + M[1];
                    //Q_ = "( RM " + O + P + "Only )";
                    Q_ = "( " + O + P + "Only )";
                    break;
            }
            return Q_;
        }
    }
}
