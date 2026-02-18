namespace Users.Extensions
{
    public static class GuidExtensions
    {
        /// <summary>
        /// Valida si un GUID es válido (no es vacío/nulo)
        /// </summary>
        /// <param name="guid">El GUID a validar</param>
        /// <returns>True si el GUID es válido, False si es vacío o por defecto</returns>
        public static bool IsValid(this Guid guid)
        {
            return guid != Guid.Empty;
        }

        /// <summary>
        /// Valida si un GUID es nulo o vacío
        /// </summary>
        /// <param name="guid">El GUID a validar</param>
        /// <returns>True si el GUID es nulo o vacío, False si es válido</returns>
        public static bool IsNullOrEmpty(this Guid? guid)
        {
            return guid == null || guid.Value == Guid.Empty;
        }
    }
}
