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


using NUnit.Framework;
using MoarDT.CRDT.StateCRDT;
using MoarDT.Tests.Extensions;

namespace MoarDT.Tests.CRDT.StateCRDT
{
    [TestFixture]
    public class PNCounterTest
    {
        [Test]
        public void ValueTests()
        {
            var pna = new PNCounter();
            var pnb = new PNCounter();
            var pnc = new PNCounter();

            pna.Increment(5);
            pna.Increment(10);

            pnb++;
            pnb.Increment(2);
            pnb++;

            pnc.Decrement();

            pna.Value.ShouldEqual(15);

            pnb.Value.ShouldEqual(4);

            pnc.Value.ShouldBeLessThan(0);
            pnc.Value.ShouldEqual(-1);
        }

        [Test]
        public void SingleActorMergeTests()
        {
            var pna = new PNCounter();
            var pnb = new PNCounter();

            pna.Increment(5);
            pnb++;

            var pnc = PNCounter.Merge(pna, pnb);

            pnc.Value.ShouldEqual(5);
        }

        [Test]
        public void MultiActorMergeTests()
        {
            var pna = new PNCounter();
            var pnb = new PNCounter(1);

            pna.Increment(5);
            pnb++;

            var pnc = PNCounter.Merge(pna, pnb);

            pnc.Value.ShouldEqual(6);
        }
    }
}
