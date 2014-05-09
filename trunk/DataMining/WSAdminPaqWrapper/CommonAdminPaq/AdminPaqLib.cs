using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace CommonAdminPaq
{
    public class AdminPaqLib
    {
        /* **************************************************
         * AdminPaq SDK to allow AdminPaq database queries. *
         * **************************************************
         */
        [DllImport("DBFWIN32")]
        public static extern int dbLogIn(string UserName, string filePath);
        [DllImport("DBFWIN32")]
        public static extern void dbLogOut(int hDbc);
        [DllImport("DBFWIN32")]
        public static extern int dbCmdExec(int hDbc, string CmdStr);
        [DllImport("DBFWIN32")]
        public static extern int dbFieldChar([MarshalAs(UnmanagedType.I4)] int hDbc, [MarshalAs(UnmanagedType.LPStr)] string FileName, [MarshalAs(UnmanagedType.I4)] int nField, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszChar, [MarshalAs(UnmanagedType.I4)] int nlen);
        [DllImport("DBFWIN32")]
        public static extern int dbFieldInt(int hDbc, string FileName, int nField, ref int lpInt);
        [DllImport("DBFWIN32")]
        public static extern int dbFieldLong(int hDbc, string fileName, int nField, ref int lpLong);
        [DllImport("DBFWIN32")]
        public static extern int dbFieldDouble(int hDbc, string FileName, int nField, ref double lpLong);
        [DllImport("DBFWIN32")]
        public static extern int dbGet(int hDbc, string FileName, string TagName, string Key);
        [DllImport("DBFWIN32")]
        public static extern int dbGetNoLock(int hDbc, string FileName, string TagName, string Key);
        [DllImport("DBFWIN32")]
        public static extern long dbGetNoLockBDE(long hDbc, string fileName, string TagName, string Key);
        [DllImport("DBFWIN32")]
        public static extern int dbGetTopNoLock([MarshalAs(UnmanagedType.I4)]int hDbc, [MarshalAs(UnmanagedType.LPStr)]string FileName, [MarshalAs(UnmanagedType.LPStr)]string TagName);
        [DllImport("DBFWIN32")]
        public static extern long dbGetNextNoLock(long hDbc, string fileName, string TagName, string PrevKey);
        [DllImport("DBFWIN32")]
        public static extern long dbKey(long hDbc, string fileName, string TagName, string Key, long nlen);
        [DllImport("DBFWIN32")]
        public static extern long dbKeySet(long hDbc, string fileName, string TagName, string Key, long nlen);
        [DllImport("DBFWIN32")]
        public static extern long dbResult(long hDbc, ref TDBError dbError);
        [DllImport("DBFWIN32")]
        public static extern int dbSkip([MarshalAs(UnmanagedType.I4)]int hDbc, [MarshalAs(UnmanagedType.LPStr)]string FileName, [MarshalAs(UnmanagedType.LPStr)]string TagName, [MarshalAs(UnmanagedType.I4)]int Direction);
        [DllImport("DBFWIN32")]
        public static extern long dbGetRecCount(long hDbc, string fileName, ref long lpLong);
        [DllImport("DBFWIN32")]
        public static extern long dbFieldLen(long hDbc, string fileName, int nField, ref long lpFieldLen);
        [DllImport("DBFWIN32")]
        public static extern void dbResult([MarshalAs(UnmanagedType.I4)]int hDbc, out TDBError dbError);

        /* **************************************************************************************************
         * WINDOWS API TO SET DLL DIRECTORY (Adding AdminPaq libraries to the knowledge of our application. *
         * **************************************************************************************************
         */
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        private string reportDir;
        private string dataDir;

        public string ReportDirectory
        {
            get { return reportDir; }
            set { reportDir = value; }
        }

        public string DataDirectory
        {
            get { return dataDir; }
            set { dataDir = value; }
        }

        public void SetDllFolder() {
            string adminPaqBaseDir;
            bool allowedDirectory;
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Computación en Acción, SA CV\AdminPAQ");
            adminPaqBaseDir = rk.GetValue("DirectorioBase").ToString();
            allowedDirectory = SetDllDirectory(adminPaqBaseDir);

            reportDir = rk.GetValue("DirectorioReportes").ToString();
            dataDir = rk.GetValue("DirectorioDatos").ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class TDBError {
        [MarshalAs(UnmanagedType.I4)]
        public int nErrorSeverity;
        [MarshalAs(UnmanagedType.I4)]
        public int nErrorCode;
        [MarshalAs(UnmanagedType.I4)]
        public int nSubErrorCode;
        public char[] szErrorMessage;
    }
}
