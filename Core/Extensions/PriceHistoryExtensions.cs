using Core.Collections;
using Core.Models;

namespace Core.Extensions
{
    public static class PriceHistoryExtensions
    {
        public static async Task ApplyPriceHistory(this IHomeCollection db, Home home)
        {
            var previous = await db.GetStoredHomePrices(home.Id);
            if (previous != null)
            {
                home.UpdatePrice(previous);
            }
        }

        public static void UpdatePrice(this Home home, Home previous)
        {
            switch (home.Condicion)
            {
                case "Venta":
                    home.UpdateSale(previous);
                    break;
                case "Alquiler":
                    home.UpdateRent(previous);
                    break;
                case "Alquiler y venta":
                    home.UpdateSale(previous);
                    home.UpdateRent(previous);
                    break;
            }
        }

        public static void UpdateSale(this Home home, Home previous)
        {
            if (!home.PrecioFinal.HasValue || home.PrecioFinal <= 0)
            {
                return;
            }

            var (initial, historicMin) = CalculatePriceHistory(
                home.PrecioFinal.Value, previous.PrecioFinal, previous.PrecioInicial, previous.PrecioHistoricoVenta);
            home.PrecioInicial = initial;
            home.PrecioHistoricoVenta = historicMin;
        }

        public static void UpdateRent(this Home home, Home previous)
        {
            if (!home.PrecioAlquiler.HasValue || home.PrecioAlquiler <= 0)
            {
                return;
            }

            var (initial, historicMin) = CalculatePriceHistory(
                home.PrecioAlquiler.Value, previous.PrecioAlquiler, previous.PrecioAlquilerInicial, previous.PrecioHistoricoAlquiler);
            home.PrecioAlquilerInicial = initial;
            home.PrecioHistoricoAlquiler = historicMin;
        }

        // El precio inicial es el más alto desde el último mínimo histórico.
        public static (int? Initial, decimal? HistoricMin) CalculatePriceHistory(
            int newPrice, int? previousPrice, int? previousInitial, decimal? previousMin)
        {
            if (!previousPrice.HasValue || previousPrice <= 0)
            {
                return (previousInitial, previousMin ?? newPrice);
            }

            var historicMin = previousMin ?? previousPrice.Value;

            if (newPrice < historicMin)
            {
                var peak = Math.Max(previousInitial ?? 0, previousPrice.Value);
                return (peak, newPrice);
            }

            if (newPrice > previousPrice.Value)
            {
                // Si se sube desde el mínimo empieza un nuevo tramo; si no, se mantiene el pico.
                var peak = previousPrice.Value <= historicMin
                    ? newPrice
                    : Math.Max(previousInitial ?? newPrice, newPrice);
                return (peak, historicMin);
            }

            return (previousInitial, historicMin);
        }
    }
}
