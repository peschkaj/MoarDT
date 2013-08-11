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

namespace MoarDT.CRDT.StateCRDT
{
    public class PNCounter : AbstractCRDT
    {
        internal GCounter P;
        internal GCounter N;
        private readonly string _actor;

        public BigInteger Value 
        {
            get 
            { 
                return P.Value - N.Value; 
            }
        }

        public PNCounter (string actor = null,
                          Dictionary<string, BigInteger> p = null, 
                          Dictionary<string, BigInteger> n = null)
        {
            _actor = String.IsNullOrEmpty(actor) ? DefaultActorId() : actor;
            P = new GCounter(_actor, counterContents: p);
            N = new GCounter(_actor, counterContents: n);
        }

        public static PNCounter operator ++(PNCounter pnc)
        {
            return pnc.Increment();
        }

        public PNCounter Increment(int value = 1)
        {
            return Increment(_actor, value);
        }

        public PNCounter Increment(string actor, int value = 1)
        {
            P.Increment(actor, value);
            return this;
        }

        public static PNCounter operator --(PNCounter pnc)
        {
            return pnc.Decrement();
        }

        public PNCounter Decrement(int value = 1)
        {
            return Decrement(_actor, value);
        }

        public PNCounter Decrement(string actor, int value = 1)
        {
            N.Increment(actor, value);
            return this;
        }

        public static PNCounter Merge(PNCounter pna, PNCounter pnb, string clientId = null)
        {
            return new PNCounter(clientId ?? DefaultActorId())
                {
                    P = GCounter.Merge(pna.P, pnb.P, clientId ?? DefaultActorId()),
                    N = GCounter.Merge(pna.N, pnb.N, clientId ?? DefaultActorId())
                };
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == typeof(PNCounter) && Equals((PNCounter)obj);
        }

        public bool Equals(PNCounter other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return P.Equals(other.P) && N.Equals(other.N);
        }

        public override int GetHashCode()
        {
            unchecked {
                var result = P.GetHashCode();
                result = (result * 397) ^ N.GetHashCode();
                return result;
            }
        }
    }
}

