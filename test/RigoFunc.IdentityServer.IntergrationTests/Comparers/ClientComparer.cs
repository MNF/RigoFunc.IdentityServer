using System;
using System.Collections.Generic;
using IdentityServer4.Models;

namespace RigoFunc.IdentityServer.IntergrationTests.Comparers {
    public class ClientComparer : IEqualityComparer<Client> {
        public bool Equals(Client x, Client y) {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.ClientId == y.ClientId
                   && x.AbsoluteRefreshTokenLifetime == y.AbsoluteRefreshTokenLifetime
                   && x.AccessTokenLifetime == y.AccessTokenLifetime
                   && x.AuthorizationCodeLifetime == y.AuthorizationCodeLifetime
                   && x.IdentityTokenLifetime == y.IdentityTokenLifetime
                   && x.SlidingRefreshTokenLifetime == y.SlidingRefreshTokenLifetime
                //todo
                ;
        }

        public int GetHashCode(Client obj) {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Name field if it is not null.

            //Get hash code for the Code field.

            //Calculate the hash code for the product.
            return
                (obj.AbsoluteRefreshTokenLifetime)
                ^ (obj.AccessTokenLifetime)
                ^ (obj.AuthorizationCodeLifetime)
                ^ (obj.IdentityTokenLifetime)
                ^ (obj.SlidingRefreshTokenLifetime)
                //todo
                ;
        }
    }
}
