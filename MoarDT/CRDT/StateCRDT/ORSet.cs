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
using System.Threading.Tasks;
using MoarDT.Collections;
using MoarDT.CRDT.Causality;

namespace MoarDT.CRDT.StateCRDT
{
    public class ORSet<T> : AbstractCRDT, IEquatable<ORSet<T>>
    {
        internal MultiValueDictionary<T, int> addSet;
        internal MultiValueDictionary<T, int> removeSet;

        public ORSet (MultiValueDictionary<T, int> additions = null,
                      MultiValueDictionary<T, int> removals = null)
        : this(DefaultActorId(), additions, removals) { } 


        public ORSet (int actor, 
                      MultiValueDictionary<T, int> additions = null, 
                      MultiValueDictionary<T, int> removals = null)
        {
            VectorClock = new VectorClock();
            Actor = actor;
            addSet = additions ?? new MultiValueDictionary<T, int>();
            removeSet = removals ?? new MultiValueDictionary<T, int>();
        }

        public ORSet<T> Add(T element)
        {
            addSet.Add(element, Actor);
            return this;
        }

        public ORSet<T> Remove(T element)
        {
            removeSet.Add(element, Actor);
            return this;
        }

        public ORSet<T> Clear()
        {
            var keys = addSet.Keys;

            foreach (var k in keys)
            {
                removeSet.Add(k, addSet[k]);
            }

            return this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is ORSet<T> && Equals((ORSet<T>)obj);
        }

        public bool Equals(ORSet<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return addSet == other.addSet 
                   && removeSet == other.removeSet;
        }

        public static ORSet<T> Merge(ORSet<T> left, ORSet<T> right, string actor = null)
        {
            var orSet = new ORSet<T>();

            Parallel.ForEach(left.addSet.Keys, k => orSet.Add(k));
            Parallel.ForEach(right.addSet.Keys, k => orSet.Add(k));
            Parallel.ForEach(left.removeSet.Keys, k => orSet.Remove(k));
            Parallel.ForEach(right.removeSet.Keys, k => orSet.Remove(k));

            return orSet;
        }

        public override int GetHashCode()
        {
            unchecked {
                var result = addSet.GetHashCode();
                result = (result * 397) ^ removeSet.GetHashCode();
                return result;
            }
        }
    }
}

