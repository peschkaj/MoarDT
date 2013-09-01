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
    public class PNCounter : AbstractCRDT, IEquatable<PNCounter>, IComparable, IComparable<PNCounter>
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

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
                return 1;

            if (obj is PNCounter)
                return CompareTo((PNCounter)obj);
            else
                throw new ArgumentException("obj is not PNCounter");
        }

        public int CompareTo(PNCounter Y)
        {
            return Compare(this, Y);
        }

        public static int Compare(PNCounter X, PNCounter Y)
        {
            /* In theory, equality is defined as:
             * let b = (∀i ∈ [0,n − 1] : X.P[i] ≤ Y.P[i] ∧ ∀i ∈ [0,n − 1] : X.N[i] ≤ Y.N[i])
             * 
             * I suspect we could extrapolate some delightfully complex comparison for
             * PNCounters, but the truth is that we really only compare about the Value
             */
            if (ReferenceEquals(X, null) || ReferenceEquals(Y, null))
                throw new NullReferenceException("Cannot compare null values with non-null values");

            return X.Value.CompareTo(Y.Value);
        }
    }
}

