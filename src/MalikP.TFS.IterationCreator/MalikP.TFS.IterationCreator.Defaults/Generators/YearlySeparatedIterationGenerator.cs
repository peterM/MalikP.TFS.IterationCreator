//-------------------------------------------------------------------------------------------------
// <copyright file="YearlySeparatedIterationGenerator.cs" company="MalikP.">
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

namespace MalikP.TFS.IterationCreator.Defaults.Generators
{
    public class YearlySeparatedIterationGenerator : TeamFoundationIterationGeneratorBase
    {
        public YearlySeparatedIterationGenerator(int year)
        {
            Year = year;
        }

        public YearlySeparatedIterationGenerator() : this(DateTime.Now.Year) { }

        public override bool IsValid => _isValid == null || _isValid.Value;

        public override DateTime? IterationEndDate => new DateTime(Year, 12, 31);

        public override DateTime? IterationStartDate => new DateTime(Year, 1, 1);

        protected int Year { get; private set; }

        private bool? _isValid { get; set; }

        public override string GetLastName() => LastName;

        public override string GetName()
        {
            var name = $"Year {Year}";
            LastName = name;
            return name;
        }

        public override void Reset() => _isValid = null;

        protected override void GenerateInternal() => _isValid = _isValid == null;
    }
}