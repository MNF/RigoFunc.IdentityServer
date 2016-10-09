using IdentityServer4.Models;
using System.Collections.Generic;

namespace Host.Configuration {
    public class Scopes {
        public static IEnumerable<Scope> Get() {
            var list = new List<Scope>();
            // your scope here

            list.AddRange(StandardScopes.AllAlwaysInclude);
            list.Add(StandardScopes.RolesAlwaysInclude);
            list.Add(StandardScopes.AllClaims);
            list.Add(StandardScopes.OfflineAccess);

            return list;
        }
    }
}