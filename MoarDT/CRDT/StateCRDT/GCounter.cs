//
//  Copyright 2013  Jeremiah Peschka
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
using System.Numerics;
using System.Collections.Generic;
using MoarDT.Extensions;

namespace MoarDT.CRDT.StateCRDT
{
    public class GCounter : AbstractCRDT
    {
        public DVVPair Version { get; private set; }
        internal Dictionary<DVVPair, BigInteger> Payload { get; set; }

        public GCounter(DVVPair dvv = null, ulong currentValue = default(ulong), 
                        Dictionary<DVVPair, BigInteger> counterContents = null,
                        DVVPair version = null)
        {
            Version = dvv ?? new DVVPair(DefaultActorId(), 0);
            Actor = Version.Actor;

            if (currentValue != default(ulong))
                Payload.Add(dvv, currentValue);

            Payload = counterContents ?? new Dictionary<DVVPair, BigInteger>();

//            Version = version ?? new DVVPair(Actor, 0);
        }

        public BigInteger Value 
        {
            get 
            {
                return Payload.Aggregate((BigInteger)0, (a, b) => a + b.Value);
            }
        }

        public static GCounter operator ++(GCounter gc)
        {
            return gc.Increment();
        }

        public GCounter Increment(int value = 1)
        {
            return Increment(Version, value);
        }

        public GCounter Increment(DVVPair actor, int value = 1)
        {
            Payload[actor] = Payload.ValueOrDefault(actor) + value;
            return this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = Payload.GetHashCode();
                result = (result * 397) ^ Actor.GetHashCode();
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is GCounter && Equals((GCounter)obj);
        }

        public bool Equals(GCounter other)
        {
            /* (∀i ∈ [0, n − 1] : X.P [i] ≤ Y.P [i]) */
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Payload.Equals(other.Payload))
                return true;

            return Payload.Count() == other.Payload.Count()
                   && Payload.Keys.Intersect(other.Payload.Keys).Count() == Payload.Count()
                   && Payload.Equals(other.Payload);
        }

        public static GCounter Merge(GCounter gca, GCounter gcb, DVVPair version = null, bool partialOrder = false)
        {
            /* let ∀i ∈ [0,n − 1] : Z.P[i] = max(X.P[i],Y.P[i]) */
            var keys = gca.Payload.Keys.Union(gcb.Payload.Keys);
            var newContents = new Dictionary<DVVPair, BigInteger>();

            foreach (var key in keys)
            {
                if (!gca.Payload.ContainsKey(key) && gcb.Payload.ContainsKey(key))
                    newContents[key] = gcb.Payload[key];
                else if (gca.Payload.ContainsKey(key) && !gcb.Payload.ContainsKey(key))
                    newContents[key] = gca.Payload[key];
                else
                    newContents[key] = gca.Payload[key] > gcb.Payload[key] ? gca.Payload[key] : gcb.Payload[key];
            }

            // TODO: Need to get the largest version from gca or gcb and increment by one
            return new GCounter(version ?? new DVVPair(DefaultActorId(), 0),
                                counterContents: newContents);

//            return new GCounter(clientId ?? DefaultActorId(), 
//                                counterContents: newContents);
        }
    }
}

