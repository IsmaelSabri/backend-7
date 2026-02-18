using System;
using System.Reflection;

namespace Homes.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Recorre las propiedades públicas de un objeto y reemplaza los valores string nulos por cadena vacía.
        /// También procesa recursivamente propiedades de clase no-sistema para cubrir objetos anidados.
        /// </summary>
        public static void NormalizeNullStrings(this object obj)
        {
            if (obj == null) return;

            var type = obj.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite) continue;

                // Propiedades string
                if (prop.PropertyType == typeof(string))
                {
                    var val = prop.GetValue(obj) as string;
                    if (val == null)
                    {
                        prop.SetValue(obj, string.Empty);
                    }
                }
                else if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                {
                    // Evitar procesar tipos del sistema (System.*) que no tienen sentido recorrer
                    var ns = prop.PropertyType.Namespace ?? string.Empty;
                    if (ns.StartsWith("System")) continue;

                    var nested = prop.GetValue(obj);
                    if (nested != null)
                    {
                        // Llamada recursiva para objetos anidados
                        NormalizeNullStrings(nested);
                    }
                }
            }
        }
    }
}
