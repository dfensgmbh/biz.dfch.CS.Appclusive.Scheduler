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
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Public.Tests
{
    [TestClass]
    public class IConfigurationLoaderTest
    {
        class ConfigurationLoaderImpl : IConfigurationLoader
        {
            public void Initialise(BaseDto configuration, DictionaryParameters parameters)
            {
                Contract.Requires(configuration is ConfigurationImpl);

                var configurationImpl = configuration as ConfigurationImpl;
                configurationImpl.StringProperty = "arbitrary-string";

                return;
            }
        }

        class ConfigurationImpl : BaseDto
        {
            [Required]
            public string StringProperty { get; set; }

            public ConfigurationImpl()
            {
                // N/A
            }
            
            public ConfigurationImpl(IConfigurationLoader loader)
            {
                Contract.Requires(null != loader);
                loader.Initialise(this, null);
            }
        }

        class WrongConfigurationImpl : BaseDto
        {
            [Required]
            public string StringProperty { get; set; }

            public WrongConfigurationImpl()
            {
                // N/A
            }
            
            public WrongConfigurationImpl(IConfigurationLoader loader)
            {
                Contract.Requires(null != loader);
                loader.Initialise(this, null);
            }
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithConfigurationNullThrowsContractException()
        {
            // Arrange
            ConfigurationImpl configuration = null;
            var parameters = default(DictionaryParameters);

            // Act
            var sut = new ConfigurationLoaderImpl();
            sut.Initialise(configuration, parameters);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithWrongConfigurationThrowsContractException()
        {
            // Arrange
            WrongConfigurationImpl configuration = new WrongConfigurationImpl();
            var parameters = default(DictionaryParameters);

            // Act
            var sut = new ConfigurationLoaderImpl();
            sut.Initialise(configuration, parameters);
        }

        [TestMethod]
        public void InitialiseSucceeds()
        {
            // Arrange
            ConfigurationImpl configuration = new ConfigurationImpl();
            var parameters = default(DictionaryParameters);

            // Act
            var sut = new ConfigurationLoaderImpl();
            sut.Initialise(configuration, parameters);
        }
    }
}
