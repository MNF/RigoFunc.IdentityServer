# RigoFunc.IdentityServer
This repo contains the plugin for [IdentityServer v4](https://github.com/IdentityServer/IdentityServer4) that uses [ASP.NET Core Identity](https://github.com/aspnet/Identity) as its identity management library.

SEE: [https://github.com/tibold/IdentityServer4.Contrib.AspNetIdentity](https://github.com/tibold/IdentityServer4.Contrib.AspNetIdentity)

The change is we add `CODE` login sent to `Phone Number`.

[![Join the chat at https://gitter.im/xyting/RigoFunc.IdentityServer](https://badges.gitter.im/xyting/RigoFunc.IdentityServer.svg)](https://gitter.im/xyting/RigoFunc.IdentityServer?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


# Announcement
- RigoFunc.IdentityServer.Services.EntityFrameworkCore move to https://github.com/lovedotnet/RigoFunc.IdentityServer.Services.EntityFrameworkCore
- RigoFunc.IdentityServer.RedisStore move to https://github.com/lovedotnet/RigoFunc.IdentityServer.RedisStore

# Wiki
https://github.com/xyting/RigoFunc.IdentityServer/wiki

# OAuth Client
https://github.com/xyting/RigoFunc.OAuth


# Token Redis Store
- ref https://github.com/kylesonaty/IdentityServer3.Contrib.Store.Redis

## Usage
```
var builder = services.AddIdentityServer().AddRedisTransientStores(options=>{
	options.config = "localhost";
	options.db = 0;
});
```

# External login
[SEE:ExternalLogin.md](./doc/ExternalLogin.md)
```sequence
title:Title
participant A
participant B
participant C

```

# Clients and Scopes
[SEE](./src/RigoFunc.IdentityServer.Services.EntityFrameworkCore/README.md)
