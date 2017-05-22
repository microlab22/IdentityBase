﻿using IdentityBase.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityBase.Public
{
    public static class StartupRestApi
    {
        public static void AddRestApi(this IServiceCollection services, ApplicationOptions options)
        {
            var issuer = "http://localhost:5000"; 

            services.AddAuthorization(authOptions =>
            {
                authOptions.AddScopePolicy("useraccount:read", issuer);
                authOptions.AddScopePolicy("useraccount:write", issuer);
                authOptions.AddScopePolicy("useraccount:delete", issuer);
            });
        }

        public static void AddScopePolicy(this AuthorizationOptions options, string scope, string issuer)
        {
            options.AddPolicy(scope, policy => policy.Requirements.Add(new HasScopeRequirement(scope, issuer)));
        }

        public static void UseRestApi(this IApplicationBuilder app, ApplicationOptions options)
        {
            app.Map("/api", appApi =>
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                appApi.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
                {
                    Authority = "http://localhost:5000",
                    RequireHttpsMetadata = false,
                    AllowedScopes = { "api1" },
                    AutomaticAuthenticate = true
                });

                appApi.UseMvc(routes =>
                {
                    routes.MapRoute(name: "api", template: "{controller=Status}/{action=Get}/{id?}", defaults: new { area = "Api" });
                });
            });
        }
    }

    public class HasScopeRequirement : AuthorizationHandler<HasScopeRequirement>, IAuthorizationRequirement
    {
        private readonly string issuer;
        private readonly string scope;

        public HasScopeRequirement(string scope, string issuer)
        {
            this.scope = scope;
            this.issuer = issuer;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == issuer))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == issuer).Value.Split(' ');

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == scope))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
