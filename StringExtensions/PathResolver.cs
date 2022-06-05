using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace StringExtensions
{
    public static class PathResolver
    {
        public static void SelectInExplorer(this string path)
        {
            //Process.Start("explorer.exe " + "/select, " + '\"' + filePath.Replace('/', '\\') + '\"');
            StartProcess("explorer.exe", null, "/select, " + path.Replace('/', '\\').Quote()
                );
        }
        public static Process StartProcess(FileInfo file, params string[] args) => StartProcess(file.FullName, file.DirectoryName, args);
        public static Process StartProcess(string file, string workDir = null, params string[] args)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = file;
            proc.Arguments = string.Join(" ", args);
            Debug.WriteLine($"{proc.FileName}, {proc.Arguments.ToString()}"); // Replace with your logging function
            if (workDir != null)
            {
                proc.WorkingDirectory = workDir;
                Debug.WriteLine("WorkingDirectory:", proc.WorkingDirectory); // Replace with your logging function
            }
            return Process.Start(proc);
        }
        public static string Quote(this string text)
        {
            return SurroundWith(text, "\"");
        }
        public static string SurroundWith(this string text, string surrounds)
        {
            return surrounds + text + surrounds;
        }

        public static string GetDirectoryOf(this string filepath)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filepath);
            return fileInfo.Directory.FullName;
        }

        /// <summary>
        /// Gets the parent directory of the file
        /// </summary>
        /// <param name="filepath">Path to the FILE to show the parent directory</param>
        public static void ExploreFolderOf(this string filepath)
        {
            System.Diagnostics.Process.Start("explorer.exe", filepath.GetDirectoryOf());
        }

        public static void ExplorePath(this string filepath)
        {
            System.Diagnostics.Process.Start("explorer.exe", filepath);
        }

        public static bool IsValidURL(this string URL)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(URL);
        }
        public static bool FileExistsOrValidURL(this String str)
        {
            return FileExists(str) || IsValidURL(str);
        }
        public static bool FileExists(this String str)
        {
            return System.IO.File.Exists(str);
        }
        public static string ExcludeExtension(this String withPossibleExtension)
        {
            var dotIndex = -1;
            int extensionWithDotLength = 0;
            for (int i = withPossibleExtension.Length - 1; i >= 0; i--)
            {

                extensionWithDotLength++;
                if (withPossibleExtension[i] == '.')
                {
                    if (extensionWithDotLength < 2)
                        return withPossibleExtension;
                    dotIndex = i;
                    break;
                }
                if (
                    (withPossibleExtension[i] < 'a' ||
                    withPossibleExtension[i] > 'z')
                    &&
                    (withPossibleExtension[i] > 'Z' ||
                    withPossibleExtension[i] < 'A')
                    &&
                    (withPossibleExtension[i] < '0' ||
                    withPossibleExtension[i] > '9')
                )
                    return withPossibleExtension;
            }

            int withoutExtensionLength = withPossibleExtension.Length - extensionWithDotLength;

            if (extensionWithDotLength > 5)
                return withPossibleExtension;
            //because there is no audio extension with more then 4 symbols

            return withPossibleExtension.Substring(0, withoutExtensionLength);
        } //ExcludeExtension

        public static bool Equals(string path1, string path2)
        {
            return new Uri(path1) == new Uri(path2);
        }
    }
}
