//Created by Starflash Studios, 2017-19
//This work is licensed under the GNU General Public License V3.0
//The GNU General Public License is a free, copyleft license for software and other kinds of works.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

public static class NeoLib {

    /// <summary>
    /// Shorthand for string.IsNullOrEmpty(value);
    /// </summary>
    /// <param name="value">The string in question</param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string value) {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsElevated {
        get { return WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid); }
    }    

    public static void Show(this Window value, Window r) {
        r.Hide();
        value.Top = r.Top;
        value.Left = r.Left;
        value.Show();
        Type t = value.GetType();
        try {
            MethodInfo method = t.GetMethod("OnShow");
            method.Invoke(value, null);
        } catch { } // No OnShow() method
    }

    public static BitmapImage BitmapImage(this Bitmap src) {
        MemoryStream ms = new MemoryStream();
        src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        BitmapImage image = new BitmapImage();
        image.BeginInit();
        ms.Seek(0, SeekOrigin.Begin);
        image.StreamSource = ms;
        image.EndInit();
        return image;
    }

    public static double Round(this double value, int digits = 0, MidpointRounding method = MidpointRounding.AwayFromZero) => Math.Round(value, digits, method);
    public static float Round(this float value, int digits = 0, MidpointRounding method = MidpointRounding.AwayFromZero) => (float)Math.Round(value, digits, method);

    public static double Ceil(this double value) => Math.Ceiling(value);
    public static float Ceil(this float value) => (float)Math.Ceiling(value);
    public static double Floor(this double value) => Math.Floor(value);
    public static float Floor(this float value) => (float)Math.Floor(value);

    public static int ParseInt(this string value) => int.Parse(value);
    public static long ParseLong(this string value) => long.Parse(value);
    public static float ParseFloat(this string value) => float.Parse(value);
    public static double ParseDouble(this string value) => double.Parse(value);

    public static string ReverseSubstring(this string str, int index) => str.Reverse().Substring(str.Length - index).Reverse();
    public static string ReverseSubstring(this string str, int index, int length) => str.Reverse().Substring(str.Length - index, length).Reverse();
    public static string Reverse(this string str) {
        return string.IsNullOrEmpty(str) ? string.Empty : new string(str.ToCharArray().Reverse().ToArray());
    }

    public static string SentenceCase(this string value) {
        string lowerCase = value.ToLower();
        Regex r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);
        return r.Replace(lowerCase, s => s.Value.ToUpper());
    }

    public static void Wait(int time) {
        System.Threading.Thread.Sleep(time);
    }

    public static string Truncate(this string value, int maxChars) {
        return value.Length <= maxChars ? value : value.Substring(0, maxChars) + (char)0x2026 + "...";
    }

    public static string Truncate(this string value, int maxChars, string endWord) {
        if (value.Length > maxChars || !value.IsEmptyOrNull()) {
            string ending = value.Substring(value.LastIndexOf(endWord));
            if (value.Length > maxChars - ending.Length) {
                return string.Format("{0}...{1}", value.Substring(0, maxChars - ending.Length), ending);
            }
        }
        return value;
    }

    [Flags]
    public enum TruncateOptions {
        None = 0x0,
        FinishWord = 0x1,
        AllowLastWordToGoOverMaxLength = 0x2,
        IncludeEllipsis = 0x4
    }

    /// <summary>
    /// Shorthand for string.IsNullOrEmpty(value);
    /// </summary>
    /// <param name="value">The string in question</param>
    /// <returns></returns>
    public static bool IsEmptyOrNull(this string value) {
        return string.IsNullOrEmpty(value);
    }

    public static string EncodeAscii(this string value) {
        string pattern = "[^ -~]+";
        Regex reg_exp = new Regex(pattern);
        return reg_exp.Replace(value, "");
    }

    public static string EncodeWindows(this string value) => value.Trim(Path.GetInvalidFileNameChars());
    public static string Nominal(this string value) => '"' + value.Trim('"') + '"';
    public static string UnNominal(this string value) {
        if (value[0] == '"') { value.Remove(0, 1); }
        if (value[value.Length - 1] == '"') { value.Remove(value.Length - 1, 1); }
        return value;
    }

    public static void WriteDebugLines(this List<object> value) {
        foreach (object obj in value) {
            System.Diagnostics.Debug.WriteLine(obj.ToString());
        }
    }

    public static void WriteDebugLines(this object[] value) {
        foreach (object obj in value) {
            System.Diagnostics.Debug.WriteLine(obj.ToString());
        }
    }

    public static bool ToBool(this bool? value) {
        return value == true;
    }

    #region General Extensions
    public static bool Prompt() {
        string read = Console.ReadLine();
        if (read == null || read.Length < 1) { return false; } else {
            return read.ToLower()[0] == "y"[0];
        }
    }

    public static void Wait(ConsoleKey key = ConsoleKey.Enter) {
        while (!(Console.KeyAvailable && Console.ReadKey(true).Key == key)) {
            // do something
        }
        return;
    }

    public static bool Prompt(out string c) { c = Console.ReadLine(); return c.ToLower()[0] == "y"[0]; }

    #endregion

    #region List/Array Management
    /// <summary>
    /// Returns a generated string (with newlines) from the entered array of strings
    /// </summary>
    /// <param name="array">The entered array of strings</param>
    /// <returns></returns>
    public static string ToString(string[] array) {
        string gen = array[0];
        for (int i = 0; i < array.Length - 1; i++) { gen += "\n" + array[i + 1]; }
        return gen;
    }

    ///<summary>
    ///Returns a random object from the given list of objects
    ///<paramref name="list">The given list</paramref>
    ///</summary>
    public static T Random<T>(this List<T> list) {
        Random random = new Random();
        try { return list[random.Next(0, list.Count)]; } catch { return list[0]; }
    }

    /// <summary>
    /// Converts the given array and returns it as a list
    /// </summary>
    /// <typeparam name="T">The returned list</typeparam>
    /// <param name="array">The given array</param>
    /// <returns></returns>
    public static List<T> ToList<T>(this T[] array) {
        List<T> list = new List<T>();
        foreach (T t in array) { list.Add(t); }
        return list;
    }

    /// <summary>
    /// Combines two arrays
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="a">The array that will be combined with "b"</param>
    /// <param name="b">The array to be combined with "a"</param>
    /// <returns></returns>
    public static T[] Combine<T>(this T[] a, T[] b) {
        List<T> nA = a.ToList();
        nA.AddRange(b.ToList());
        a = nA.ToArray();
        return nA.ToArray();
    }

    /// <summary>
    /// Returns the combination of two arrays
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="a">The first array</param>
    /// <param name="b">The second array</param>
    /// <returns></returns>
    public static T[] SCombine<T>(this T[] a, T[] b) {
        List<T> nA = a.ToList();
        nA.AddRange(b.ToList());
        return nA.ToArray();
    }

    /// <summary>
    /// Returns a generated string (with newlines) from the entered list of strings
    /// </summary>
    /// <param name="list">The entered list of strings</param>
    /// <returns></returns>
    public static string GenerateString(List<string> list) {
        string gen = list[0];
        for (int i = 0; i < list.Count - 1; i++) { gen += "\n" + list[i + 1]; }
        return gen;
    }
    #endregion

    #region Value Truncating
    /// <summary>
    /// Returns the truncated version of "value" to "dec" places
    /// </summary>
    /// <param name="value">The input value to truncate</param>
    /// <param name="dec">The amount of decimal places</param>
    /// <returns></returns>
    public static float STruncate(this float value, int decimals) {
        return (int)(value * (float)Math.Pow(10, decimals)) / (float)Math.Pow(10, decimals);
    }

    /// <summary>
    /// Truncates the given value to the entered amount of decimal places
    /// </summary>
    /// <param name="value">The given value to be Truncated</param>
    /// <param name="decimals">The amount of decimal places</param>
    /// <returns></returns>
    public static float Truncate(this float value, int decimals) {
        float final = (int)(value * (float)Math.Pow(10, decimals)) / (float)Math.Pow(10, decimals);
        value = final;
        return final;
    }
    #endregion

    #region Value Forcing
    /// <summary>
    /// Forces the given variable (val) positive
    /// </summary>
    /// <param name="val">The given variable</param>
    /// <returns>The positive version of val</returns>
    public static float Positive(this float val) {
        return val < 0 ? -val : val;
    }

    /// <summary>
    /// Forces the given variable (val) positive
    /// </summary>
    /// <param name="val">The given variable</param>
    /// <returns>The positive version of val</returns>
    public static int Positive(this int val) {
        return val < 0 ? -val : val;
    }

    /// <summary>
    /// Forces the given variable (val) negative
    /// </summary>
    /// <param name="val">The given variable</param>
    /// <returns>The positive version of val</returns>
    public static float Negative(this float val) {
        return val > 0 ? -val : val;
    }

    /// <summary>
    /// Forces the given variable (val) negative
    /// </summary>
    /// <param name="val">The given variable</param>
    /// <returns>The positive version of val</returns>
    public static int Negative(this int val) {
        return val > 0 ? -val : val;
    }
    #endregion

    public static DialogResult BrowseForFile(out string path, string initialDirectory = "C:\\", string title = "Browse for Files", bool checkFile = true, bool checkPath = true, string extension = "txt", string filter = "Text files (*.txt)|*.txt") {
        OpenFileDialog dialog = new OpenFileDialog {
            InitialDirectory = initialDirectory,
            Title = title,

            CheckFileExists = checkFile,
            CheckPathExists = checkPath,

            DefaultExt = extension,
            Filter = filter
        };

        path = dialog.FileName;
        return dialog.ShowDialog();
    }

    public static OpenFileDialog BrowseForFile(out DialogResult result, string initialDirectory = "C:\\", string title = "Browse for Files", bool checkFile = true, bool checkPath = true, string extension = "txt", string filter = "Text files (*.txt)|*.txt") {
        OpenFileDialog dialog = new OpenFileDialog {
            InitialDirectory = initialDirectory,
            Title = title,

            CheckFileExists = checkFile,
            CheckPathExists = checkPath,

            DefaultExt = extension,
            Filter = filter
        };

        result = dialog.ShowDialog();
        return dialog;
    }
}