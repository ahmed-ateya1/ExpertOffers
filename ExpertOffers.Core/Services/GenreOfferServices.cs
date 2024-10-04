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
            _logger.LogWarning("CreateAsync: Genre request was null.");
            throw new ArgumentNullException(nameof(genreRequest), "Genre request cannot be null.");
        }

        ValidationHelper.ValidateModel(genreRequest);
        _logger.LogInformation("Creating new genre with name: {GenreName}", genreRequest.GenreName);

        var genre = _mapper.Map<GenreOffer>(genreRequest);
        genre.GenreID = Guid.NewGuid();

        try
        {
            genre.GenreImgURL = await _fileServices.CreateFile(genreRequest.GenreImg);
            _logger.LogInformation("Genre image uploaded successfully.");
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

        _logger.LogInformation("Genre created successfully with ID: {GenreID}", genre.GenreID);
        return _mapper.Map<GenreResponse>(genre);
    }

    public async Task<bool> DeleteAsync(Guid? id)
    {
        if (id == null)
        {
            _logger.LogWarning("DeleteAsync: Genre ID was null.");
            throw new ArgumentNullException(nameof(id), "Genre ID cannot be null.");
        }

        var genre = await _unitOfWork.Repository<GenreOffer>().GetByAsync(x => x.GenreID == id);
        if (genre == null)
        {
            _logger.LogWarning("DeleteAsync: Genre not found with ID: {GenreID}", id);
            throw new ArgumentNullException(nameof(genre), "Genre not found.");
        }

        bool result = false;
        await ExecuteWithTransaction(async () =>
        {
            result = await _unitOfWork.Repository<GenreOffer>().DeleteAsync(genre);
            await _unitOfWork.CompleteAsync();
        });

        _logger.LogInformation("Genre deleted successfully with ID: {GenreID}", id);
        return result;
    }

    public async Task<IEnumerable<GenreResponse>> GetAllAsync(Expression<Func<GenreOffer, bool>>? expression = null)
    {
        _logger.LogInformation("Fetching all genres with the provided expression.");
        var genres = await _unitOfWork.Repository<GenreOffer>().GetAllAsync(expression);
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
            _logger.LogWarning("UpdateAsync: Genre update request was null.");
            throw new ArgumentNullException(nameof(genreRequest), "Genre update request cannot be null.");
        }

        ValidationHelper.ValidateModel(genreRequest);
        _logger.LogInformation("Updating genre with ID: {GenreID}", genreRequest.GenreID);

        var genre = await _unitOfWork.Repository<GenreOffer>().GetByAsync(x => x.GenreID == genreRequest.GenreID);
        if (genre == null)
        {
            _logger.LogWarning("UpdateAsync: Genre not found with ID: {GenreID}", genreRequest.GenreID);
            throw new ArgumentNullException(nameof(genre), "Genre not found.");
        }

        _mapper.Map(genreRequest, genre);

        if (genreRequest.GenreImg != null)
        {
            try
            {
                genre.GenreImgURL = await _fileServices.UpdateFile(genreRequest.GenreImg, Path.GetFileName(genre.GenreImgURL));
                _logger.LogInformation("Genre image updated successfully.");
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

        _logger.LogInformation("Genre updated successfully with ID: {GenreID}", genre.GenreID);
        return _mapper.Map<GenreResponse>(genre);
    }
}
