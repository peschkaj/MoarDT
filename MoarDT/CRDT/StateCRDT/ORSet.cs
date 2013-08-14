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
using MoarDT.Collections;

namespace MoarDT.CRDT.StateCRDT
{
    public class ORSet<T> : AbstractCRDT
    {
        internal MultiValueDictionary<T, string> addSet;
        internal MultiValueDictionary<T, string> removeSet;

        public ORSet (string actor = null, 
                      MultiValueDictionary<T, string> additions = null, 
                      MultiValueDictionary<T, string> removals = null)
        {
            Actor = actor ?? DefaultActorId();
            addSet = additions ?? new MultiValueDictionary<T, string>();
            removeSet = removals ?? new MultiValueDictionary<T, string>();
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

        public static ORSet<T> Merge(ORSet<T> left, ORSet<T> right, string actor = null)
        {


            throw new NotImplementedException();
        }
    }
}

