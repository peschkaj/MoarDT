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
using MoarDT.CRDT.Causality;

namespace MoarDT.CRDT.StateCRDT
{
    public class PNCounter : AbstractCRDT
    {
        internal GCounter P;
        internal GCounter N;

        public BigInteger Value 
        {
            get 
            { 
                return P.Value - N.Value; 
            }
        }

        public PNCounter(BigInteger value) : this(DefaultActorId(), value) { }

        public PNCounter(int actor, BigInteger value)
        {
            Actor = actor;

            P = new GCounter(Actor, value.PositiveOrDefault());
            N = new GCounter(Actor, value.NegativeOrDefault());
        }

        public PNCounter(Dictionary<int, BigInteger> p = null, 
                         Dictionary<int, BigInteger> n = null) :
            this(DefaultActorId(), p, n) { }

        public PNCounter(int actor,
                         Dictionary<int, BigInteger> p = null, 
                         Dictionary<int, BigInteger> n = null)
        {
            Actor = actor;

            P = new GCounter(Actor, counterContents: p);
            N = new GCounter(Actor, counterContents: n);
        }

        public static PNCounter operator ++(PNCounter pnc)
        {
            return pnc.Increment();
        }

        public PNCounter Increment(int value = 1)
        {
            return Increment(Actor, value);
        }

        public PNCounter Increment(int actor, int value = 1)
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
            return Decrement(Actor, value);
        }

        public PNCounter Decrement(int actor, int value = 1)
        {
            N.Increment(actor, value);
            return this;
        }

        public static PNCounter Merge(PNCounter pna, PNCounter pnb)
        {
            return Merge(pna, pnb, DefaultActorId());
        }

        public static PNCounter Merge(PNCounter pna, PNCounter pnb, int actor)
        {
            return new PNCounter(actor)
                {
                    P = GCounter.Merge(pna.P, pnb.P, actor),
                    N = GCounter.Merge(pna.N, pnb.N, actor)
                };
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is PNCounter && Equals((PNCounter)obj);
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
                result = (result * 397) ^ Actor.GetHashCode();
                return result;
            }
        }

        public static PNCounter Prune(PNCounter pnc)
        {
            return Prune(pnc, DefaultActorId());
        }

        public static PNCounter Prune(PNCounter pnc, int actor)
        {
            return new PNCounter(actor, pnc.Value);
        }
    }
}

