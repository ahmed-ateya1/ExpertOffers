using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.CompanyDto;
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

        return _mapper.Map<IEnumerable<CompanyResponse>>(companies);
    }

    public async Task<CompanyResponse> GetByAsync(Expression<Func<Company, bool>> expression, bool isTracked = true)
    {
        var company = await _unitOfWork.Repository<Company>().GetByAsync(expression, isTracked , includeProperties: "Industrial,User,User.Country,User.City");
       
       
        return _mapper.Map<CompanyResponse>(company);
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
}
