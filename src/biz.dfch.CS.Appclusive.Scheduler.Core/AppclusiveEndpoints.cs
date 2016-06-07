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
using System.Net;
using biz.dfch.CS.Appclusive.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Utilities.Logging;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class AppclusiveEndpoints
    {
        public AppclusiveEndpoints(Uri baseUri, ICredentials credential)
        {
            Contract.Requires(null != baseUri);
            Contract.Requires(baseUri.IsAbsoluteUri);
            Contract.Ensures(null != Diagnostics);
            Contract.Ensures(null != Core);
            Contract.Ensures(null != Infrastructure);
            Contract.Ensures(null != Csm);
            Contract.Ensures(null != Cmp);

            credential = credential ?? CredentialCache.DefaultNetworkCredentials;

            Trace.WriteLine(string.Format("Initialising Appclusive endpoints from '{0}' ...", baseUri.AbsoluteUri));

            Uri uri;
            
            uri = new Uri(string.Format("{0}/Diagnostics", baseUri.AbsoluteUri.TrimEnd('/')));
            Diagnostics = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(uri);
            Diagnostics.Credentials = credential;
            Diagnostics.Format.UseJson();

            uri = new Uri(string.Format("{0}/Core", baseUri.AbsoluteUri.TrimEnd('/')));
            Core = new biz.dfch.CS.Appclusive.Api.Core.Core(uri);
            Core.Credentials = credential;
            Core.Format.UseJson();

            uri = new Uri(string.Format("{0}/Infrastructure", baseUri.AbsoluteUri.TrimEnd('/')));
            Infrastructure = new biz.dfch.CS.Appclusive.Api.Infrastructure.Infrastructure(uri);
            Infrastructure.Credentials = credential;
            Infrastructure.Format.UseJson();

            uri = new Uri(string.Format("{0}/Csm", baseUri.AbsoluteUri.TrimEnd('/')));
            Csm = new biz.dfch.CS.Appclusive.Api.Csm.Csm(uri);
            Csm.Credentials = credential;
            Csm.Format.UseJson();

            uri = new Uri(string.Format("{0}/Cmp", baseUri.AbsoluteUri.TrimEnd('/')));
            Cmp = new biz.dfch.CS.Appclusive.Api.Cmp.Cmp(uri);
            Cmp.Credentials = credential;
            Cmp.Format.UseJson();

            Trace.WriteLine(string.Format("Initialising Appclusive endpoints from '{0}' COMPLETED.", baseUri.AbsoluteUri));
        }

        public biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics Diagnostics { get; set; }
        public biz.dfch.CS.Appclusive.Api.Core.Core Core { get; set; }
        public biz.dfch.CS.Appclusive.Api.Infrastructure.Infrastructure Infrastructure { get; set; }
        public biz.dfch.CS.Appclusive.Api.Csm.Csm Csm { get; set; }
        public biz.dfch.CS.Appclusive.Api.Cmp.Cmp Cmp { get; set; }
    }
}
