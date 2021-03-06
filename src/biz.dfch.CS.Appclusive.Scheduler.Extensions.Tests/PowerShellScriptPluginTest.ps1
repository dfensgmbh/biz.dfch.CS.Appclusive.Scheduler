﻿#Requires -Version 5.0
#Requires -Modules 'Microsoft.PowerShell.Utility'

PARAM
(
	[long] $JobId
)

#
# Copyright 2016 d-fens GmbH
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#

$OutputParameter = @();

$OutputParameter += ("JobId '{0}'" -f $JobId);
$OutputParameter += ("Host.InstanceId '{0}'" -f $host.InstanceId.Guid);

return $OutputParameter;
