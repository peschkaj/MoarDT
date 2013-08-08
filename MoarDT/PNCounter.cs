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

namespace MoarDT.CRDT.StateCRDT
{
    public class PNCounter
    {
        // TODO: implement ISerializable
        internal GCounter positive;
        internal GCounter negative;
        private ulong _currentValue;

        public ulong Value {
            get { 
                Merge();
                return _currentValue;
            }
        }

        public PNCounter (ulong currentValue = default(ulong))
        {
            positive = new GCounter();
            negative = new GCounter();
            _currentValue = currentValue;
        }

        public static PNCounter operator ++(PNCounter pnc)
        {
            return pnc.Increment(1);
        }

        public PNCounter Increment(ulong value = 1)
        {
            positive.Increment(value);
            return this;
        }

        public PNCounter Increment(IEnumerable<ulong> collection)
        {
            positive.Increment(collection);
            return this;
        }

        public static PNCounter operator --(PNCounter pnc)
        {
            return pnc.Decrement(1);
        }

        public PNCounter Decrement(ulong value = 1)
        {
            negative.Increment(value);
            return this;
        }

        public PNCounter Decrement(IEnumerable<ulong> collection)
        {
            negative.Increment(collection);
            return this;
        }

        private void Merge()
        {
            var t = _currentValue;

            t += positive.Value;
            t -= negative.Value;

            _currentValue = t;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(ulong))
                return false;

            return Equals((PNCounter)obj);
        }

        public bool Equals(PNCounter other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            unchecked {
                var result = _currentValue.GetHashCode();
                result = (result * 397) ^ positive.GetHashCode();
                result = (result * 397) ^ negative.GetHashCode();
                return result;
            }
        }
    }
}

