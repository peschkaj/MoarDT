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
    public class DottedVersionVector
    {
        private SortedSet<VVPair> _versionVector;
        public VVPair Dot { get; private set; }
        public string Actor { get; private set; }

        public DottedVersionVector (string actor, VVPair dot = null, SortedSet<VVPair> VclockEntries = null)
        {
            Actor = actor;
            Dot = dot ?? new VVPair(actor, 0UL);
            _versionVector = VclockEntries ?? new SortedSet<VVPair>();
        }

        public ulong Counter()
        {
            return Counter(Actor);
        }

        public ulong Counter(string actor)
        {
            // muck through SortedSet and find the max value for the current actor
            var vvList = _versionVector.Where(pair => pair.Actor == actor);
            return vvList.Max().Counter;
        }



        public static DottedVersionVector operator ++(DottedVersionVector vector)
        {
            return vector.Increment();
        }

        public DottedVersionVector Increment()
        {
            return Increment(Actor);
        }

        public DottedVersionVector Increment(string actor)
        {
            if (Dot != null)
                _versionVector.Add(Dot);

            var vv = new VVPair(actor, Counter(actor) + 1);
            Dot = vv;

            return this;
        }

        private SortedSet<VVPair> Ancestors(VVPair vv)
        {
            var vvList = _versionVector.Where(pair => pair.Actor == vv.Actor);

            if (vvList == null || vvList.Count() == 0)
                return new SortedSet<VVPair>();

            return new SortedSet<VVPair>(vvList);
        }

        public bool DescendedFrom(DottedVersionVector other)
        {
            if (Equals(other))
                return true;



            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return _versionVector.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is DottedVersionVector && Equals((DottedVersionVector)obj);
        }

        public bool Equals(DottedVersionVector other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Dot == other.Dot && _versionVector.Equals(other._versionVector);
        }

        /// <summary>
        /// Combine all vclocks in the current VersionVector into their
        /// least possible descendant and return a new VersionVector.
        /// </summary>
        public DottedVersionVector Merge()
        {
            throw new NotImplementedException();
        }

        public static DottedVersionVector Update(DottedVersionVector dvv, VVPair vv)
        {
            var newDvv = new DottedVersionVector(dvv.Actor);
            // find any ancestors of vv
            var ancestors = dvv.Ancestors(vv);


            if (ancestors.Count > 0)
            {
                // if there are ancestors, promote vv to Dot
                newDvv.Dot = vv;

                // prune ancestors
                newDvv._versionVector = dvv.Merge()._versionVector;
            }
            // otherwise, add vv to _versionVector

            throw new NotImplementedException();
        }
    }
}

