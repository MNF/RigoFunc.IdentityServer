# RigoFunc.IdentityServer
This repo contains the plugin for [IdentityServer v4](https://github.com/IdentityServer/IdentityServer4) that uses [ASP.NET Core Identity](https://github.com/aspnet/Identity) as its identity management library.

SEE: [https://github.com/tibold/IdentityServer4.Contrib.AspNetIdentity](https://github.com/tibold/IdentityServer4.Contrib.AspNetIdentity)

The change is we add `CODE` login sent to `Phone Number`.

[![Join the chat at https://gitter.im/xyting/RigoFunc.IdentityServer](https://badges.gitter.im/xyting/RigoFunc.IdentityServer.svg)](https://gitter.im/xyting/RigoFunc.IdentityServer?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


# Announcement
- RigoFunc.IdentityServer.Services.EntityFrameworkCore move to https://github.com/lovedotnet/RigoFunc.IdentityServer.Services.EntityFrameworkCore
- RigoFunc.IdentityServer.RedisStore move to https://github.com/lovedotnet/RigoFunc.IdentityServer.RedisStore

# Breaking Changing
- RigoFunc.IdentityServer.DistributedStore had been deleted, and the distributed IPersistedGrandStore implementation had been merged into RigoFunc.IdentityServer.

# How to use

See Host project