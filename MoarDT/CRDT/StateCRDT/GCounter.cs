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
using System.Linq;
using System.Collections.Generic;
using MoarDT.Extensions;

namespace MoarDT
{
    public class GCounter 
    {
        // TODO: Implement ISerializable
        // TODO: implement ICRDT with Merge, Value
        // TODO: Move DefaultClientId to abstract base class
        internal Dictionary<string, ulong> _contents { get; set; }
        private ulong _currentValue;
        private string _clientId;

        public GCounter(string clientId = null, ulong currentValue = default(ulong), Dictionary<string, ulong> counterContents = null)
        {
            _clientId = String.IsNullOrEmpty(clientId) ? DefaultClientId() : clientId;
            _currentValue = currentValue;

            if (counterContents != null)
                _contents = counterContents;
            else
                _contents = new Dictionary<string, ulong>();
        }

        public ulong Value {
            get {
                return _contents[_clientId];
            }
        }

        public static GCounter operator ++(GCounter gc)
        {
            return gc.Increment(1);
        }

        public GCounter Increment(ulong item = 1)
        {
            _contents[_clientId] = _contents.ValueOrDefault(_clientId) + item;
            return this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = _currentValue.GetHashCode();
                result = (result * 397) ^ _contents.GetHashCode();
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(ulong))
                return false;

            return Equals((GCounter)obj);
        }

        public bool Equals(GCounter other)
        {
            /* (∀i ∈ [0, n − 1] : X.P [i] ≤ Y.P [i]) */
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            var keys = _contents.Keys.Union(other._contents.Keys);

            return keys.All(k => _contents[k] <= other._contents[k]);
        }

        public static GCounter Merge(GCounter gca, GCounter gcb)
        {
            /* let ∀i ∈ [0,n − 1] : Z.P[i] = max(X.P[i],Y.P[i]) */
            var gcList = new List<Dictionary<string, ulong>>();
            gcList.Add(gca._contents);
            gcList.Add(gcb._contents);

            var keys = gca._contents.Keys.Union(gcb._contents.Keys);
            var newContents = new Dictionary<string, ulong>();

            foreach (var key in keys)
            {
                if (!gca._contents.ContainsKey(key) && gcb._contents.ContainsKey(key))
                    newContents[key] = gcb._contents[key];
                else if (gca._contents.ContainsKey(key) && !gcb._contents.ContainsKey(key))
                    newContents[key] = gca._contents[key];
                else
                    newContents[key] = Math.Max(gca._contents[key], gcb._contents[key]);
            }

            return new GCounter(clientId: DefaultClientId(), counterContents: newContents);
        }

        private static string DefaultClientId()
        {
            return System.Net.Dns.GetHostName();
        }
    }
}

