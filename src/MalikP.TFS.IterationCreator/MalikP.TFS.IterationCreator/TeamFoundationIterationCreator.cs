//-------------------------------------------------------------------------------------------------
// <copyright file="TeamFoundationIterationCreator.cs" company="MalikP.">
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

using MalikP.TFS.Automation.Iteration;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MalikP.TFS.Automation;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;

namespace MalikP.TFS.IterationCreator
{
    public class TeamFoundationIterationCreator : ITeamFoundationIterationCreator, ITeamFoundationProjectNameGetter
    {
        public const string Iteration = "Iteration";

        Uri CollectionUri;

        public TeamFoundationIterationCreator(Uri tfsServerCollectionUri, string projectName)
        {
            CollectionUri = tfsServerCollectionUri;
            Initialize();

            if (!string.IsNullOrEmpty(projectName))
            {
                TeamProject = StructureService.ListProjects()
                                              .SingleOrDefault(project => string.Equals(project.Name, projectName, StringComparison.InvariantCultureIgnoreCase));

                BasePath = Path.Combine(ProjectName, Iteration);
            }
        }

        public bool IsInitialized { get; private set; }

        public string ProjectName => TeamProject.Name;

        protected string BasePath { get; set; }

        protected ICommonStructureService4 StructureService => TfsCollection.GetService<ICommonStructureService4>();

        protected ProjectInfo TeamProject { get; set; }

        protected TfsTeamService TeamService => TfsCollection.GetService<TfsTeamService>();

        protected TeamSettingsConfigurationService TeamSettingsService => TfsCollection.GetService<TeamSettingsConfigurationService>();

        IterationCollector Collector { get; } = new IterationCollector();

        TfsTeamProjectCollection TfsCollection { get; set; }

        public IEnumerable<string> GetProjectNames() => StructureService.ListAllProjects()
                                                                        .Select(d => d.Name)
                                                                        .ToList();

        public virtual void IndentBasePath(string name)
        {
            BasePath = Path.Combine(BasePath, name);
        }

        public void Initialize()
        {
            TfsCollection = new TfsTeamProjectCollection(CollectionUri);
            TfsCollection.EnsureAuthenticated();
            IsInitialized = true;
        }

        public virtual void Process(ITeamFoundationIterationDataProvider provider)
        {
            Collector.Clear();
            ProcessInternally(provider);
            AssignToDefaultTeam(Collector.IterationsToAssign, Collector.IterationsToUnAssign);
        }

        protected void AssignToDefaultTeam(IEnumerable<string> iterationsToAssign, IEnumerable<string> iterationsToUnassign)
        {
            var team = TeamService.QueryTeams(TeamProject.Uri);
            var selectedTeam = team.FirstOrDefault();
            var teamId = selectedTeam.Identity
                                     .TeamFoundationId;

            var teamSettings = TeamSettingsService.GetTeamConfigurations(new[] { teamId })
                                                  .FirstOrDefault()
                                                  .TeamSettings;

            List<string> iterations = teamSettings.IterationPaths
                                                  .ToList();

            var iterationsToAdd = iterationsToAssign.Select(d => d.Replace($"\\{Iteration}", string.Empty))
                                                    .ToList();
            var iterationsToRemove = iterationsToUnassign.Select(d => d.Replace($"\\{Iteration}", string.Empty))
                                                         .ToList();

            foreach (var item in iterationsToAdd)
            {
                var iterationsToRemoveByDefault = iterations.Where(iteration => iteration.StartsWith(item, StringComparison.CurrentCultureIgnoreCase))
                                                            .ToList();
                iterationsToRemoveByDefault.Remove(item);
                iterationsToRemove.AddRange(iterationsToRemoveByDefault);
            }

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            iterationsToAdd.ForEach(it => Console.WriteLine($"Assigning iteration to Team - {selectedTeam.Name}: {it} "));

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            iterationsToRemove.ForEach(it => Console.WriteLine($"UnAssigning iteration from Team - {selectedTeam.Name}: {it} "));
            Console.ForegroundColor = color;

            iterations.AddRange(iterationsToAdd);
            iterations = iterations.Distinct()
                                   .ToList();

            iterationsToRemove.ForEach(iteration => IterationContains(iteration, iterations));

            teamSettings.IterationPaths = iterations.ToArray();

            var tfvs = teamSettings.TeamFieldValues;
            if (tfvs == null || tfvs.Length <= 0)
            {
                var tfv = new TeamFieldValue
                {
                    IncludeChildren = true,
                    Value = ProjectName
                };

                teamSettings.TeamFieldValues = new[] { tfv };
            }
            else
            {
                tfvs.First()
                    .IncludeChildren = true;
            }

            TeamSettingsService.SetTeamSettings(teamId, teamSettings);
        }

        private static void IterationContains(string iteration, List<string> iterations)
        {
            if (iterations.Contains(iteration))
            {
                iterations.Remove(iteration);
            }
        }

        protected virtual void ProcessInternally(ITeamFoundationIterationDataProvider provider)
        {
            if (provider == null)
            {
                return;
            }

            NodeInfo rootNode = null;
            while (provider.Generate())
            {
                var iteration = provider.GetIteration();

                var basePath = BasePath;

                if (iteration.Assign)
                {
                    Collector.IterationsToAssign.Add(Path.Combine(basePath, iteration.Name));
                }
                else
                {
                    Collector.IterationsToUnAssign.Add(Path.Combine(basePath, iteration.Name));
                }

                var color = Console.ForegroundColor;
                try
                {
                    rootNode = StructureService.GetNodeFromPath(Path.Combine(basePath, iteration.Name));

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{ProjectName}] - Iteration with name '{iteration.Name}' exists. Iteration will not be created.");
                    Console.ForegroundColor = color;

                    continue;
                }
                catch (CommonStructureSubsystemException)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{ProjectName}] - Iteration with name '{iteration.Name}' do not exist. Iteration will be created.");
                    Console.ForegroundColor = color;

                    rootNode = StructureService.GetNodeFromPath(basePath);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{ProjectName}] - Exception occured: {ex}");
                    Console.ForegroundColor = color;
                }

                var uri = StructureService.CreateNode(iteration.Name, rootNode.Uri, iteration.StartDate, iteration.EndDate);
                (provider as ITeamFoundationIterationUriUpdatable)?.UpdateIterationUri(uri);
            }

            if (rootNode != null)
            {
                var lastValidName = provider.NameGenerator
                                            .GetLastName();

                IndentBasePath(lastValidName);
            }

            ProcessInternally(provider.ChildDataProvider);
        }
    }
}