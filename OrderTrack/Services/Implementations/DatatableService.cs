using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.DTO.Request;
using OrderTrack.Services.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace OrderTrack.Services.Implementations
{
    public class DatatableService : IDatatableService
    {
        public IQueryable<T> AplicarFiltrosGenericos<T>(IQueryable<T> query, PaginacionRequestDto filtro, string[]? searchFields = null)
        {
            // Búsqueda dinámica
            if (!string.IsNullOrWhiteSpace(filtro.SearchTerm) && searchFields != null && searchFields.Any())
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                Expression? combined = null;
                var searchValue = Expression.Constant(filtro.SearchTerm.ToLower());

                foreach (var field in searchFields)
                {
                    var property = Expression.PropertyOrField(parameter, field);
                    var propertyType = property.Type;

                    if (propertyType == typeof(string))
                    {
                        var toLowerCall = Expression.Call(property, "ToLower", null);
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var containsCall = Expression.Call(toLowerCall, containsMethod!, searchValue);
                        combined = combined == null ? containsCall : Expression.OrElse(combined, containsCall);
                    }
                    else
                    {
                        if (propertyType.IsValueType || propertyType == typeof(string))
                        {
                            var toStringCall = Expression.Call(property, "ToString", null);
                            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            var containsCall = Expression.Call(toStringCall, containsMethod!, searchValue);
                            combined = combined == null ? containsCall : Expression.OrElse(combined, containsCall);
                        }
                    }
                }

                var lambda = Expression.Lambda<Func<T, bool>>(combined!, parameter);
                query = query.Where(lambda);
            }

            // Ordenamiento dinámico
            if (!string.IsNullOrWhiteSpace(filtro.SortColumn))
            {
                var propertyInfo = typeof(T).GetProperty(filtro.SortColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.PropertyOrField(parameter, propertyInfo.Name); // Asegura el uso del nombre correcto
                    var lambda = Expression.Lambda(property, parameter);

                    var methodName = filtro.SortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2);
                    var genericMethod = method.MakeGenericMethod(typeof(T), property.Type);

                    query = (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, lambda })!;
                }
            }

            return query;
        }


    }
}
