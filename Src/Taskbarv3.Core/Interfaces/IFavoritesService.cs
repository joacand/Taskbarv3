namespace Taskbarv3.Core.Interfaces
{
    public interface IFavoritesService
    {
        bool PlayFavorites();
        bool AddToFavorites();
        void Reset();
        void ClearFavorites();
    }
}
