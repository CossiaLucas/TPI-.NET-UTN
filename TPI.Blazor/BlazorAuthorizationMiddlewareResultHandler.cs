using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Components.Endpoints;

namespace TPI.Blazor
{
    public sealed class BlazorAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _default = new();

        public Task HandleAsync(RequestDelegate next, HttpContext context,
            AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            //AuthorizeRouteView vuelve a evaluar
            // [Authorize] contra el AuthenticationStateProvider dentro del circuito.
            if (context.GetEndpoint()?.Metadata.GetMetadata<ComponentTypeMetadata>() is not null)
                return next(context);

            return _default.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
