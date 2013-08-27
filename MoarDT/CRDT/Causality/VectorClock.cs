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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoarDT.CRDT.Causality
{
    public class VectorClock : IEquatable<VectorClock>
    {
        // TODO implement vclock pruning

        public long Timestamp { get; private set; }
        
        private List<VVPair> _versions;

        public VectorClock() : this(Environment.TickCount) { }

        public VectorClock(long timestamp, List<VVPair> versions = null)
        {
            Timestamp = timestamp;
            _versions = versions ?? new List<VVPair>() { new VVPair(VVPair.DefaultActorId(), 0) };
        }

        public void IncrementVersion(int actor, long timestamp)
        {
            Timestamp = timestamp;

            bool found = false;
            int index;

            for (index = 0; index < _versions.Count; index++)
            {
                if (_versions[index].Actor == actor)
                {
                    found = true;
                    break;
                } 

                if (_versions[index].Actor > actor)
                {
                    break;
                }
            }

            // Is there a way to perform the above as LINQ?
            // var version = _versions.Where(v => v.Actor == actor || v.Actor > actor).FirstOrDefault();

            if (found)
            {
                _versions[index]++;
            }
            else if (index < _versions.Count - 1)
            {
                _versions.Insert(0, new VVPair(actor, 1));
            }
            else
            {
                _versions.Add(new VVPair(actor, 1));
            }
        }

        public VectorClock Increment(int actor, long timestamp)
        {
            var vclock = this.Clone();
            vclock.IncrementVersion(actor, timestamp);
            return vclock;
        }

        public VectorClock Clone()
        {
            return new VectorClock(Timestamp, _versions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is VectorClock && Equals((VectorClock)obj);
        }

        public bool Equals(VectorClock other)
        {
            if (this._versions.Count != other._versions.Count)
                return false;

            /* A potentially better solution is listed at 
             * http://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order
             * 
             * This will required addtional time to implement and, frankly, the below 
             * falls into Works For Me territory.
             */
            return Enumerable.SequenceEqual(_versions.OrderBy(t => t), other._versions.OrderBy(t => t));
        }

        public override int GetHashCode() {
            return _versions.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("{timestamp = ");
            sb.Append(Timestamp);
            sb.Append(", versions = [");
            sb.Append(String.Join(",", _versions));
            sb.Append("]}");

            return sb.ToString();
        }
    }
}

