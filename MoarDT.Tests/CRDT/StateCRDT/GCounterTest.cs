//
//  Copyright 2013  Jeremiah Peschka
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

using MoarDT.CRDT.StateCRDT;
using NUnit.Framework;
using MoarDT.Tests.Extensions;

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

            var gcaVal = gca.Value;
            var gcbVal = gcb.Value;

            var gcnew = GCounter.Merge(gca, gcb);

            gcnew.Value.ShouldEqual(gcaVal);
            gcnew.Value.ShouldNotEqual(gcbVal);
        }

        [Test]
        public void ValuesConverge()
        {
            var gca = new GCounter(1);
            var gcb = new GCounter(2);
            var gcc = new GCounter(3);

            gca++;
            gcb.Increment(10);
            gcc.Increment(4);

            var gcResult = GCounter.Merge(GCounter.Merge(gca, gcb), gcc);

            gcResult.Value.ShouldEqual(15UL);
        }

        [Test]
        public void NewGCountersAreZero()
        {
            var gca = new GCounter();

            gca.Value.ShouldEqual(0UL);
        }
    }
}

