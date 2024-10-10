using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.IndustrialDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class IndustrialServices : IIndustrialServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndustrialServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IndustrialResponse> CreateAsync(IndustrialAddRequest? industrialAddRequest)
        {
            if (industrialAddRequest == null)
            {
                throw new ArgumentNullException(nameof(industrialAddRequest));
            }
            ValidationHelper.ValidateModel(industrialAddRequest);

            var industrial = new Industrial()
            {
                IndustrialID = Guid.NewGuid(),
                IndustrialName = industrialAddRequest.IndustrialName,
            };
            await _unitOfWork.Repository<Industrial>().CreateAsync(industrial);
            await _unitOfWork.CompleteAsync();
            return new IndustrialResponse()
            {
                IndustrialID = industrial.IndustrialID,
                IndustrialName = industrial.IndustrialName,
            };
        }

        public async Task<bool> DeleteAsync(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var industrial = await _unitOfWork.Repository<Industrial>().GetByAsync(x => x.IndustrialID == id);
            if (industrial == null)
            {
                throw new ArgumentNullException(nameof(industrial));
            }

            return await _unitOfWork.Repository<Industrial>().DeleteAsync(industrial);
        }

        public async Task<List<IndustrialResponse>> GetAllAsync(Expression<Func<Industrial, bool>>? expression = null)
        {
            var industrials = await _unitOfWork.Repository<Industrial>().GetAllAsync(expression , includeProperties: "Companies");

            industrials = industrials.OrderBy(x => x.IndustrialName).ToList();
            return industrials.Select(x => new IndustrialResponse()
            {
               IndustrialID = x.IndustrialID,
               IndustrialName = x.IndustrialName,
            }).ToList();
        }

        public async Task<IndustrialResponse> GetByAsync(Expression<Func<Industrial, bool>> expression, bool isTracked = true)
        {
             var industrial = await _unitOfWork.Repository<Industrial>().GetByAsync(expression, isTracked , includeProperties: "Companies");
            if (industrial == null)
            {
                throw new ArgumentNullException(nameof(industrial));
            }
            return new IndustrialResponse()
            {
                IndustrialID = industrial.IndustrialID,
                IndustrialName = industrial.IndustrialName,
            };

        }

        public async Task<IndustrialResponse> UpdateAsync(IndustrialUpdateRequest? industrialUpdateRequest)
        {
            if (industrialUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(industrialUpdateRequest));
            }
            ValidationHelper.ValidateModel(industrialUpdateRequest);

            var industrial = await _unitOfWork.Repository<Industrial>().GetByAsync(x => x.IndustrialID == industrialUpdateRequest.IndustrialID);
            if (industrial == null)
            {
                throw new ArgumentNullException(nameof(industrial));
            }
            industrial.IndustrialName = industrialUpdateRequest.IndustrialName;
            await _unitOfWork.Repository<Industrial>().UpdateAsync(industrial);
            await _unitOfWork.CompleteAsync();
            return new IndustrialResponse()
            {
                IndustrialID = industrial.IndustrialID,
                IndustrialName = industrial.IndustrialName,
            };
        }
    }
}
