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
using System.Collections.Generic;
using MoarDT.Extensions;

namespace MoarDT.CRDT.StateCRDT
{
    public class GCounter : AbstractCRDT
    {
        internal Dictionary<string, ulong> Payload { get; set; }
        private readonly string _clientId;

        public GCounter(string clientId = null, ulong currentValue = default(ulong), Dictionary<string, ulong> counterContents = null)
        {
            _clientId = clientId ?? DefaultClientId();

            if (currentValue != default(ulong))
                Payload.Add(_clientId, currentValue);

            Payload = counterContents ?? new Dictionary<string, ulong>();
        }

        public ulong Value 
        {
            get 
            {
                return Payload.Aggregate(0UL, (a, b) => a + b.Value);
            }
        }

        public static GCounter operator ++(GCounter gc)
        {
            return gc.Increment();
        }

        public GCounter Increment(ulong item = 1)
        {
            Payload[_clientId] = Payload.ValueOrDefault(_clientId) + item;
            return this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Payload.GetHashCode();
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

        public static GCounter Merge(GCounter gca, GCounter gcb, string clientId = null)
        {
            /* let ∀i ∈ [0,n − 1] : Z.P[i] = max(X.P[i],Y.P[i]) */
            var keys = gca.Payload.Keys.Union(gcb.Payload.Keys);
            var newContents = new Dictionary<string, ulong>();

            foreach (var key in keys)
            {
                if (!gca.Payload.ContainsKey(key) && gcb.Payload.ContainsKey(key))
                    newContents[key] = gcb.Payload[key];
                else if (gca.Payload.ContainsKey(key) && !gcb.Payload.ContainsKey(key))
                    newContents[key] = gca.Payload[key];
                else
                    newContents[key] = Math.Max(gca.Payload[key], gcb.Payload[key]);
            }

            return new GCounter(clientId ?? DefaultClientId(), 
                                counterContents: newContents);
        }
    }
}

