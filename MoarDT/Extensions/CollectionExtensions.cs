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

namespace MoarDT.Extensions
{
    public static class CollectionExtensions
    {
        internal static IList<TR> FullOuterJoin<TA, TB, TK, TR>(
            this IEnumerable<TA> a,
            IEnumerable<TB> b,
            Func<TA, TK> selectKeyA, 
            Func<TB, TK> selectKeyB,
            Func<TA, TB, TK, TR> projection,
            TA defaultA = default(TA), 
            TB defaultB = default(TB),
            IEqualityComparer<TK> cmp = null)
        {
            cmp = cmp?? EqualityComparer<TK>.Default;
            var alookup = a.ToLookup(selectKeyA, cmp);
            var blookup = b.ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TK>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                from xa in alookup[key].DefaultIfEmpty(defaultA)
                    from xb in blookup[key].DefaultIfEmpty(defaultB)
                    select projection(xa, xb, key);

            return join.ToList();
        }
    }
}

