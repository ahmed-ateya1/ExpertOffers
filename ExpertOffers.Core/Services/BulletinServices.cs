using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.BulletinDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class BulletinServices : IBulletinServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileServices _fileServices;
        private readonly ILogger<BulletinServices> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BulletinServices(
            IUnitOfWork unitOfWork,
            IFileServices fileServices,
            ILogger<BulletinServices> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _fileServices = fileServices;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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
        private async Task<Company> GetCurrentCompanyAsync()
        {
            var email = _httpContextAccessor.HttpContext
                .User.FindFirstValue(ClaimTypes.Email)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var user = await _unitOfWork.Repository<ApplicationUser>()
                .GetByAsync(u => u.Email == email)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var company = await _unitOfWork.Repository<Company>()
                .GetByAsync(c => c.UserID == user.Id)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            return company;
        }
        private async Task<BulletinGenre> CheckGenreIsValid(Guid genreID)
        {
            var genre = await _unitOfWork.Repository<BulletinGenre>()
                .GetByAsync(g => g.GenreID == genreID)?? throw new ArgumentException("Genre Not Found");
            return genre;
        }
        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host.Value}/api/";
        }
        private async Task HandleNotificationAsync(Bulletin bulletin)
        {
            var favorites = await _unitOfWork.Repository<Favorite>()
                .GetAllAsync(f => f.CompanyID == bulletin.CompanyID, includeProperties: "Client");

            var notifications = favorites.Select(f => new Notification
            {
                ClientID = f.Client.ClientID,
                BulletinId = bulletin.BulletinID,
                Message = $"New Bulletin from {bulletin.Company.CompanyName}",
                CreatedDate = DateTime.Now,
                IsRead = false,
                NotificationID = Guid.NewGuid(),
                NotificationType = NotificationOptions.NEW_BULLETIN.ToString(),
                ReferenceURL = $"{GetBaseUrl()}Bulletin/getBulletinById/{bulletin.BulletinID}"
            }).ToList();

            await _unitOfWork.Repository<Notification>().AddRangeAsync(notifications);
        }
        public async Task<BulletinResponse?> CreateAsync(BulletinAddRquest? request)
        {
            if(request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            ValidationHelper.ValidateModel(request);

            Company company = null;
            if (request.CompanyID != null)
            {
                company = await _unitOfWork.Repository<Company>().GetByAsync(c => c.CompanyID == request.CompanyID)
                    ?? throw new KeyNotFoundException("Company not found.");
            }
            else
            {
                company = await GetCurrentCompanyAsync();
            }
            var genre = await CheckGenreIsValid(request.GenreID);

            var bulletin = _mapper.Map<Bulletin>(request);

            bulletin.CompanyID = company.CompanyID;
            bulletin.Company = company;
            bulletin.Genre = genre;


            var tasks = new List<Task<string>>
            {
                _fileServices.CreateFile(request.BulletinPicture),
                _fileServices.CreateFile(request.BulletinPdf)
            };
            var files = await Task.WhenAll(tasks);
            bulletin.BulletinPictureUrl = files[0];
            bulletin.BulletinPdfUrl = files[1];

            BulletinResponse result = null;
            await ExecuteWithTransaction(async () =>
            {
                await _unitOfWork.Repository<Bulletin>().CreateAsync(bulletin);
                result = _mapper.Map<BulletinResponse>(bulletin);
                bulletin.IsActive = result.IsActive;
                await _unitOfWork.CompleteAsync();
            });
            return result;
        }

        public async Task<bool> DeleteAsync(Guid? id)
        {
            var bulletin = await _unitOfWork.Repository<Bulletin>()
                .GetByAsync(b => b.BulletinID == id ,includeProperties: "Company,Genre,Notifications")
                ?? throw new KeyNotFoundException("Bulletin not found.");

            if(bulletin.BulletinPdfUrl!=null)
            {
                string fileName = new Uri(bulletin.BulletinPdfUrl).Segments.Last();
                await _fileServices.DeleteFile(fileName);
            }
            if (bulletin.BulletinPictureUrl != null)
            {
                string fileName = new Uri(bulletin.BulletinPictureUrl).Segments.Last();
                await _fileServices.DeleteFile(fileName);
            }
            await ExecuteWithTransaction(async () =>
            {
                if (bulletin.Notifications.Any())
                {
                    await _unitOfWork.Repository<Notification>().RemoveRangeAsync(bulletin.Notifications);
                }
                await _unitOfWork.Repository<Bulletin>().DeleteAsync(bulletin);
            });
            return true;
        }

        public async Task<IEnumerable<BulletinResponse>> GetAllAsync(Expression<Func<Bulletin, bool>>? expression = null)
        {
            var bulletins = await _unitOfWork.Repository<Bulletin>()
                .GetAllAsync(expression, includeProperties: "Company,Genre");

            bulletins= bulletins.OrderByDescending(x=>x.DiscountPercentage);

            return _mapper.Map<IEnumerable<BulletinResponse>>(bulletins);
        }

        public async Task<BulletinResponse> GetByAsync(Expression<Func<Bulletin, bool>> expression, bool isTracked = false)
        {
            var bulletin = await _unitOfWork.Repository<Bulletin>()
                .GetByAsync(expression, isTracked ,includeProperties: "Company,Genre")
                ?? throw new KeyNotFoundException("Bulletin not found.");
            bulletin.TotalViews++;
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BulletinResponse>(bulletin); 
        }

        public async Task<BulletinResponse> UpdateAsync(BulletinUpdateRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ValidateModel(request);

            Company company = null;
            if (request.CompanyID != null)
            {
                company = await _unitOfWork.Repository<Company>().GetByAsync(c => c.CompanyID == request.CompanyID)
                    ?? throw new KeyNotFoundException("Company not found.");
            }
            else
            {
                company = await GetCurrentCompanyAsync();
            }
            var genre = await CheckGenreIsValid(request.GenreID);

            var bulletin = await _unitOfWork.Repository<Bulletin>()
                .GetByAsync(x => x.BulletinID == request.BulletinID, includeProperties: "Company,Genre");

         
            if (request.BulletinPdf != null)
            {
                string fileName = new Uri(bulletin.BulletinPdfUrl).Segments.Last();
                await _fileServices.UpdateFile(request.BulletinPdf,fileName);
            }
            if (request.BulletinPicture != null)
            {
                string fileName = new Uri(bulletin.BulletinPictureUrl).Segments.Last();
                await _fileServices.UpdateFile(request.BulletinPicture, fileName);
            }
            _mapper.Map(request, bulletin);
            bulletin.CompanyID = company.CompanyID;
            bulletin.Company = company;
            bulletin.Genre = genre;
            bulletin.GenreID = genre.GenreID;
            await ExecuteWithTransaction(async() =>
            {
                await _unitOfWork.Repository<Bulletin>().UpdateAsync(bulletin);
                await _unitOfWork.CompleteAsync();
            });

            return _mapper.Map<BulletinResponse>(bulletin);
        }
    }
}
