﻿using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Services.Contracts;
using System.Net;
namespace WebAPI.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app, ILoggerService logger)
        /*Middleware'e bir request geldiğinde ilgili request'in ConfigureExceptionHandler üzerinden geçmesini sağlayacağız.*/
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context => /*Buradaki context'i request olarak düşünebiliriz.*/
                {
                    context.Response.ContentType="application/json";
                    var contextFeature=context.Features.Get<IExceptionHandlerFeature>(); 
                    /*context'in IExceptionHandlerFeature gibi bir özelliği var mı yok mu anlamak önemli. Eğer böyle bir özelliği varsa hata var demektir.*/
                    if (contextFeature != null)
                    {
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            _ => StatusCodes.Status500InternalServerError
                        };
                        await context.Response.WriteAsync(new ErrorDetail() 
                        { 
                            StatusCode=context.Response.StatusCode,
                            Message=contextFeature.Error.Message
                        }.ToString());
                    }
                    /*context feature'larında hata var ise yok ise diyerek çalışma yürütüldü.*/
                });
            });
        }
    }
}