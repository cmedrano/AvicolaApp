using AvicolaApp.Services.Interfaces;
using System.Collections.ObjectModel;

namespace AvicolaApp.Services
{
    /// <summary>
    /// Servicio de provincias argentinas con caching en memoria (O(1)).
    /// Implementa el patrón Singleton para evitar múltiples instancias.
    /// Thread-safe gracias a ReadOnlyDictionary.
    /// </summary>
    public class ProvinciaService : IProvinciaService
    {
        private static readonly ReadOnlyDictionary<int, string> _provincias;
        private const string PROVINCIA_NO_ESPECIFICADA = "Provincia no especificada";

        static ProvinciaService()
        {
            // Inicializar el diccionario con las 24 provincias argentinas
            var provinciasDict = new Dictionary<int, string>
            {
                { 1, "CAPITAL FEDERAL" },
                { 2, "BUENOS AIRES" },
                { 3, "MENDOZA" },
                { 4, "CORDOBA" },
                { 5, "CORRIENTES" },
                { 6, "NEUQUEN" },
                { 7, "RIO NEGRO" },
                { 8, "ENTRE RIOS" },
                { 9, "CHUBUT" },
                { 10, "CATAMARCA" },
                { 11, "MISIONES" },
                { 12, "FORMOSA" },
                { 13, "LA PAMPA" },
                { 14, "LA RIOJA" },
                { 15, "JUJUY" },
                { 16, "SAN LUIS" },
                { 17, "SANTA FE" },
                { 18, "SALTA" },
                { 19, "CHACO" },
                { 20, "TIERRA DEL FUEGO" },
                { 21, "SANTA CRUZ" },
                { 22, "SGO. DEL ESTERO" },
                { 23, "TUCUMAN" },
                { 24, "SAN JUAN" }
            };

            _provincias = new ReadOnlyDictionary<int, string>(provinciasDict);
        }

        /// <summary>
        /// Obtiene el nombre de la provincia por su ID.
        /// Complejidad O(1) - búsqueda en diccionario.
        /// </summary>
        /// <param name="id">ID de la provincia</param>
        /// <returns>Nombre de la provincia o 'Provincia no especificada' si no existe</returns>
        public string GetNombreProvincia(int id)
        {
            return _provincias.TryGetValue(id, out var nombre) 
                ? nombre 
                : PROVINCIA_NO_ESPECIFICADA;
        }

        /// <summary>
        /// Obtiene el nombre de la provincia por su ID (nullable).
        /// Complejidad O(1) - búsqueda en diccionario.
        /// </summary>
        /// <param name="id">ID nullable de la provincia</param>
        /// <returns>Nombre de la provincia o 'Provincia no especificada' si no existe o es null</returns>
        public string GetNombreProvincia(int? id)
        {
            if (id == null || id <= 0)
                return PROVINCIA_NO_ESPECIFICADA;

            return GetNombreProvincia(id.Value);
        }
    }
}
