using Correios.NET.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Correios.NET.Api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                switch (context.Response.StatusCode)
                {
                    case 404:                        
                        break;
                    case 418:
                        break;
                    default:
                        break;
                }
            }
            catch (ParseException e)
            {
                HandleNotFound(context, e);
            }
            catch (Exception e)
            {
                HandleException(context, e);
            }            
        }

        private static void HandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 500;
        }

        private static void HandleNotFound(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 404;
        }
    }
}
