using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BulletinGenreDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class BulletinGenreServices : IBulletinGenreServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BulletinGenreServices> _logger;
        private readonly IMapper _mapper;
        public BulletinGenreServices(
            IUnitOfWork unitOfWork,
            ILogger<BulletinGenreServices> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        private async Task ExecuteWithTransaction(Func<Task> action)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await action();
                    await _unitOfWork.CommitTransactionAsync();
                    _logger.LogInformation("Transaction committed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Transaction failed: {ErrorMessage}", ex.Message);
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }
        public async Task<BulletinGenreResponse> CreateAsync(BulletinGenreAddRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ValidateModel(request);
            var genre = _mapper.Map<BulletinGenre>(request);

            await ExecuteWithTransaction(async() =>
            {
                await _unitOfWork.Repository<BulletinGenre>()
                .CreateAsync(genre);
                await _unitOfWork.CompleteAsync();
            });
            return _mapper.Map<BulletinGenreResponse>(genre);
        }

        public async Task<bool> DeleteAsync(Guid? genreID)
        {
            if(genreID == null)
                throw new ArgumentNullException(nameof(genreID));

            var genre = await _unitOfWork.Repository<BulletinGenre>()
                .GetByAsync(x=>x.GenreID == genreID,includeProperties: "Bulletins");
            if(genre == null)
                throw new ArgumentNullException(nameof(genre));

            var result = false;

            await ExecuteWithTransaction(async () =>
            {
                if(genre.Bulletins.Any())
                {
                    await _unitOfWork.Repository<Bulletin>()
                    .RemoveRangeAsync(genre.Bulletins);
                }
                result = await _unitOfWork.Repository<BulletinGenre>().DeleteAsync(genre);
                await _unitOfWork.CompleteAsync();
            });
            return result;
        }

        public async Task<IEnumerable<BulletinGenreResponse>> GetAllAsync(Expression<Func<BulletinGenre, bool>>? expression = null)
        {
            var result = await _unitOfWork.Repository<BulletinGenre>()
                .GetAllAsync(expression);
            return _mapper.Map<IEnumerable<BulletinGenreResponse>>(result);
        }

        public async Task<BulletinGenreResponse> GetByAsync(Expression<Func<BulletinGenre, bool>> expression, bool isTracked = false)
        {
            var result = await _unitOfWork.Repository<BulletinGenre>()
                .GetByAsync(expression, isTracked);
            return _mapper.Map<BulletinGenreResponse>(result);
        }

        public async Task<BulletinGenreResponse> UpdateAsync(BulletinGenreUpdateRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            var genre = await _unitOfWork.Repository<BulletinGenre>()
                .GetByAsync(x=>x.GenreID == request.GenreID);
            if (genre == null)
                throw new ArgumentNullException(nameof(genre));

            _mapper.Map(request, genre);

            await ExecuteWithTransaction(async () =>
            {
                await _unitOfWork.Repository<BulletinGenre>()
                .UpdateAsync(genre);
                await _unitOfWork.CompleteAsync();
            });
            return _mapper.Map<BulletinGenreResponse>(genre);
        }
    }
}
