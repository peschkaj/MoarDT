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
    public class GSet<T> : AbstractCRDT
    {
        internal HashSet<T> Payload = new HashSet<T>();
        public string Actor { get; private set; }

        public GSet(string actor = null, HashSet<T> contents = null)
        {
            Actor = actor ?? DefaultActorId();

            if (contents != null)
                Payload = contents;
        }

        public GSet<T> Add(T value)
        {
            Payload.Add(value);

            return this;
        }

        public bool Contains(T value)
        {
            return Payload.Contains(value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is GSet<T> && Equals((GSet<T>)obj);
        }

        public bool Equals(GSet<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Payload.Equals(other.Payload))
                return true;

            return Payload.Count() == other.Payload.Count()
                   && Payload.IsProperSubsetOf(other.Payload);
        }

        public static GSet<T> Merge(GSet<T> gsa, GSet<T> gsb, string actor = null)
        {
            return new GSet<T>(actor ?? DefaultActorId(),
                               new HashSet<T>(gsa.Payload.Union(gsb.Payload)));
        }

        public override int GetHashCode()
        {
            return Payload.GetHashCode();
        }
    }
}

