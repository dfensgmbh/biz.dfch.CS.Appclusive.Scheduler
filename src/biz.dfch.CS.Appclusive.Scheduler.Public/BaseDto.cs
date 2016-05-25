﻿/**
 * Copyright 2015-2016 d-fens GmbH
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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Newtonsoft.Json;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public abstract class BaseDto
    {
        public string SerializeObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static object DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        [Pure]
        public virtual bool IsValid()
        {
            if (0 < TryValidate().Count)
            {
                return false;
            }
            return true;
        }

        public virtual List<ValidationResult> GetValidationResults()
        {
            return TryValidate();
        }

        private List<ValidationResult> TryValidate()
        {
            var context = new ValidationContext(this, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, results, true);
            return results;
        }


        public virtual void Validate()
        {
            var results = TryValidate();
            var isValid = 0 >= results.Count ? true : false;

            if (isValid)
            {
                return;
            }

            foreach (var result in results)
            {
                Contract.Assert(isValid, result.ErrorMessage);
            }
        }
    }
}
