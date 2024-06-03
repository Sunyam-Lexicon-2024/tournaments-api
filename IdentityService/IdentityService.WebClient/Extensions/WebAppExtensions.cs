namespace IdentityService.WebClient.Extensions;

public static class WebAppExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "oidc";
        })
        .AddCookie("Cookies")
        // Auth code flow with PKCE
        // https://docs.duendesoftware.com/identityserver/v7/fundamentals/clients/
        .AddOpenIdConnect("oidc", options =>
        {
            options.Authority = configuration["Oidc:Authority"];

            options.ClientId = "devWeb";
            options.ClientSecret = "devWebSecret";
            options.ResponseType = "code";

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");

            options.MapInboundClaims = false; // Disable claim relabling

            options.SaveTokens = true;
        });

        return services;
    }
}