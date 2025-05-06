﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoList.Core.Errors;
using ToDoList.Errors;

namespace ToDoList.Server.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext actionExecutedContext)
    {
        var code = 500;

        if (actionExecutedContext.Exception is EntityNotFoundException)
        {
            code = 404; // Not found
        }

        if (actionExecutedContext.Exception is UnauthorizedException)
        {
            code = 401; // Not found
        }


        actionExecutedContext.HttpContext.Response.StatusCode = code;

        actionExecutedContext.Result = new JsonResult(new
        {
            error = actionExecutedContext.Exception.Message
        });
    }
}
