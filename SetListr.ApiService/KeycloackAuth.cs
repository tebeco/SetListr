// using System;

// using Microsoft.IdentityModel.Tokens;

// namespace SetListr.ApiService;

// public static class KeycloackAuth
// {
//     public static void AddKeyclockAuth(this IServiceCollection services, IConfiguration configuration)
//     {
//         // https://dev.to/kayesislam/integrating-openid-connect-to-your-application-stack-25ch
//         services
//             .AddAuthentication()
//             .AddJwtBearer(x =>
//             {
//                 x.RequireHttpsMetadata = Convert.ToBoolean($"{configuration["Keycloak:require-https"]}");
//                 x.MetadataAddress = $"{configuration["Keycloak:server-url"]}/realms/projects/.well-known/openid-configuration";
//                 x.TokenValidationParameters = new TokenValidationParameters
//                 {
//                     RoleClaimType = "groups",
//                     NameClaimType = $"{configuration["Keycloak:name_claim"]}",
//                     ValidAudience = $"{configuration["Keycloak:audience"]}",
//                     // https://stackoverflow.com/questions/60306175/bearer-error-invalid-token-error-description-the-issuer-is-invalid
//                     ValidateIssuer = Convert.ToBoolean($"{configuration["Keycloak:validate-issuer"]}"),  
//                 };
//             });
        
//         services.AddAuthorization(o =>
//         {
//             o.DefaultPolicy = new AuthorizationPolicyBuilder()
//                 .RequireAuthenticatedUser()
//                 .RequireClaim("email_verified", "true")
//                 .Build();
//         });
        
//         return services;
//     }
// }
