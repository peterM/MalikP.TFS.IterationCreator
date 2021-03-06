//-------------------------------------------------------------------------------------------------
// <copyright file="TeamFoundationIterationGeneratorBase.cs" company="MalikP.">
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
using MalikP.TFS.Automation.Iteration;
using MalikP.Core.Interfaces;

namespace MalikP.TFS.IterationCreator.Defaults.Generators
{
    public abstract class TeamFoundationIterationGeneratorBase : IResetable, INameGenerator, IPeriodGenerator, ITeamFoundationIterationGenerator, ITeamFoundationIterationUriUpdatable
    {
        protected TeamFoundationIterationGeneratorBase() { }

        public abstract bool IsValid { get; }

        public abstract DateTime? IterationEndDate { get; }

        public abstract DateTime? IterationStartDate { get; }

        protected TeamFoundationIteration Iteration { get; set; }

        protected String LastName { get; set; }

        public virtual bool Generate()
        {
            if (Iteration != null)
            {
                BeforeNewGeneration(Iteration);
            }

            GenerateInternal();

            if (IsValid)
            {
                Iteration = new TeamFoundationIteration
                {
                    Name = GetName(),
                    StartDate = IterationStartDate,
                    EndDate = IterationEndDate,
                    Assign = true
                };
            }

            return IsValid;
        }

        public virtual TeamFoundationIteration GetIteration() => Iteration;

        public abstract string GetLastName();

        public abstract string GetName();

        public abstract void Reset();

        public void UpdateIterationUri(string uri) => Iteration.Uri = uri;

        protected virtual void BeforeNewGeneration(TeamFoundationIteration iteration)
        {
        }

        protected abstract void GenerateInternal();
    }
}