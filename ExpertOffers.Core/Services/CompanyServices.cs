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
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Security.Claims;

public class CompanyServices : ICompanyServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileServices _fileServices;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CompanyServices(IUnitOfWork unitOfWork, IMapper mapper, IFileServices fileServices, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileServices = fileServices;
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
        var company = await _unitOfWork.Repository<Company>().GetByAsync(x => x.UserID == userId,includeProperties: "Industrial,User,User.Country,User.City");
        if (company == null)
            throw new UnauthorizedAccessException("Company not found");
        return company;
    }
    public async Task<IEnumerable<CompanyResponse>> GetAllAsync(Expression<Func<Company, bool>>? expression = null)
    {
        var companies = await _unitOfWork.Repository<Company>().GetAllAsync(expression , includeProperties: "Industrial,User,User.Country,User.City"); 

        var result = _mapper.Map<IEnumerable<CompanyResponse>>(companies);
        var client = await GetCurrentClientAsync();
        if(client != null)
        {
            await SetFavoriteAsync(result.ToList(), client.ClientID);
        }
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


        var user = await GetUserAsync();
        var companyUpdate = await GetComapnyByUserIdAsync(user.Id);

        if (companyUpdate == null)
        {
            throw new ArgumentNullException(nameof(companyUpdate), "Company not found.");
        }

        if (request.CompanyLogo != null)
        {
            companyUpdate.CompanyLogoURL = await _fileServices.UpdateFile(request.CompanyLogo , Path.GetFileName(companyUpdate.CompanyLogoURL));
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

    public async Task<bool> DeleteAsync(Guid CompanyID)
    {
        var company = await _unitOfWork.Repository<Company>()
            .GetByAsync(x => x.CompanyID == CompanyID,includeProperties: "Coupons,Bulletins,Branches,Offers,Notifications,Favorites");

        await ExecuteWithTransaction(async () =>
        {
            var tasks = new List<Task>();

            if (company.CompanyLogoURL != null)
            {
               tasks.Add(_fileServices.DeleteFile(Path.GetFileName(company.CompanyLogoURL)));
            }

            if(company.Coupons.Any())
            {
                foreach (var coupon in company.Coupons)
                {
                    tasks.Add(_fileServices.DeleteFile(Path.GetFileName(coupon.CouponePictureURL)));
                }
                tasks.Add(_unitOfWork.Repository<Coupon>().RemoveRangeAsync(company.Coupons));
            }
            if(company.Favorites.Any())
            {
                tasks.Add(_unitOfWork.Repository<Favorite>().RemoveRangeAsync(company.Favorites));
            }
            if(company.Bulletins.Any())
            {
                foreach (var bulletin in company.Bulletins)
                {
                    tasks.Add(_fileServices.DeleteFile(Path.GetFileName(bulletin.BulletinPdfUrl)));
                    tasks.Add(_fileServices.DeleteFile(Path.GetFileName(bulletin.BulletinPictureUrl)));
                }
                tasks.Add(_unitOfWork.Repository<Bulletin>().RemoveRangeAsync(company.Bulletins));
            }
            if (company.Branches.Any())
            {
                tasks.Add(_unitOfWork.Repository<Branch>().RemoveRangeAsync(company.Branches));
            }
            if (company.Offers.Any())
            {
                foreach (var offer in company.Offers)
                {
                    tasks.Add(_fileServices.DeleteFile(Path.GetFileName(offer.OfferPictureURL)));
                }
                tasks.Add(_unitOfWork.Repository<Offer>().RemoveRangeAsync(company.Offers));
            }
            if (company.Notifications.Any())
            {
                tasks.Add(_unitOfWork.Repository<Notification>().RemoveRangeAsync(company.Notifications));
            }
             tasks.Add(_unitOfWork.Repository<Company>().DeleteAsync(company));
            await Task.WhenAll(tasks);
        });
        return true;
    }
}
