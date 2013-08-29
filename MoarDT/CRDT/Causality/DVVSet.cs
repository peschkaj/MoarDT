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

namespace MoarDT.CRDT.Causality
{
    /// <summary>
    /// A DVV Set as described in Dotted Version Vectors: Logical Clocks for Optimistic Replication
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class DVVSet<T>
    {

        public SortedSet<VVPair> Versions { get; internal set; }

        /// <summary>
        /// The most recently seen server side version.
        /// </summary>
        /// <value>The dot.</value>
        public VVPair Dot { get; internal set; }

        public DVVSet()
        {
            Versions = new SortedDictionary<VVPair, T>();
        }

        public DVVSet(DVVSet<T> history)
        {
            Versions = history.Versions;
            Dot = history.Dot;
        }

        /// <summary>
        /// Retrieves the current value from this DVVSet
        /// </summary>
        /// <returns>The value.</returns>
        /// <remarks>
        /// <para>When a client wants to read a key/value, we extract the global causal 
        /// information (a VV) from DVVSet using a function called join. Then, we 
        /// extract all values using the function values. The VV should be treated as 
        /// an opaque object that should be returned in a subsequent write. In the 
        /// example, join gives the VV (A,3) and values gives the list of values [v2,v3].</para>
        /// </remarks>
        public T CurrentValue()
        {
            throw new NotImplementedException();
        }

        public void Update(T value)
        {
            throw new NotImplementedException();
        }

        public static DVVSet<T> NewDVVSet(int? actor = null, ulong currentCounter = 0, DVVSet<T> history = null)
        {
            actor = actor ?? AbstractCRDT.DefaultActorId();

            // construct a new dotted version vector based on history
            var dvvSet = new DVVSet<T>();

            if (history != null && history.Versions.Count > 0)
            {

                dvvSet.Versions.UnionWith(history.Versions);
            }

            // TODO : figure out if the new DVVSet should have the highest counter of the Actor or the highest counter seen
            dvvSet.Versions.Add(new VVPair(actor.Value, currentCounter));

            return dvvSet;
        }
    }
}

