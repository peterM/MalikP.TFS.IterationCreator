//-------------------------------------------------------------------------------------------------
// <copyright file="TeamFoundationIterationDataProviderBase.cs" company="MalikP.">
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

namespace MalikP.TFS.IterationCreator.Defaults.Providers
{
    public abstract class TeamFoundationIterationDataProviderBase : ITeamFoundationIterationDataProvider, ITeamFoundationIterationUriUpdatable, IResetable
    {
        protected TeamFoundationIterationDataProviderBase() { }

        public ITeamFoundationIterationDataProvider ChildDataProvider { get; protected set; }

        public virtual bool IsValid => NameGenerator.IsValid && PeriodGenerator.IsValid;


        public ITeamFoundationIterationGenerator IterationGenerator { get; protected set; }
        public INameGenerator NameGenerator => IterationGenerator as INameGenerator;

        public ITeamFoundationIterationDataProvider ParentDataProvider { get; protected set; }

        public IPeriodGenerator PeriodGenerator => IterationGenerator as IPeriodGenerator;
        public virtual bool Generate()
        {
            IterationGenerator.Generate();
            return IsValid;
        }

        public TeamFoundationIteration GetIteration()
        {
            var iteration = IterationGenerator.GetIteration();

            if (ChildDataProvider != null)
                iteration.Assign = false;

            return iteration;
        }

        public ITeamFoundationIterationDataProvider MostParentProvider()
        {
            if (ParentDataProvider != null)
                return ParentDataProvider?.MostParentProvider();

            return this;
        }

        public void Reset()
        {
            IterationGenerator.Reset();
        }

        public void SetChild(ITeamFoundationIterationDataProvider child)
        {
            ChildDataProvider = child;
        }

        public void UpdateIterationUri(string uri)
        {
            (IterationGenerator as ITeamFoundationIterationUriUpdatable)?.UpdateIterationUri(uri);
        }
    }
}
