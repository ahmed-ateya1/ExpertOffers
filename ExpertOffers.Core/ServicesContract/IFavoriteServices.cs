using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.FavoriteDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    /// <summary>
    /// Represents the interface for managing favorite services.
    /// </summary>
    public interface IFavoriteServices
    {
        /// <summary>
        /// Adds a favorite.
        /// </summary>
        /// <param name="favoriteAdd">The favorite to add.</param>
        /// <returns>The added favorite response.</returns>
        Task<FavoriteResponse> AddFavorite(FavoriteAddRequest? favoriteAdd);

        /// <summary>
        /// Removes a favorite.
        /// </summary>
        /// <param name="favoriteID">The ID of the favorite to remove.</param>
        /// <returns>A boolean indicating whether the favorite was successfully removed.</returns>
        Task<bool> RemoveFavorite(Guid? favoriteID);

        /// <summary>
        /// Gets all favorites.
        /// </summary>
        /// <param name="expression">The expression to filter the favorites (optional).</param>
        /// <returns>A collection of favorite responses.</returns>
        Task<IEnumerable<FavoriteResponse>> GetAllAsync(Expression<Func<Favorite, bool>>? expression = null);

        /// <summary>
        /// Gets a favorite by expression.
        /// </summary>
        /// <param name="expression">The expression to filter the favorite.</param>
        /// <param name="isTracked">A boolean indicating whether the favorite should be tracked (optional).</param>
        /// <returns>The matching favorite response.</returns>
        Task<FavoriteResponse> GetByAsync(Expression<Func<Favorite, bool>>? expression = null, bool isTracked = false);
    }
}
