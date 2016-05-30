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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Reflection;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public class SchedulerPluginParameters : Dictionary<string, object>
    {
        public T Convert<T>() 
            where T : new()
        {
            var t = new T();

            var propInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Contract.Assert(null != propInfos);
            
            foreach (var propInfo in propInfos)
            {
                object dictionaryPropertyValue;
                this.TryGetValue(propInfo.Name, out dictionaryPropertyValue);

                object propertyValue = null;
                var propertyType = propInfo.PropertyType;
                try
                {
                    if (null != dictionaryPropertyValue &&
                        dictionaryPropertyValue is IEnumerable)
                    {
                        propertyValue = dictionaryPropertyValue;
                    }
                    else if(dictionaryPropertyValue is IConvertible)
                    {
                        propertyValue = System.Convert.ChangeType(dictionaryPropertyValue, propertyType);
                    }
                    else
                    {
                        propertyValue = dictionaryPropertyValue;
                    }
                    propInfo.SetValue(t, propertyValue, null);

                    if (null == dictionaryPropertyValue && 0 < propInfo.CustomAttributes.Count())
                    {
                        var attribute = propInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(DefaultValueAttribute));
                        if (null != attribute && 1 == attribute.ConstructorArguments.Count())
                        {
                            propInfo.SetValue(t, attribute.ConstructorArguments[0].Value, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Contract.Assert(null != propertyValue, string.Format("Conversion FAILED for '{0}' with type '{1}'.", propInfo.Name, propertyType.Name));
                }
            }

            var context = new ValidationContext(t, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(t, context, results, true);
            if (!isValid)
            {
                foreach (var validationResult in results)
                {
                    Contract.Assert(isValid, string.Format("Object validation FAILED: '{0}'", validationResult.ErrorMessage));
                }
            }

            return t;
        }
    }
}
