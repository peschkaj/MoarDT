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
    public static class IntegerExtensions
    {
        public static byte[] ToCrdtString(this int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        public static int FromCrdtString(this byte[] value, int offset = 0)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(value);

            return BitConverter.ToInt32(value, offset);
        }
    }
}

