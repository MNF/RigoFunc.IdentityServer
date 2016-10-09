using IdentityServer4.Models;
using System.Collections.Generic;

namespace Host.Configuration {
    public class Clients {
        public static IEnumerable<Client> Get() {
            var scopes = new List<string> {
                        StandardScopes.OpenId.Name,
                        StandardScopes.ProfileAlwaysInclude.Name,
                        StandardScopes.EmailAlwaysInclude.Name,
                        StandardScopes.PhoneAlwaysInclude.Name,
                        StandardScopes.RolesAlwaysInclude.Name,
                        StandardScopes.OfflineAccess.Name,
                        StandardScopes.AllClaims.Name,
                    };

            return new List<Client> {
                new Client {
                    ClientId = "app",
                    ClientName = "appapi",
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = scopes,
                    // Defaults to 1296000 seconds / 15 days
                    AccessTokenLifetime = 1296000,
                    AccessTokenType = AccessTokenType.Reference
                },

                new Client {
                    ClientId = "admin",
                    ClientName = "spa",
                    ClientSecrets = {
                        new Secret("secret5".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = scopes,
                    AccessTokenType = AccessTokenType.Jwt
                }
            };
        }
    }
}