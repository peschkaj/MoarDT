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
using NUnit.Framework;

namespace MoarDT.Tests.CRDT.StateCRDT
{
    [TestFixture()]
    public class GCounterTest
    {
        [Test()]
        public void MergingTwoGCountersReturnsTheLargestValue()
        {
            var gca = new GCounter();
            var gcb = new GCounter();

            gca++;
            gca++;
            gcb++;

            var gca_val = gca.Value;
            var gcb_val = gcb.Value;

            var gcnew = GCounter.Merge(gca, gcb);

            gcnew.Value.ShouldEqual(gca_val);
            gcnew.Value.ShouldNotEqual(gcb_val);
        }
    }
}

