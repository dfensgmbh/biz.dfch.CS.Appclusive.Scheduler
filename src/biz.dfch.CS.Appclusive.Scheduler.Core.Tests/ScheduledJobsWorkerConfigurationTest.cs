﻿/**
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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Testing.Attributes;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class ScheduledJobsWorkerConfigurationTest
    {
        [TestMethod]
        [ExpectContractFailure]
        public void ServerNotReachableRetriesSettingToZeroThrowsContractException()
        {
            // Arrange

            // Act
            var sut = new ScheduledJobsWorkerConfiguration();
            sut.ServerNotReachableRetries = 0;

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ServerNotReachableRetriesGettingUninitialisedThrowsContractException()
        {
            // Arrange

            // Act
            var sut = new ScheduledJobsWorkerConfiguration();
            var result = sut.ServerNotReachableRetries;

            // Assert
            // N/A
        }
    }
}
