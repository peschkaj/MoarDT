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
using MoarDT.Extensions;

namespace MoarDT.CRDT.Causality
{
    public class VectorClock : IComparable, IComparable<VectorClock>, IEquatable<VectorClock>
    {
        private enum Occurs
        {
            Before = -1,
            Concurrently = 0,
            After = 1
        }

        public long Timestamp { get; private set; }
        
        private List<VVPair> _versions;

        public VectorClock() : this(Environment.TickCount) { }

        public VectorClock(long timestamp, List<VVPair> versions = null)
        {
            Timestamp = timestamp;
            _versions = versions ?? new List<VVPair>() { new VVPair(VVPair.DefaultActorId(), 0) };
        }

        public void IncrementVersion(int actor)
        {
            IncrementVersion(actor, Environment.TickCount);
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
                    break;
            }

            // Is there a way to perform the above as LINQ?
            // var version = _versions.Where(v => v.Actor == actor || v.Actor > actor).FirstOrDefault();

            if (found)
                _versions[index]++;
            else if (index < _versions.Count - 1)
                _versions.Insert(0, new VVPair(actor, 1));
            else
                _versions.Add(new VVPair(actor, 1));
        }

        public VectorClock Increment(int actor)
        {
            return Increment(actor, Environment.TickCount);
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
            if (ReferenceEquals(null, other))
                return false;

            if (_versions.Count != other._versions.Count)
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

        public VectorClock Merge(VectorClock other)
        {
            var newClock = new VectorClock();

            var allVersions = _versions.FullOuterJoin(other._versions, 
                                                      v1 => v1.Actor, 
                                                      v2 => v2.Actor, 
                                                      (v1, v2, Actor) => new {v1, v2})
                                       .ToList();

            foreach (var v in allVersions)
            {
                if (v.v1 != null && v.v2 == null)
                    newClock._versions.Add(v.v1.Clone());
                else if (v.v1 == null && v.v2 != null)
                    newClock._versions.Add(v.v2.Clone());
                else
                {
                    if (v.v1.Actor == v.v2.Actor)
                        newClock._versions.Add(new VVPair(v.v1.Actor, Math.Max(v.v1.Counter, v.v2.Counter)));
                    else if (v.v1.Actor < v.v2.Actor)
                        newClock._versions.Add(v.v1.Clone());
                    else
                        newClock._versions.Add(v.v2.Clone());
                }
            }

            return newClock;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) 
                return 1;

            if (obj is VectorClock)
                return CompareTo((VVPair)obj);
            else
                throw new ArgumentException("obj is not a VVPair");
        }

        public int CompareTo(VectorClock other)
        {
            return Compare(this, other);
        }

        public static int Compare(VectorClock left, VectorClock right)
        {
            if (left == null || right == null)
                throw new ArgumentNullException("Can't compare null VectorClocks to real things!");

            bool leftBigger = false;
            bool rightBigger = false;
            int leftPos = 0;
            int rightPos = 0;

            while (leftPos < left._versions.Count && rightPos < right._versions.Count)
            {
                var leftVV = left._versions[leftPos];
                var rightVV = right._versions[rightPos];

                if (leftVV.Actor == rightVV.Actor)
                {
                    if (leftVV.Counter > rightVV.Counter)
                        leftBigger = true;
                    else
                        rightBigger = true;

                    leftPos++;
                    rightPos++;
                }
                else if (leftVV.Actor > rightVV.Actor)
                {
                    // left is missing a version from right. 
                    // keep walking the right hand side until we see the flip side
                    rightBigger = true;
                    rightPos++;
                }
                else
                {
                    // since left != right AND right > left that means right has a version
                    // that left hasn't see. Increment the left counter and keep walking
                    leftBigger = true;
                    leftPos++;
                }
            }

            // check for stragglers
            if (leftPos < left._versions.Count)
                leftBigger = true;
            else if (rightPos < right._versions.Count)
                rightBigger = true;

            // if both vclocks are "not bigger", the one on the left wins.
            // viva el reloj vector del proletariado!
            if (!leftBigger && !rightBigger)
                return (int)Occurs.Before;
            else if (leftBigger && !rightBigger)
                return (int)Occurs.After;
            else if (!leftBigger && rightBigger)
                return (int)Occurs.Before;
            else
                return (int)Occurs.Concurrently;
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

        public static bool operator <(VectorClock left, VectorClock right)
        {
            return Compare(left, right) < 0;
        }

        public static bool operator >(VectorClock left, VectorClock right)
        {
            return Compare(left, right) > 0;
        }

        public static bool operator ==(VectorClock left, VectorClock right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VectorClock left, VectorClock right)
        {
            return Compare(left, right) != 0;
        }

        public static bool operator >=(VectorClock left, VectorClock right)
        {
            return Compare(left, right) >= 0;
        }

        public static bool operator <=(VectorClock left, VectorClock right)
        {
            return Compare(left, right) <= 0;
        }
    }
}