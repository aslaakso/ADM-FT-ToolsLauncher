/*
 *
 *  Certain versions of software and/or documents (“Material”) accessible here may contain branding from
 *  Hewlett-Packard Company (now HP Inc.) and Hewlett Packard Enterprise Company.  As of September 1, 2017,
 *  the Material is now offered by Micro Focus, a separately owned and operated company.  Any reference to the HP
 *  and Hewlett Packard Enterprise/HPE marks is historical in nature, and the HP and Hewlett Packard Enterprise/HPE
 *  marks are the property of their respective owners.
 * __________________________________________________________________
 * MIT License
 *
 * © Copyright 2012-2019 Micro Focus or one of its affiliates..
 *
 * The only warranties for products and services of Micro Focus and its affiliates
 * and licensors (“Micro Focus”) are set forth in the express warranty statements
 * accompanying such products and services. Nothing herein should be construed as
 * constituting an additional warranty. Micro Focus shall not be liable for technical
 * or editorial errors or omissions contained herein.
 * The information contained herein is subject to change without notice.
 * ___________________________________________________________________
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HpToolsLauncher.Properties;

namespace HpToolsLauncher
{
    public enum TestStorageType
    {
        Alm,
        AlmLabManagement,
        FileSystem,
        LoadRunner,
        Unknown
    }

    class Program
    {
        private static readonly Dictionary<string, string> argsDictionary = new Dictionary<string, string>();

        //[MTAThread]
        static void Main(string[] args)
        {
            ConsoleQuickEdit.Disable();
            if (args.Count() == 0 || args.Contains("/?"))
            {
                ShowHelp();
                return;
            }
            // show version?
            if (args[0] == "-v" || args[0] == "-version" || args[0] == "/v")
            {
                Console.WriteLine(Assembly.GetEntryAssembly().GetName().Version.ToString());
                Environment.Exit(0);
                return;
            }

            for (int i = 0; i < args.Count(); i = i + 2)
            {
                string key = args[i].StartsWith("-") ? args[i].Substring(1) : args[i];
                string val = i + 1 < args.Count() ? args[i + 1].Trim() : String.Empty;
                argsDictionary[key] = val;
            }
            string paramFileName, runtype;
            string failOnTestFailed = "N";
            argsDictionary.TryGetValue("runtype", out runtype);
            argsDictionary.TryGetValue("paramfile", out paramFileName);
            TestStorageType enmRuntype = TestStorageType.Unknown;

            if (!Enum.TryParse<TestStorageType>(runtype, true, out enmRuntype))
                enmRuntype = TestStorageType.Unknown;

            if (string.IsNullOrEmpty(paramFileName))
            {
                ShowHelp();
                return;
            }

            ShowTitle();
            ConsoleWriter.WriteLine(Resources.GeneralStarted);

            var apiRunner = new Launcher(failOnTestFailed, paramFileName, enmRuntype);

            apiRunner.Run();
        }

        private static void ShowTitle()
        {
            AssemblyName assembly = Assembly.GetEntryAssembly().GetName();
            Console.WriteLine("Micro Focus Automation Tools - {0} {1} ", assembly.Name, assembly.Version.ToString());
            Console.WriteLine();
        }

        private static void ShowHelp()
        {
            AssemblyName assembly = Assembly.GetEntryAssembly().GetName();

            ShowTitle();
            Console.WriteLine("Usage: {0} -v|-version", assembly.Name);
            Console.WriteLine("  Show program version");
            Console.WriteLine();
            Console.Write("Usage: {0} -paramfile ", assembly.Name);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("<a file in key=value format> ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("  Execute tests with parameters");
            Console.WriteLine();
            Console.WriteLine("  -paramfile is required and the parameters file may contain the following mandatory fields:");
            Console.WriteLine("\t# Basic parameters");
            Console.WriteLine("\trunType=FileSystem|Alm");
            Console.WriteLine("\tresultsFilename=<result-file>");
            Console.WriteLine();
            Console.WriteLine("\t# ALM parameters");
            Console.WriteLine("\talmServerUrl=http(s)://<server>:<port>/qcbin");
            Console.WriteLine("\talmUserName=<username>");
            Console.WriteLine("\talmPasswordBasicAuth=<base64-password>");
            Console.WriteLine("\talmDomain=<domain>");
            Console.WriteLine("\talmProject=<project>");
            Console.WriteLine("\talmRunMode=RUN_LOCAL|RUN_REMOTE|RUN_PLANNED_HOST");
            Console.WriteLine("\tTestSet<i:1-to-n>=<test-set-path>|<Alm-folder>");
            Console.WriteLine();
            Console.WriteLine("\t# File System parameters");
            Console.WriteLine("\tTest<i:1-to-n>=<test-folder>|<path-to-test-folders>|<.lrs file>|<.mtb file>|<.mtbx file>");
            Console.WriteLine();
            Console.WriteLine("\t# Mobile Center parameters");
            Console.WriteLine("\tMobileHostAddress=http(s)://<server>:<port>");
            Console.WriteLine("\tMobileUserName=<username>");
            Console.WriteLine("\tMobilePasswordBasicAuth=<base64-password>");
            Console.WriteLine("\tMobileTenantId=<mc-tenant-id>");
            Console.WriteLine();
            Console.WriteLine("\t# Parallel Runner parameters");
            Console.WriteLine("\tparallelRunnerMode=true|false");
            Console.WriteLine("\tParallelTest<i>Env<j>=<key>:<value>[,...]");
            Console.WriteLine();
            Console.WriteLine("* For the details of the entire parameter list, see the online README on GitHub.");
            Environment.Exit((int)Launcher.ExitCodeEnum.Failed);
        }
    }
}
