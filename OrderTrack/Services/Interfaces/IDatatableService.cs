using OrderTrack.Models.DTO.Request;

namespace OrderTrack.Services.Interfaces
{
    public interface IDatatableService
    {
        public IQueryable<T> AplicarFiltrosGenericos<T>(IQueryable<T> query, PaginacionRequestDto filtro, string[]? searchFields = null);
    }
}
