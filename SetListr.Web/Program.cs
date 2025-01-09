using SetListr.Web;
using SetListr.Web.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using SetListr.Web.Services.Settings;
using SetListr.Web.Services.Versioning;
using SetListr.Web.Services.DTO;
using SetListr.Web.Services.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Keycloak.AuthServices.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor()
                .AddTransient<AuthorizationHandler>();

builder.Services.AddFluentUIComponents();

builder.Services.AddOutputCache();
    
var oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

builder.Services.AddAuthentication(oidcScheme)
                .AddKeycloakWebApp(
                    builder.Configuration.GetSection(KeycloakAuthenticationOptions.Section),
                    configureOpenIdConnectOptions: options =>
                    {
                        // we need this for front-channel sign-out
                        options.SaveTokens = true;
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.RequireHttpsMetadata = false;
                        options.ClientId = "setlistr-frontend";
                        options.Scope.Add("setlistr:all");
                        options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.Events = new OpenIdConnectEvents
                        {
                            OnSignedOutCallbackRedirect = context =>
                            {
                                context.Response.Redirect("/");
                                context.HandleResponse();

                                return Task.CompletedTask;
                            }
                        };
                    });

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpClient<SetListApiClient>(client => 
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
        client.Timeout = TimeSpan.FromSeconds(500);
    })
    .AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddScoped<CacheStorageAccessor>();
builder.Services.AddSingleton<IAppVersionService, AppVersionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();
MapLoginAndLogout(app);

app.Run();

static IEndpointConventionBuilder MapLoginAndLogout(IEndpointRouteBuilder endpoints)
{
    var group = endpoints.MapGroup("authentication");

    group.MapGet("/login", () => TypedResults.Challenge(new AuthenticationProperties { RedirectUri = "/" }))
        .AllowAnonymous();

    group.MapPost("/logout", () => TypedResults.SignOut(new AuthenticationProperties { RedirectUri = "/" },
        [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

    return group;
}
