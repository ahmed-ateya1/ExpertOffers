using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.CompanyDto;
using ExpertOffers.Core.Dtos.FavoriteDto;
using ExpertOffers.Core.DTOS.ClientDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Security.Claims;
using static Azure.Core.HttpHeader;

public class CompanyServices : ICompanyServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileServices _fileServices;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CompanyServices> _logger;

    public CompanyServices(IUnitOfWork unitOfWork, IMapper mapper, IFileServices fileServices, IHttpContextAccessor httpContextAccessor, ILogger<CompanyServices> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileServices = fileServices;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    private async Task ExecuteWithTransaction(Func<Task> action)
    {
        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                await action();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
    private async Task SetFavoriteAsync(List<CompanyResponse> companies, Guid clientID)
    {
        var companyIds = companies.Select(c => c.CompanyID).ToList();

        var favoriteCompanyIds = await _unitOfWork.Repository<Favorite>()
            .GetAllAsync(x => x.ClientID == clientID && companyIds.Contains(x.CompanyID));

        var ids = favoriteCompanyIds.Select(x => x.CompanyID).ToList();

        var favoriteIdsSet = new HashSet<Guid>(ids);

        foreach (var company in companies)
        {
            company.IsFavoriteToCurrentUser = favoriteIdsSet.Contains(company.CompanyID);
        }
    }

    private async Task<Client?> GetCurrentClientAsync()
    {
        var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);

        if (email == null)
            return null;

        var user = await _unitOfWork.Repository<ApplicationUser>()
            .GetByAsync(x => x.Email == email, includeProperties: "Country,City");

        if (user == null)
            return null;

        var client = await _unitOfWork.Repository<Client>()
            .GetByAsync(x => x.ClientID == user.ClientID);

        return client;
    }
    private async Task<City> GetCityByIdAsync(Guid cityId)
    {
        var city = await _unitOfWork.Repository<City>().GetByAsync(x => x.CityID == cityId);
        if (city == null)
            throw new UnauthorizedAccessException("City not found");
        return city;
    }

    private async Task<Country> GetCountryByIdAsync(Guid countryId)
    {
        var country = await _unitOfWork.Repository<Country>().GetByAsync(x => x.CountryID == countryId);
        if (country == null)
            throw new UnauthorizedAccessException("Country not found");
        return country;
    }
    private async Task<ApplicationUser> GetUserAsync()
    {
        var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            throw new UnauthorizedAccessException("User is not authenticated");

        var user = await _unitOfWork.Repository<ApplicationUser>()
            .GetByAsync(x => x.Email == email, includeProperties: "Country,City");

        if (user == null)
            throw new UnauthorizedAccessException("User is not authenticated");

        return user;
    }
    private async Task<Company> GetComapnyByUserIdAsync(Guid userId)
    {
        var company = await _unitOfWork.Repository<Company>().GetByAsync(x => x.UserID == userId,includeProperties: "Industrial,User,User.Country,User.City,Bulletins");
        if (company == null)
            throw new UnauthorizedAccessException("Company not found");
        return company;
    }
    public async Task<IEnumerable<CompanyResponse>> GetAllAsync(Expression<Func<Company, bool>>? expression = null)
    {
        var companies = await _unitOfWork.Repository<Company>().GetAllAsync(expression , includeProperties: "Industrial,User,User.Country,User.City,Bulletins"); 

        var result = _mapper.Map<IEnumerable<CompanyResponse>>(companies);
        var client = await GetCurrentClientAsync();
        if(client != null)
        {
            await SetFavoriteAsync(result.ToList(), client.ClientID);
        }
        result = result.OrderByDescending(x => x.NumberOfBulletins);
        return result;
    }

    public async Task<CompanyResponse> GetByAsync(Expression<Func<Company, bool>> expression, bool isTracked = true)
    {
        var company = await _unitOfWork.Repository<Company>().GetByAsync(expression, isTracked , includeProperties: "Industrial,User,User.Country,User.City");
       
        var result = _mapper.Map<CompanyResponse>(company);
        var user = await GetCurrentClientAsync();
        if (user != null)
        {
            result.IsFavoriteToCurrentUser = await _unitOfWork.Repository<Favorite>()
                .AnyAsync(x => x.CompanyID == company.CompanyID && x.ClientID == user.ClientID);
        }
        return result;
    }

    public async Task<CompanyResponse> UpdateAsync(CompanyUpdateRequest? request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        ValidationHelper.ValidateModel(request);
        var country = await GetCountryByIdAsync(request.CountryID);

        var IsExistInCountry = await _unitOfWork.Repository<City>()
            .GetByAsync(x => x.CountryID == request.CountryID);
        if (IsExistInCountry == null)
        {
            throw new ArgumentNullException(nameof(IsExistInCountry), "City not found in this country.");
        }
        var industrial = await _unitOfWork.Repository<Industrial>()
            .GetByAsync(x => x.IndustrialID == request.IndustrialID) ?? throw new ArgumentNullException("Industrial not found.");

        var user = await GetUserAsync();
        var companyUpdate = await GetComapnyByUserIdAsync(user.Id);

        if (companyUpdate == null)
        {
            throw new ArgumentNullException(nameof(companyUpdate), "Company not found.");
        }

        if (request.CompanyLogo != null)
        {
            string fileName = new Uri(companyUpdate.CompanyLogoURL).Segments.Last();
            companyUpdate.CompanyLogoURL = await _fileServices.UpdateFile(request.CompanyLogo , fileName);
        }


        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                user.CityID = request.CityID;
                user.City = IsExistInCountry;
                user.Country = country;
                user.CountryID = request.CountryID;
                user.PhoneNumber = request.PhoneNumber;
                _mapper.Map(request, companyUpdate);

                companyUpdate.User = user;

                await _unitOfWork.Repository<Company>().UpdateAsync(companyUpdate);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        return _mapper.Map<CompanyResponse>(companyUpdate);
    }

    public async Task<bool> DeleteAsync(Guid companyID)
    {
        var company = await _unitOfWork.Repository<Company>()
            .GetByAsync(
                x => x.CompanyID == companyID,
                includeProperties: "Coupons,Bulletins,Branches,Offers,Notifications,Favorites,User"
            );

        if (company == null)
            throw new ArgumentException("Company not found.");

        await ExecuteWithTransaction(async () =>
        {
            await DeleteFileAsync(company.CompanyLogoURL);

            if (company.Coupons?.Any() == true)
            {
                foreach (var coupon in company.Coupons)
                {
                    await DeleteNotificationsAsync(
                        await _unitOfWork.Repository<Notification>()
                            .GetAllAsync(n => n.CouponId == coupon.CouponID)
                    );
                    await DeleteFileAsync(coupon.CouponePictureURL);
                }

                await _unitOfWork.Repository<Coupon>().RemoveRangeAsync(company.Coupons);
            }

            if (company.Favorites?.Any() == true)
            {
                await _unitOfWork.Repository<Favorite>().RemoveRangeAsync(company.Favorites);
            }

            if (company.Bulletins?.Any() == true)
            {
                foreach (var bulletin in company.Bulletins)
                {
                    await DeleteNotificationsAsync(
                        await _unitOfWork.Repository<Notification>()
                            .GetAllAsync(n => n.BulletinId == bulletin.BulletinID)
                    );
                    await DeleteFileAsync(bulletin.BulletinPictureUrl);
                    await DeleteFileAsync(bulletin.BulletinPdfUrl);
                }

                await _unitOfWork.Repository<Bulletin>().RemoveRangeAsync(company.Bulletins);
            }

            if (company.Branches?.Any() == true)
            {
                await _unitOfWork.Repository<Branch>().RemoveRangeAsync(company.Branches);
            }

            if (company.Offers?.Any() == true)
            {
                foreach (var offer in company.Offers)
                {
                    await DeleteNotificationsAsync(
                        await _unitOfWork.Repository<Notification>()
                            .GetAllAsync(n => n.OfferId == offer.OfferID)
                    );
                    await DeleteFileAsync(offer.OfferPictureURL);
                }

                await _unitOfWork.Repository<Offer>().RemoveRangeAsync(company.Offers);
            }

            if (company.Notifications?.Any() == true)
            {
                await _unitOfWork.Repository<Notification>().RemoveRangeAsync(company.Notifications);
            }

            await _unitOfWork.Repository<Company>().DeleteAsync(company);
        });

        if (company.User != null)
        {
            await _unitOfWork.Repository<ApplicationUser>().DeleteAsync(company.User);
        }

        return true;
    }

    private async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        try
        {
            var fileName = new Uri(fileUrl).Segments.Last();
            await _fileServices.DeleteFile(fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete file: {fileUrl}");
        }
    }

    private async Task DeleteNotificationsAsync(IEnumerable<Notification> notifications)
    {
        if (notifications?.Any() == true)
        {
            await _unitOfWork.Repository<Notification>().RemoveRangeAsync(notifications);
        }
    }



    public async Task<bool> CreateAsync(CompanyAddRequest? request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        ValidationHelper.ValidateModel(request);
        var user = await GetUserAsync();
        var file = await _fileServices.CreateFile(request.CompanyLogo);
        var company = new Company
        {
            UserID = user.Id,
            CompanyName = request.CompanyName,
            IndustrialID = request.IndustrialID,
            CompanyLogoURL = file,
            CompanyID = Guid.NewGuid(),
        };
        await ExecuteWithTransaction(async () =>
        {
            await _unitOfWork.Repository<Company>().CreateAsync(company);
        });
        return true;
    }
}
