using System.Reflection;

namespace Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Recorre las propiedades públicas de un objeto y reemplaza los valores string nulos por cadena vacía.
        /// También procesa recursivamente propiedades de clase no-sistema para cubrir objetos anidados.
        /// </summary>

        public static void NormalizeNullValues(this object obj)
        {
            if (obj == null)
                return;

            var type = obj.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var propType = prop.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(propType);

                // 1. Propiedades string a string.Empty
                if (propType == typeof(string))
                {
                    var val = prop.GetValue(obj) as string;
                    if (val == null)
                    {
                        prop.SetValue(obj, string.Empty);
                    }
                }
                // 2. Propiedades numéricas anulables (int?, decimal?, double?, etc.) a 0
                else if (underlyingType != null && IsNumericType(underlyingType))
                {
                    var val = prop.GetValue(obj);
                    if (val == null)
                    {
                        // Creamos un valor '0' convertido dinámicamente al tipo subyacente correcto
                        var zeroValue = Convert.ChangeType(0, underlyingType);
                        prop.SetValue(obj, zeroValue);
                    }
                }
                // 3. Objetos complejos anidados (recursión)
                else if (propType.IsClass)
                {
                    var ns = propType.Namespace ?? string.Empty;
                    if (ns.StartsWith("System"))
                        continue;

                    var nested = prop.GetValue(obj);
                    if (nested != null)
                    {
                        NormalizeNullValues(nested);
                    }
                }
            }
        }

        // Función auxiliar para identificar si un tipo es numérico nativo
        private static bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(double) ||
                   type == typeof(decimal) || type == typeof(long) ||
                   type == typeof(float) || type == typeof(short) ||
                   type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(uint) || type == typeof(ulong) ||
                   type == typeof(ushort);
        }
    }

}