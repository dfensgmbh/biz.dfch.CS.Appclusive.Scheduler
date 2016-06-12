/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Scheduler.Public;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class ScheduledJobsWorkerTest
    {
        public ScheduledJobsWorkerConfiguration configuration;
        public Timer timer;
        
        class DefaultPluginData : IAppclusivePluginData
        {
            public string Type { get; set; }

            public int Priority { get; set; }

            public string Role { get; set; }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var loader = new ScheduledJobsWorkerConfigurationLoader();

            configuration = Mock.Create<ScheduledJobsWorkerConfiguration>(Constructor.Mocked);
            Mock.Arrange(() => configuration.Logger).IgnoreInstance().Returns(new Logger());
            Mock.Arrange(() => configuration.Uri).IgnoreInstance().Returns(new Uri("http://www.example.com/arbitrary-folder"));
            Mock.Arrange(() => configuration.ManagementUriName).IgnoreInstance().Returns("ManagementUriName");
            Mock.Arrange(() => configuration.ManagementUriType).IgnoreInstance().Returns("ManagementUriName");
            Mock.Arrange(() => configuration.ServerNotReachableRetries).IgnoreInstance().Returns(5);
            Mock.Arrange(() => configuration.UpdateIntervalInMinutes).IgnoreInstance().Returns(1);
            Mock.Arrange(() => configuration.IsValid()).IgnoreInstance().Returns(true);

            var timer = Mock.Create<ScheduledTaskWorkerTimerFactory>();
            Mock.Arrange(() => timer.CreateTimer(Arg.IsAny<TimerCallback>(), Arg.IsAny<object>(), Arg.IsAny<int>(), Arg.IsAny<int>()))
                .IgnoreInstance()
                .Returns(default(Timer));
        }

        [TestMethod]
        public void RunTaskSucceeds()
        {
            // Arrange
            var task = Mock.Create<ScheduledJobScheduler>();
            Mock.Arrange(() => task.IsScheduledToRun(Arg.IsAny<DateTimeOffset>()))
                .IgnoreInstance()
                .Returns(true)
                .MustBeCalled();

            var scheduledJobsManagerImpl = Mock.Create<ScheduledJobsManagerImpl>();
            Mock.Arrange(() => scheduledJobsManagerImpl.GetJobs(Arg.IsAny<AppclusiveEndpoints>()))
                .IgnoreInstance()
                .Returns(new ScheduledJobsMockData().GetJobs())
                .MustBeCalled();

            var defaultPlugin = Mock.Create<DefaultPlugin>();
            Mock.Arrange(() => defaultPlugin.Invoke(Arg.IsAny<DictionaryParameters>(), Arg.IsAny<NonSerialisableJobResult>()))
                .IgnoreInstance()
                .Returns(true)
                .MustBeCalled();

            configuration.Plugins = new List<Lazy<IAppclusivePlugin, IAppclusivePluginData>>()
            {
                new Lazy<IAppclusivePlugin, IAppclusivePluginData>
                (
                    () => 
                    { 
                        return new DefaultPlugin();
                    }
                    , 
                    new DefaultPluginData()
                    {
                        Type = "Default"
                        ,
                        Priority = int.MinValue
                        ,
                        Role = "Default"
                    }
                )
                ,
                new Lazy<IAppclusivePlugin, IAppclusivePluginData>
                (
                    () => 
                    { 
                        return new DefaultPlugin();
                    }
                    , 
                    new DefaultPluginData()
                    {
                        Type = "Default"
                        ,
                        Priority = int.MinValue + 1
                        ,
                        Role = "RaisedPriority"
                    }
                )
            };
            var sut = new ScheduledJobsWorker(configuration);
            var isUpdateScheduledTaskSucceeded = sut.GetScheduledJobs();
            Contract.Assert(isUpdateScheduledTaskSucceeded);
            sut.IsActive = isUpdateScheduledTaskSucceeded;
            
            var stateObject = default(object);

            // Act
            sut.RunTasks(stateObject);

            // Assert
            Mock.Assert(scheduledJobsManagerImpl);
            Mock.Assert(task);
            Mock.Assert(defaultPlugin);
        }
    }
}
