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
using System.Text;

namespace MoarDT
{
    public static class StringExtensions
    {
        private static readonly Encoding CrdtEncoding = new UTF8Encoding(false);

        public static byte[] ToCrdtString(this string value)
        {
            return value == null ? null : CrdtEncoding.GetBytes(value);
        }

        public static string FromCrdtString(this byte[] value)
        {
            return value == null ? null : CrdtEncoding.GetString(value);
        }
    }
}

