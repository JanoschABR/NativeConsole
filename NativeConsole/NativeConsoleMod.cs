using Elements.Core;
using Microsoft.Win32.SafeHandles;
using Pastel;
using ResoniteModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Mods.NativeConsole {
    public class NativeConsoleMod : ResoniteMod {
        public override string Name => "NativeConsole";
        public override string Author => "JanoschR";
        public override string Version => "1.0.0";


        #region Mod Configuration
        public static ModConfiguration config;

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<bool> cfgEnable
            = new ModConfigurationKey<bool>(
                "enabled",
                "Enable", () => true
        );

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<bool> cfgUseColor
            = new ModConfigurationKey<bool>(
                "useColor",
                "Use Color?", () => true
        );

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<string> cfgColorInfo
            = new ModConfigurationKey<string>(
                "colorInfo",
                "Info Message Color", () => "1DB4CF"
        );

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<string> cfgColorWarning
            = new ModConfigurationKey<string>(
                "colorWarning",
                "Warning Message Color", () => "CF9C1D"
        );

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<string> cfgColorError
            = new ModConfigurationKey<string>(
                "colorError",
                "Error Message Color", () => "CF1D1D"
        );
        #endregion

        #region Variables
        public static string colorInfo;
        public static string colorWarning;
        public static string colorError;
        #endregion

        #region Native Windows Kernel32 Stuff
        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport(
            "kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        private const int STD_OUTPUT_HANDLE = -11;
        #endregion

        #region Native Windows Console Stuff
        public static void SetupNativeConsole () {
            AllocConsole();
            SetupNativeConsoleStream();
        }

        // Windows fuckery to redirect
        // the Console.Write output to our new console window
        public static void SetupNativeConsoleStream () {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle fileHandle = new SafeFileHandle(handle, true);

            FileStream fileStream = new FileStream(fileHandle, FileAccess.Write);
            Encoding encoding = Encoding.GetEncoding(Encoding.ASCII.CodePage);

            StreamWriter output = new StreamWriter(fileStream, encoding);
            output.AutoFlush = true;

            Console.SetOut(output);
        }
        #endregion


        public override void OnEngineInit() {
            config = GetConfiguration();
            config.Save(true);

            // Check if the mod should continue or not
            if (!config.GetValue(cfgEnable)) {
                return;
            }

            // Pastel's way of handling color is already bad enough,
            // don't need to make it worse with constant config reading
            colorInfo = config.GetValue(cfgColorInfo);
            colorWarning = config.GetValue(cfgColorWarning);
            colorError = config.GetValue(cfgColorError);

            // Check if color should be used and configure Pastel accordingly
            if (!config.GetValue(cfgUseColor)) {
                Pastel.ConsoleExtensions.Disable();
            }

            // Setup the native console
            SetupNativeConsole();

            // Add the UniLog output to the console
            UniLog.OnLog += (msg) => {
                Console.WriteLine($"[I] {msg}".Pastel(colorInfo));
            };
            UniLog.OnWarning += (msg) => {
                Console.WriteLine($"[W] {msg}".Pastel(colorWarning));
            };
            UniLog.OnError += (msg) => {
                Console.WriteLine($"[E] {msg}".Pastel(colorError));
            };
        }
    }
}
