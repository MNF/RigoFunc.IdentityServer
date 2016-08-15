# RedisStore

�ṩ�˴洢Token����Ȩ��Redis�Ĺ���


## Usage

### Install
```powershell
Install-Package  RigoFunc.IdentityServer.RedisStore
```

### Configure

```c#
//Startup.cs
using RigoFunc.IdentityServer;
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddRedisTransientStores(options => {
                options.config = "192.168.1.7:6379";
                options.db = 0;
            });
            //...
        }
    }
```