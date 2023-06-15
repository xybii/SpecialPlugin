using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SpecialPlugin.Web.Core;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;

namespace SpecialPlugin.Project.IdentityServer
{
    public class Module : StartupModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            var configuration = GetConfiguration(services);

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                .AddProfileService<ProfileService>();

            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                builder.UseMySql(configuration["IdentityServerOptions:ConnectionString"], mySqlOptions => mySqlOptions
                .CharSetBehavior(CharSetBehavior.NeverAppend)
                .MigrationsAssembly(typeof(Module).GetTypeInfo().Assembly.GetName().Name));
            });

            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                builder.UseMySql(configuration["IdentityServerOptions:ConnectionString"], mySqlOptions => mySqlOptions
                .CharSetBehavior(CharSetBehavior.NeverAppend)
                .MigrationsAssembly(typeof(Module).GetTypeInfo().Assembly.GetName().Name));
            });

            builder.AddDeveloperSigningCredential();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(configuration["IdentityServerOptions:ConnectionString"], mySqlOptions => mySqlOptions
                .CharSetBehavior(CharSetBehavior.NeverAppend)
                .MigrationsAssembly(typeof(Module).GetTypeInfo().Assembly.GetName().Name));
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            IdentityModelEventSource.ShowPII = true;

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddSingleton<ICorsPolicyService, CorsPolicyService>();

            services.AddAuthentication()
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://127.0.0.1:5000";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "identity";
                });
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();

            app.UseCookiePolicy();

            app.UseIdentityServer();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                dbContext.Database.Migrate();

                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                configurationDbContext.Database.Migrate();

                if (!configurationDbContext.Clients.Any())
                {
                    configurationDbContext.Clients.Add(new IdentityServer4.EntityFramework.Entities.Client()
                    {
                        ClientId = "identity",
                        ClientName = "客户端模式",
                        ClientSecrets = new List<ClientSecret>()
                        {
                            new ClientSecret() { Type = "SharedSecret", Value = "identity".Sha256() }
                        },
                        AccessTokenLifetime = 3600 * 24,
                        AllowedGrantTypes = new List<ClientGrantType>()
                        {
                            new ClientGrantType(){ GrantType = GrantType.ClientCredentials }
                        },
                        AllowedScopes = new List<ClientScope>()
                        {
                            new ClientScope(){ Scope = IdentityServerConstants.StandardScopes.OpenId },
                            new ClientScope(){ Scope = IdentityServerConstants.StandardScopes.Profile },
                            new ClientScope(){ Scope = IdentityServerConstants.StandardScopes.OfflineAccess },
                            new ClientScope(){ Scope = "identity" }
                        }
                    });

                    configurationDbContext.SaveChanges();
                }

                if (!configurationDbContext.ApiScopes.Any())
                {
                    configurationDbContext.ApiScopes.Add(new IdentityServer4.EntityFramework.Entities.ApiScope()
                    {
                        Name = "identity",
                        Enabled = true,
                        Emphasize = true,
                        Required = true,
                        ShowInDiscoveryDocument = true
                    });

                    configurationDbContext.SaveChanges();
                }

                if (!configurationDbContext.IdentityResources.Any())
                {
                    configurationDbContext.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource()
                    {
                        Name = IdentityServerConstants.StandardScopes.OpenId,
                        Enabled = true,
                        Emphasize = true,
                        NonEditable = true,
                        Required = true,
                        ShowInDiscoveryDocument = true
                    });

                    configurationDbContext.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource()
                    {
                        Name = IdentityServerConstants.StandardScopes.Profile,
                        Enabled = true,
                        Emphasize = true,
                        NonEditable = true,
                        Required = true,
                        ShowInDiscoveryDocument = true
                    });

                    configurationDbContext.IdentityResources.Add(new IdentityServer4.EntityFramework.Entities.IdentityResource()
                    {
                        Name = IdentityServerConstants.StandardScopes.OfflineAccess,
                        Enabled = true,
                        Emphasize = true,
                        NonEditable = true,
                        Required = true,
                        ShowInDiscoveryDocument = true
                    });

                    configurationDbContext.SaveChanges();
                }

                if (!configurationDbContext.ApiResources.Any())
                {
                    configurationDbContext.ApiResources.Add(new IdentityServer4.EntityFramework.Entities.ApiResource()
                    {
                        Name = "identity",
                        Enabled = true,
                        NonEditable = true,
                        ShowInDiscoveryDocument = true,
                        Scopes = new List<ApiResourceScope>()
                        {
                            new ApiResourceScope(){ Scope = IdentityServerConstants.StandardScopes.OpenId },
                            new ApiResourceScope(){ Scope = IdentityServerConstants.StandardScopes.Profile },
                            new ApiResourceScope(){ Scope = IdentityServerConstants.StandardScopes.OfflineAccess },
                            new ApiResourceScope(){ Scope = "identity" }
                        }
                    });

                    configurationDbContext.SaveChanges();
                }
            }
        }

        public static IConfiguration GetConfiguration(IServiceCollection services)
        {
            var service = services
                .FirstOrDefault(o => o.ServiceType == typeof(HostBuilderContext))?
                .ImplementationInstance;

            var hostBuilderContext = service as HostBuilderContext;

            if (hostBuilderContext?.Configuration != null)
            {
                return hostBuilderContext.Configuration as IConfigurationRoot;
            }

            return services.FirstOrDefault(o => o.ServiceType == typeof(IConfiguration)).ImplementationType as IConfigurationRoot;
        }
    }
}
