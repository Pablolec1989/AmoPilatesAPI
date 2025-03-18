using Microsoft.EntityFrameworkCore;

namespace AmoPilates.Utilidades
{
    public static class HttpContextExtensions
    {
        public async static Task insertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync(); //Buscamos la cantidad de registros
            httpContext
                .Response
                .Headers
                .Append("cantidad-total-registros", cantidad.ToString()); //incorporo el dato en la cabecera
        }
    }
}
