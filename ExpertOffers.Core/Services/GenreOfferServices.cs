using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.GenreOffer;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

public class GenreOfferServices : IGenreOfferServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileServices _fileServices;
    private readonly ILogger<GenreOfferServices> _logger;

    public GenreOfferServices(IUnitOfWork unitOfWork, IMapper mapper, IFileServices fileServices, ILogger<GenreOfferServices> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileServices = fileServices;
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

    public async Task<GenreResponse> CreateAsync(GenreAddRequest? genreRequest)
    {
        if (genreRequest == null)
        {
            _logger.LogWarning("CreateAsync: genreID request was null.");
            throw new ArgumentNullException(nameof(genreRequest), "genreID request cannot be null.");
        }

        ValidationHelper.ValidateModel(genreRequest);
        _logger.LogInformation("Creating new genre with name: {GenreName}", genreRequest.GenreName);

        var genre = _mapper.Map<GenreOffer>(genreRequest);
        genre.GenreID = Guid.NewGuid();

        try
        {
            genre.GenreImgURL = await _fileServices.CreateFile(genreRequest.GenreImg);
            _logger.LogInformation("genreID image uploaded successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while uploading genre image: {ErrorMessage}", ex.Message);
            throw;
        }

        await ExecuteWithTransaction(async () =>
        {
            await _unitOfWork.Repository<GenreOffer>().CreateAsync(genre);
            await _unitOfWork.CompleteAsync();
        });

        _logger.LogInformation("genreID created successfully with ID: {GenreID}", genre.GenreID);
        return _mapper.Map<GenreResponse>(genre);
    }

    public async Task<bool> DeleteAsync(Guid? id)
    {
        if (id == null)
        {
            _logger.LogWarning("DeleteAsync: genreID ID was null.");
            throw new ArgumentNullException(nameof(id), "genreID ID cannot be null.");
        }

        var genre = await _unitOfWork.Repository<GenreOffer>()
            .GetByAsync(x => x.GenreID == id,includeProperties: "Offers");
        if (genre == null)
        {
            _logger.LogWarning("DeleteAsync: genreID not found with ID: {GenreID}", id);
            throw new ArgumentNullException(nameof(genre), "genreID not found.");
        }

        bool result = false;
        await ExecuteWithTransaction(async () =>
        {
            if(genre.Offers.Any())
            {
                foreach (var offer in genre.Offers)
                {
                    string fileName = new Uri(offer.OfferPictureURL).Segments.Last();
                    await _fileServices.DeleteFile(fileName);
                }
                await _unitOfWork.Repository<Offer>().RemoveRangeAsync(genre.Offers);   
               _logger.LogInformation("Offers deleted successfully.");
            }
            if (genre.GenreImgURL != null)
            {
                string fileName = new Uri(genre.GenreImgURL).Segments.Last();
                await _fileServices.DeleteFile(fileName);
                _logger.LogInformation("genre image deleted successfully.");
            }
            
            result = await _unitOfWork.Repository<GenreOffer>().DeleteAsync(genre);
            await _unitOfWork.CompleteAsync();
        });

        _logger.LogInformation("genreID deleted successfully with ID: {GenreID}", id);
        return result;
    }

    public async Task<IEnumerable<GenreResponse>> GetAllAsync(Expression<Func<GenreOffer, bool>>? expression = null)
    {
        _logger.LogInformation("Fetching all genres with the provided expression.");
        var genres = await _unitOfWork.Repository<GenreOffer>().GetAllAsync(expression);
        genres = genres.OrderBy(x => x.GenreName);
        return _mapper.Map<IEnumerable<GenreResponse>>(genres);
    }

    public async Task<GenreResponse> GetByAsync(Expression<Func<GenreOffer, bool>> expression, bool isTracked = true)
    {
        _logger.LogInformation("Fetching genre by expression.");
        var genre = await _unitOfWork.Repository<GenreOffer>().GetByAsync(expression, isTracked);
        return _mapper.Map<GenreResponse>(genre);
    }

    public async Task<GenreResponse> UpdateAsync(GenreUpdateRequest? genreRequest)
    {
        if (genreRequest == null)
        {
            _logger.LogWarning("UpdateAsync: genreID update request was null.");
            throw new ArgumentNullException(nameof(genreRequest), "genreID update request cannot be null.");
        }

        ValidationHelper.ValidateModel(genreRequest);
        _logger.LogInformation("Updating genre with ID: {GenreID}", genreRequest.GenreID);

        var genre = await _unitOfWork.Repository<GenreOffer>().GetByAsync(x => x.GenreID == genreRequest.GenreID);
        if (genre == null)
        {
            _logger.LogWarning("UpdateAsync: genreID not found with ID: {GenreID}", genreRequest.GenreID);
            throw new ArgumentNullException(nameof(genre), "genreID not found.");
        }

        _mapper.Map(genreRequest, genre);

        if (genreRequest.GenreImg != null)
        {
            try
            {
                string fileName = new Uri(genre.GenreImgURL).Segments.Last();

                genre.GenreImgURL = await _fileServices.UpdateFile(genreRequest.GenreImg, fileName);
                _logger.LogInformation("genreID image updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating genre image: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        await ExecuteWithTransaction(async () =>
        {
            await _unitOfWork.Repository<GenreOffer>().UpdateAsync(genre);
            await _unitOfWork.CompleteAsync();
        });

        _logger.LogInformation("genreID updated successfully with ID: {GenreID}", genre.GenreID);
        return _mapper.Map<GenreResponse>(genre);
    }
}
