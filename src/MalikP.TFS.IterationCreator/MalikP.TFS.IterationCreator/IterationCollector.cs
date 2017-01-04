//-------------------------------------------------------------------------------------------------
// <copyright file="IterationCollector.cs" company="MalikP.">
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
using System.Linq;
using System.Collections.Generic;
using MalikP.TFS.IterationCreator.Defaults;
using MalikP.TFS.Automation.Iteration;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MalikP.TFS.Automation;
using System.Runtime.CompilerServices;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;

namespace MalikP.TFS.IterationCreator
{

    public class IterationCollector
    {
        public List<string> IterationsToAssign = new List<string>();
        public List<string> IterationsToUnAssign = new List<string>();

        public void Clear()
        {
            IterationsToAssign.Clear();
            IterationsToUnAssign.Clear();
        }
    }

}