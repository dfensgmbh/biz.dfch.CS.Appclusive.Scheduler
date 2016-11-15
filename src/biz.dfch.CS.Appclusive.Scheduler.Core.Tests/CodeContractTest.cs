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
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Testing.Attributes;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class CodeContractTest
    {
        class CodeContractTester
        {
            internal bool ContractEnsureTester()
            {
                Contract.Ensures(true == Contract.Result<bool>());

                return false;
            }
        }

        [TestMethod]
        [ExpectContractFailure]
        public void RequiresEnabledThrowsContractException()
        {
            Contract.Requires(false == true);
        
            Assert.Fail("CodeContracts are not enabled.");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void EnsuresEnabledThrowsContractException()
        {
            // Arrange
            var sut = new CodeContractTester();

            // Act
            sut.ContractEnsureTester();
       
            Assert.Fail("CodeContracts are not enabled.");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void AssertEnabledThrowsContractException()
        {
            Contract.Assert(false == true);
        
            Assert.Fail("CodeContracts are not enabled.");
        }
    }
}
