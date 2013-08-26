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

namespace MoarDT
{
    // dvvsets need to include the sibling
    public class VVPair : IComparable
    {
        public int Actor { get; private set; }
        public ulong Counter { get; private set; }

        public VVPair(int actor, ulong counter)
        {
            Actor = actor;
            Counter = counter;
        }

        public static VVPair operator ++(VVPair obj)
        {
            obj.Counter++;
            return obj;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) 
                return 1;

            var dot = obj as VVPair;

            if (dot != null)
            {
                if (Actor == dot.Actor)
                    return Counter.CompareTo(dot.Counter);
                else
                    return Actor.CompareTo(dot.Actor);
            }
            else
            {
                throw new ArgumentException("obj is not a VVPair");
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = Actor.GetHashCode();
                result = (result * 397) ^ Counter.GetHashCode();
                return result;
            }
        }

        public override bool Equals (object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj is VVPair && Equals(obj);
        }

        public bool Equals(VVPair other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Actor == other.Actor && Counter == other.Counter;
        }
    }
}

