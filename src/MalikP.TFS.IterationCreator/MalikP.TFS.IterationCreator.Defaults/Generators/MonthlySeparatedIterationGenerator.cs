//-------------------------------------------------------------------------------------------------
// <copyright file="MonthlySeparatedIterationGenerator.cs" company="MalikP.">
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
using MalikP.TFS.Automation.Iteration;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace MalikP.TFS.IterationCreator.Defaults.Generators
{
    public class MonthlySeparatedIterationGenerator : YearlySeparatedIterationGenerator
    {
        public MonthlySeparatedIterationGenerator(int year) : base(year) { }

        public MonthlySeparatedIterationGenerator() { }

        public override string GetName()
        {
            var name = new DateTime(Year, CurrentMonth, 1).ToString("MMMM", CultureInfo.InvariantCulture);
            LastName = name;
            return name;
        }

        public override bool IsValid
        {
            get
            {
                return CurrentMonth > 0 && CurrentMonth <= 12;
            }
        }

        public override DateTime? IterationEndDate
        {
            get
            {
                var daysInMonth = GetDaysInMonth();
                return new DateTime(Year, CurrentMonth, daysInMonth);
            }
        }

        protected int GetDaysInMonth()
        {
            return DateTime.DaysInMonth(Year, CurrentMonth);
        }

        public override DateTime? IterationStartDate
        {
            get
            {
                return new DateTime(Year, CurrentMonth, 1);
            }
        }
        protected int CurrentMonth { get; private set; }


        protected override void GenerateInternal()
        {
            CurrentMonth++;
        }

        public override void Reset()
        {
            CurrentMonth = 0;
        }
    }
}