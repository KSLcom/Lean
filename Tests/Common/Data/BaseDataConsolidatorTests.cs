﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using NUnit.Framework;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;

namespace QuantConnect.Tests.Common.Data
{
    [TestFixture]
    public class BaseDataConsolidatorTests
    {
        [Test]
        public void AggregatesNewTradeBarProperly()
        {
            TradeBar newTradeBar = null;
            var creator = new BaseDataConsolidator(4);
            creator.DataConsolidated += (sender, tradeBar) =>
            {
                newTradeBar = tradeBar;
            };
            var reference = DateTime.Today;
            var bar1 = new Tick
            {
                Symbol = "SPY",
                Time = reference,
                Value = 5,
                Quantity = 10
            };
            creator.Update(bar1);
            Assert.IsNull(newTradeBar);

            var bar2 = new Tick
            {
                Symbol = "SPY",
                Time = reference.AddHours(1),
                Value = 10,
                Quantity = 20
            };
            creator.Update(bar2);
            Assert.IsNull(newTradeBar);
            var bar3 = new Tick
            {
                Symbol = "SPY",
                Time = reference.AddHours(2),
                Value = 1,
                Quantity = 10
            };
            creator.Update(bar3);
            Assert.IsNull(newTradeBar);

            var bar4 = new Tick
            {
                Symbol = "SPY",
                Time = reference.AddHours(3),
                Value = 9,
                Quantity = 20
            };
            creator.Update(bar4);
            Assert.IsNotNull(newTradeBar);

            Assert.AreEqual("SPY", newTradeBar.Symbol);
            Assert.AreEqual(bar1.Time, newTradeBar.Time);
            Assert.AreEqual(bar1.Value, newTradeBar.Open);
            Assert.AreEqual(bar2.Value, newTradeBar.High);
            Assert.AreEqual(bar3.Value, newTradeBar.Low);
            Assert.AreEqual(bar4.Value, newTradeBar.Close);

            // base data can't aggregate volume
            Assert.AreEqual(0, newTradeBar.Volume);
        }
    }
}
