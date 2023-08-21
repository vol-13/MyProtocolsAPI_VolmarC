using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyProtocolsAPI_VolmarC.Attributes
{
    //esta clase ayuda a limitar la forma en que se puede consumir un recurso de controlador (un end point). 
    //Basicamente vamos a crear una decoración personalizada que inyecta cierta fuancionalidad ya sea a todo 
    //un controller o a un end point particular. 

    [AttributeUsage(validOn: AttributeTargets.All)]
    public sealed class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        //Especificamos cuál es el clave:valor dentro de appsettings que queremos usar como ApiKey
        private readonly string _apiKey = "Progra6ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //acá validamos que en el body (en tipo json) del request vaya la info de la ApiKey
            //si no va la info presentamos un mensaje de error indicándo que falta ApiKey y que no se 
            //puede consumir el recurso. 

            if (!context.HttpContext.Request.Headers.TryGetValue(_apiKey, out var ApiSalida))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Llamada no contiene información de seguridad..."
                };
                return;
                //si no hay info de segurida sale de la funcion y muestra este mensaje

            }

            //Si viene info de seguridad falta validar que sea la correcta
            //para esto lo primero es extraer el valor de Progra6ApiKey dentro de appsettings.json
            //para poder comparar contra lo que viene en el request 
            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var ApiKeyValue = appSettings.GetValue<string>(_apiKey);

            //queda comparar que las apikey sean iguales
            if (!ApiKeyValue.Equals(ApiSalida))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "ApiKey Inválida..."
                };

                return;
            }

            await next();

        }

    }
}
