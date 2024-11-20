using System;
using HouseOs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace HouseOs.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

public class AuthorizeAttribute: Attribute, IAuthorizationFilter
{

    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
         var logger = context.HttpContext.RequestServices.GetService<ILogger<AuthorizeAttribute>>();
         logger?.LogInformation("AuthorizeAttribute: OnAuthorization called");

        var isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated ?? false;
        var userInItems = context.HttpContext.Items["User"] as User;
        logger?.LogInformation($"IsAuthenticated: {isAuthenticated}, User in Items: {userInItems != null}");

        if (!isAuthenticated || userInItems == null)
        {
            logger?.LogWarning("AuthorizeAttribute: User not authenticated");
            context.Result = new JsonResult(new { message = "Unauthorized" }) 
            { 
                StatusCode = StatusCodes.Status401Unauthorized 
            };
        }
        else
        {
            logger?.LogInformation($"AuthorizeAttribute: User authorized. User ID: {userInItems?.Id ?? 0}");
        }

    }

}
