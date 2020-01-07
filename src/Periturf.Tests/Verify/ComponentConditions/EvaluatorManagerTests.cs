/*
 *     Copyright 2019 Adam Burton (adz21c@gmail.com)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using NUnit.Framework;
using Periturf.Verify;
using Periturf.Verify.ComponentConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify.ComponentConditions
{
    [TestFixture]
    class EvaluatorManagerTests
    {
        [Test]
        public async Task Given_Instances_When_GetInstancesAndDispose_Then_ReceiveAllInOrder()
        {
            var instance1 = new ConditionInstance(TimeSpan.FromMilliseconds(1000), "ID1");
            var instance2 = new ConditionInstance(TimeSpan.FromMilliseconds(2000), "ID2");
            var manager = new EvaluatorManager();

            Task<List<ConditionInstance>> getInstancesTask;
            await using (var evaluator = await manager.CreateEvaluatorAsync())
            {
                getInstancesTask = Task.Run(() => evaluator.GetInstancesAsync().AsyncToListAsync());

                var instanceHandler = (IConditionInstanceHandler)manager;
                await instanceHandler.HandleInstanceAsync(instance1);
                await instanceHandler.HandleInstanceAsync(instance2);

                await Task.Delay(100);
            
                Assert.That(manager.HasActiveEvaluators, Is.True);
            }

            var instances = await getInstancesTask;

            Assert.That(instances, Is.Not.Null);
            Assert.That(instances.Count, Is.EqualTo(2));
            Assert.That(instances, Does.Contain(instance1));
            Assert.That(instances, Does.Contain(instance2));
            Assert.That(instances.First(), Is.SameAs(instance1));
            Assert.That(instances.Last(), Is.SameAs(instance2));
        }

        [Test]
        public async Task Given_MultipleEvaluators_When_GetInstancesAndDispose_Then_ReceiveExcepted()
        {
            var instance1 = new ConditionInstance(TimeSpan.FromMilliseconds(1000), "ID1");
            var instance2 = new ConditionInstance(TimeSpan.FromMilliseconds(2000), "ID2");
            var instance3 = new ConditionInstance(TimeSpan.FromMilliseconds(3000), "ID2");

            var evaluator1Expectation = new[] { instance1 };
            var evaluator2Expectation = new[] { instance1, instance2 };
            var evaluator3Expectation = new[] { instance1, instance2, instance3 };
            var evaluator4Expectation = new[] { instance1, instance2, instance3 };

            var manager = new EvaluatorManager();
            var instanceHandler = (IConditionInstanceHandler)manager;

            var evaluator1 = await manager.CreateEvaluatorAsync();
            var getEvaluator1Tasks = Task.Run(() => evaluator1.GetInstancesAsync().AsyncToListAsync());

            await instanceHandler.HandleInstanceAsync(instance1);
            
            await Task.Delay(10);
            await evaluator1.DisposeAsync();
            
            var evaluator2 = await manager.CreateEvaluatorAsync();
            var getEvaluator2Tasks = Task.Run(() => evaluator2.GetInstancesAsync().AsyncToListAsync());
            
            await instanceHandler.HandleInstanceAsync(instance2);
            
            await Task.Delay(10);
            await evaluator2.DisposeAsync();

            var evaluator3 = await manager.CreateEvaluatorAsync();
            var getEvaluator3Tasks = Task.Run(() => evaluator3.GetInstancesAsync().AsyncToListAsync());
            
            await instanceHandler.HandleInstanceAsync(instance3);
            
            await Task.Delay(10);
            await evaluator3.DisposeAsync();

            var evaluator4 = await manager.CreateEvaluatorAsync();
            var getEvaluator4Tasks = Task.Run(() => evaluator4.GetInstancesAsync().AsyncToListAsync());
            
            await Task.Delay(10);
            await evaluator4.DisposeAsync();

            await Task.Delay(50);   // Wait for manager to cleanup

            var evaluator1Instances = await getEvaluator1Tasks;
            var evaluator2Instances = await getEvaluator2Tasks;
            var evaluator3Instances = await getEvaluator3Tasks;
            var evaluator4Instances = await getEvaluator4Tasks;

            Assert.That(evaluator1Expectation.Zip(evaluator1Instances).All(x => x.First == x.Second));
            Assert.That(evaluator2Expectation.Zip(evaluator2Instances).All(x => x.First == x.Second));
            Assert.That(evaluator3Expectation.Zip(evaluator3Instances).All(x => x.First == x.Second));
            Assert.That(evaluator4Expectation.Zip(evaluator4Instances).All(x => x.First == x.Second));
            Assert.That(manager.HasActiveEvaluators, Is.False);
        }
    }
}
