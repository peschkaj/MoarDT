//
//  Copyright 2013  Brent Ozar Unlimited
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Linq;
using System.Collections.Generic;
using MoarDT.Extensions;

namespace MoarDT
{


    public class VersionVector
    {
        // vclock node, counter
        private SortedDictionary<string, int> _entries;

        public VersionVector (SortedDictionary<string, int> VclockEntries = null)
        {
            _entries = VclockEntries ?? new SortedDictionary<string, int>();
        }

        public SortedDictionary<string,int> ActiveNodes {
            get { return _entries; }
        }

        public int Counter(string node)
        {
            return _entries.ValueOrDefault(node);
        }

        public VersionVector IncrementCounter(string node)
        {
            _entries[node] = _entries.ValueOrDefault(node) + 1;
            return this;
        }

        public bool DescendedFrom(VersionVector other)
        {
            if (Equals(other))
                return true;

            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return _entries.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is VersionVector && Equals((VersionVector)obj);
        }

        public bool Equals(VersionVector other)
        {
            return ActiveNodes.Equals(other.ActiveNodes);
        }

        /// <summary>
        /// Combine all vclocks in the current VersionVector into their
        /// least possible descendant and return a new VersionVector.
        /// </summary>
        public VersionVector Merge()
        {
            throw new NotImplementedException();
        }
    }
}

