//-------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="MalikP.">
//   Copyright (c) 2016-2017, Peter Malik.
//   Authors: Peter Malik (MalikP.) (peter.malik@outlook.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using MalikP.TFS.IterationCreator.Defaults.Providers;
using MalikP.TFS.IterationCreator.Defaults.Generators;
using MalikP.TFS.IterationCreator;
using MalikP.TFS.Automation.Iteration;
using MalikP.TFS.Automation.Iteration.Settings;
using System.Collections.Generic;
using MalikP.TFS.Automation;
using System.Linq;

namespace MalikP.TFS.ExecutionConsole
{
    public static class Program
    {
        const string Command_Server = "-s:";
        const string Command_Collection = "-c:";
        const string Command_Project = "-p:";
        const string Command_TargetYear = "-t:";

        public static void Main(string[] args)
        {
            var settings = ParseArgs(args);
            if (settings != null)
            {
                var uri = new Uri(settings.ProjectCollectionUri);
                var year = settings.TargetYear;
                var projectName = settings.ProjectName;

                ITeamFoundationIterationGenerator yearGenerator = new YearlySeparatedIterationGenerator(year);
                ITeamFoundationIterationGenerator monthGenerator = new MonthlySeparatedIterationGenerator(year);

                ITeamFoundationIterationDataProvider yearProvider = new TeamFoundationIterationProvider(yearGenerator, null);
                ITeamFoundationIterationDataProvider monthProvider = new TeamFoundationIterationProvider(monthGenerator, yearProvider);

                var projects = new List<string>();

                ITeamFoundationIterationCreator creator = new TeamFoundationIterationCreator(uri, "");
                if (string.Equals(projectName, "ALL", StringComparison.InvariantCultureIgnoreCase))
                {
                    projects = (creator as ITeamFoundationProjectNameGetter)?.GetProjectNames()
                                                                             .ToList();
                }
                else
                {
                    projects.Add(projectName);
                }

                if (projects != null)
                {
                    foreach (string projectNameItem in projects)
                    {
                        Console.WriteLine($"{Divider(131, '-')}");
                        creator = new TeamFoundationIterationCreator(uri, projectNameItem);
                        if (creator.IsInitialized)
                            creator.Process(yearProvider);

                        yearProvider.Reset();
                        monthProvider.Reset();
                    }
                }
            }

                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue ...");
                    Console.ReadKey();
        }

        private static TeamFoundationDateIterationGenerationSettings ParseArgs(string[] args)
        {
            TeamFoundationDateIterationGenerationSettings resultSettings = null;
            if (args != null && args.Length == 4)
            {
                resultSettings = new TeamFoundationDateIterationGenerationSettings()
                {
                    ServerUri = args[0],
                };

                foreach (var item in args)
                {
                    if (item.StartsWith(Command_Server, StringComparison.InvariantCultureIgnoreCase))
                    {
                        resultSettings.ServerUri = item.Replace(Command_Server, string.Empty);
                    }
                    else if (item.StartsWith(Command_Collection, StringComparison.InvariantCultureIgnoreCase))
                    {
                        resultSettings.ProjectCollectionName = item.Replace(Command_Collection, string.Empty);
                    }
                    else if (item.StartsWith(Command_Project, StringComparison.InvariantCultureIgnoreCase))
                    {
                        resultSettings.ProjectName = item.Replace(Command_Project, string.Empty);
                    }
                    else if (item.StartsWith(Command_TargetYear, StringComparison.InvariantCultureIgnoreCase))
                    {
                        resultSettings.TargetYear = int.Parse(item.Replace(Command_TargetYear, string.Empty));
                    }
                }
            }
            else
            {
                WriteHelp();
            }

            return resultSettings;
        }

        private static void WriteHelp()
        {
            var foreColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{Divider(131, '#')}");
            Console.WriteLine($"{Divider()}| Options |{Divider()}");

            Console.WriteLine($"To define variable 'Team Foundation Server Uri' use this argument: '{Command_Server}'");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{"".PadLeft(10, ' ')}Example: '{ExampleServerCommand}'");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"To define variable 'Team Foundation Server Collection Name' use this argument: '{Command_Collection}'");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{"".PadLeft(10, ' ')}Example: '{ExampleCollectionCommand}'");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"To define variable 'Team Foundation Server Project Name' use this argument: '{Command_Project}'");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{"".PadLeft(10, ' ')}Example: '{ExampleAllProjectCommand}'");
            Console.WriteLine($"{"".PadLeft(10, ' ')}Example: '{ExampleProjectCommand}'");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"To define variable 'Target Year' use this argument: '{Command_TargetYear}'");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{"".PadLeft(10, ' ')}Example: '{ExampleServerCommand}'");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"{Divider(131, '-')}");
            Console.WriteLine($"Full command example:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{"".PadLeft(10, ' ')} {typeof(Program).Namespace.Normalize()}.exe {ExampleServerCommand} {ExampleCollectionCommand} {ExampleProjectCommand} {ExampleYearCommand}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{Divider(131, '-')}");

            Console.WriteLine($"{Divider(131, '#')}");

            Console.ForegroundColor = foreColor;
        }

        static string Divider(int count = 60, char divider = '-') => string.Empty.PadLeft(count, divider);
        static string ExampleYearCommand => $"{Command_TargetYear}2017";
        static string ExampleProjectCommand => $"{Command_Project}MyProjectName";
        static string ExampleAllProjectCommand => $"{Command_Project}ALL";
        static string ExampleCollectionCommand => $"{Command_Collection}DefaultCollection";
        static string ExampleServerCommand => $"{Command_Server}https://tfs.domain.com/tfs";
    }
}
