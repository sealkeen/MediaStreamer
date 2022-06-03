using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Win32DotNetWorkarounds
{
    public static class Win32CMDResolution
    {
        /// <summary>
        /// https://www.pinvoke.net/default.aspx/shell32/CommandLineToArgvW.html
        /// </summary>
        /// <param name="unsplitArgumentLine"></param>
        /// <returns></returns>
        public static string[] SplitArgs(this string unsplitArgumentLine)
        {
            int numberOfArgs;
            IntPtr ptrToSplitArgs;
            string[] splitArgs;

            ptrToSplitArgs = CommandLineToArgvW(unsplitArgumentLine, out numberOfArgs);

            // CommandLineToArgvW returns NULL upon failure.
            if (ptrToSplitArgs == IntPtr.Zero)
                throw new ArgumentException("Unable to split argument.", new Win32Exception());

            // Make sure the memory ptrToSplitArgs to is freed, even upon failure.
            try
            {
                splitArgs = new string[numberOfArgs];

                // ptrToSplitArgs is an array of pointers to null terminated Unicode strings.
                // Copy each of these strings into our split argument array.
                for (int i = 0; i < numberOfArgs; i++)
                    splitArgs[i] = Marshal.PtrToStringUni(
                        Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));

                return splitArgs;
            }
            finally
            {
                // Free memory obtained by CommandLineToArgW.
                LocalFree(ptrToSplitArgs);
            }
        }

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LocalFree(IntPtr hMem);

        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string GetEscapedCommandLine()
        {
            StringBuilder sb = new StringBuilder();
            bool gotQuote = false;
            foreach (var c in Environment.CommandLine.Reverse())
            {
                if (c == '"')
                    gotQuote = true;
                else if (gotQuote && c == '\\')
                {
                    // double it
                    sb.Append('\\');
                }
                else
                    gotQuote = false;

                sb.Append(c);
            }

            return Reverse(sb.ToString());
        }

        /// <summary>
        /// C-like argument parser
        /// </summary>
        /// <param name="commandLine">Command line string with arguments. Use Environment.CommandLine</param>
        /// <returns>The args[] array (argv)</returns>
        public static string[] CreateArgs(this string commandLine)
        {
            StringBuilder argsBuilder = new StringBuilder(commandLine);
            bool inQuote = false;

            // Convert the spaces to a newline sign so we can split at newline later on
            // Only convert spaces which are outside the boundries of quoted text
            for (int i = 0; i < argsBuilder.Length; i++)
            {
                if (argsBuilder[i].Equals('"'))
                {
                    inQuote = !inQuote;
                }

                if (argsBuilder[i].Equals(' ') && !inQuote)
                {
                    argsBuilder[i] = '\n';
                }
            }

            // Split to args array
            string[] args = argsBuilder.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Clean the '"' signs from the args as needed.
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = ClearQuotes(args[i]);
            }

            return args;
        }

        /// <summary>
        /// Cleans quotes from the arguments.<br/>
        /// All signle quotes (") will be removed.<br/>
        /// Every pair of quotes ("") will transform to a single quote.<br/>
        /// </summary>
        /// <param name="stringWithQuotes">A string with quotes.</param>
        /// <returns>The same string if its without quotes, or a clean string if its with quotes.</returns>
        private static string ClearQuotes(this string stringWithQuotes)
        {
            int quoteIndex;
            if ((quoteIndex = stringWithQuotes.IndexOf('"')) == -1)
            {
                // String is without quotes..
                return stringWithQuotes;
            }

            // Linear sb scan is faster than string assignemnt if quote count is 2 or more (=always)
            StringBuilder sb = new StringBuilder(stringWithQuotes);
            for (int i = quoteIndex; i < sb.Length; i++)
            {
                if (sb[i].Equals('"'))
                {
                    // If we are not at the last index and the next one is '"', we need to jump one to preserve one
                    if (i != sb.Length - 1 && sb[i + 1].Equals('"'))
                    {
                        i++;
                    }

                    // We remove and then set index one backwards.
                    // This is because the remove itself is going to shift everything left by 1.
                    sb.Remove(i--, 1);
                }
            }

            return sb.ToString();
        }
    }
}
