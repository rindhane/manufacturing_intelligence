using Microsoft.AspNetCore.Builder; //for classType:WebApplication
using Microsoft.AspNetCore.Http; // for headers.Append Extension; 

namespace App.Middleware
{
public static class CustomMiddleWare{
    public static WebApplication AddCustomMiddleware(this WebApplication app){
        app.Use(async (context, next) =>
        {
            // Do work that can write to the Response.
            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            await next.Invoke();
            // Do logging or other work that doesn't write to the Response
        });
        return app;
    }
}  
}