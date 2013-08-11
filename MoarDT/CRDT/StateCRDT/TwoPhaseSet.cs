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

namespace MoarDT.CRDT.StateCRDT
{
    public class TwoPhaseSet<T> : AbstractCRDT
    {
        internal GSet<T> addSet;
        internal GSet<T> removeSet;

        public TwoPhaseSet (string actor = null, HashSet<T> additions = null, HashSet<T> removals = null)
        {
            Actor = actor ?? DefaultActorId();
            addSet = new GSet<T>(actor, additions ?? new HashSet<T>());
            removeSet = new GSet<T>(actor, removals ?? new HashSet<T>());
        }

        public static TwoPhaseSet<T> Merge(TwoPhaseSet<T> tpsa, TwoPhaseSet<T> tpsb, string actor = null)
        {
            return new TwoPhaseSet<T>(actor)
            {
                addSet = GSet<T>.Merge(tpsa.addSet, tpsb.addSet),
                removeSet = GSet<T>.Merge(tpsb.removeSet, tpsb.removeSet)
            };
        }

        public HashSet<T> Value 
        {
            get 
            {
                var r = new HashSet<T>(addSet.Payload);

                r.RemoveWhere(removeSet.Contains);
                return r;
            }
        }

        public TwoPhaseSet<T> Add(T value)
        {
            addSet.Add(value);

            return this;
        }

        public TwoPhaseSet<T> Remove(T value)
        {
            if (addSet.Contains(value))
                removeSet.Add(value);

            return this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is TwoPhaseSet<T> && Equals((TwoPhaseSet<T>)obj);
        }

        public bool Equals(TwoPhaseSet<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return addSet.Payload.Count == other.addSet.Payload.Count && 
                   removeSet.Payload.Count == other.removeSet.Payload.Count &&
                   addSet.Payload.IsProperSubsetOf(other.addSet.Payload) &&
                   removeSet.Payload.IsProperSubsetOf(other.removeSet.Payload);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = addSet.GetHashCode();
                result = (result * 397) ^ removeSet.GetHashCode();
                result = (result * 397) ^ Actor.GetHashCode();
                return result;
            }
        }
    }
}

