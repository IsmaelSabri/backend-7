using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Services;

namespace Core.Extensions
{
    /// <summary>
    /// Extensión genérica para componer imágenes en objetos DTOs.
    /// Busca propiedades de ID de imagen y las compone usando el servicio de imágenes.
    /// </summary>
    public static class ImageCompositionExtension
    {
        /// <summary>
        /// Compone imágenes en un objeto DTO de forma asincrónica.
        /// Busca propiedades con el patrón de nombre "ImageId" y las compone.
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a componer</typeparam>
        /// <param name="obj">Objeto con propiedades de ID de imagen</param>
        /// <param name="imageService">Servicio para componer imágenes</param>
        /// <returns>Tarea completada cuando termina la composición</returns>
        public static async Task ComposeImagesAsync<T>(this T obj, IImageService imageService) where T : class
        {
            if (obj == null || imageService == null) return;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                // Busca propiedades que terminen con "ImageId"
                if (prop.Name.EndsWith("ImageId") && prop.PropertyType == typeof(string))
                {
                    var imageIdValue = prop.GetValue(obj) as string;
                    
                    if (!string.IsNullOrEmpty(imageIdValue) && Guid.TryParse(imageIdValue, out var imageId))
                    {
                        // Busca la propiedad correspondiente para la imagen compuesta
                        // Por ejemplo: "ImageId" -> "Image", "ProfileImageId" -> "ProfileImage"
                        string imagePropName = prop.Name.Replace("ImageId", "Image");
                        var imageProp = obj.GetType().GetProperty(imagePropName, BindingFlags.Public | BindingFlags.Instance);

                        if (imageProp != null && imageProp.PropertyType == typeof(string))
                        {
                            try
                            {
                                var composedImage = await imageService.ComposeImageAsync(imageId);
                                if (!string.IsNullOrEmpty(composedImage))
                                {
                                    imageProp.SetValue(obj, composedImage);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log silencioso - continue procesando otras imágenes
                                Console.WriteLine($"Error composing image for {imagePropName}: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compone imágenes en una colección de objetos DTOs de forma asincrónica.
        /// </summary>
        /// <typeparam name="T">Tipo de los objetos a componer</typeparam>
        /// <param name="collection">Colección de objetos con propiedades de ID de imagen</param>
        /// <param name="imageService">Servicio para componer imágenes</param>
        /// <returns>Tarea que se completa cuando termina la composición de toda la colección</returns>
        public static async Task ComposeImagesAsync<T>(this IEnumerable<T> collection, IImageService imageService) where T : class
        {
            if (collection == null || imageService == null) return;

            var tasks = collection.Select(item => item.ComposeImagesAsync(imageService)).ToList();
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Compone imágenes en un objeto QueryableAsync de forma segura.
        /// <summary>
        /// Compone imágenes en un objeto QueryableAsync de forma segura.
        /// Convierte el IQueryable a lista antes de componer.
        /// </summary>
        /// <typeparam name="T">Tipo de los objetos a componer</typeparam>
        /// <param name="queryable">Queryable a procesar</param>
        /// <param name="imageService">Servicio para componer imágenes</param>
        /// <returns>Lista con imágenes compuestas</returns>
        public static async Task<List<T>?> ToListWithComposedImagesAsync<T>(this IQueryable<T> queryable, IImageService imageService) where T : class
        {
            if (queryable == null || imageService == null) return null;

            var list = queryable.ToList();
            await list.ComposeImagesAsync(imageService);
            return list;
        }
    }
}
