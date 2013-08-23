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
    public class DVVPair : VVPair
    {
        /// <summary>
        /// A dot is used when the version vector has a gap in causal history.
        /// E.g. the DVV (A, 5, 12) has seen all activity from actor A from 
        /// version 1 through 5 and then has an update from A with a version of 12.
        /// </summary>
        /// <value>The dot.</value>
        public ulong? Dot { get; set; }

        public DVVPair (string actor, ulong counter, ulong? dot = null) : base(actor, counter)
        {
            Dot = dot;
        }
    }
}

