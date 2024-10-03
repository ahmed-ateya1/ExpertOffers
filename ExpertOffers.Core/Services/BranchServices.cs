using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BranchDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class BranchServices : IBranchServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileServices _fileServices;

        public BranchServices(IUnitOfWork unitOfWork, IMapper mapper, IFileServices fileServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileServices = fileServices;
        }

        public async Task<BranchResponse> CreateAsync(BranchAddRequest? branchAddRequest)
        {
            if (branchAddRequest == null)
                throw new ArgumentNullException(nameof(branchAddRequest));


            ValidationHelper.ValidateModel(branchAddRequest);

            var company = await _unitOfWork.Repository<Company>().GetByAsync(x=>x.CompanyID ==  branchAddRequest.CompanyID);
            if (company == null)
                throw new ArgumentNullException(nameof(company), "Company not found");
            var branch = _mapper.Map<Branch>(branchAddRequest);

            branch.BranchID = Guid.NewGuid();
            branch.Company = company;
            if (branchAddRequest.BranchLogo != null)
            {
                await _fileServices.DeleteFile(Path.GetFileName(branch.BranchLogoURL));
                branch.BranchLogoURL = await _fileServices.CreateFile(branchAddRequest.BranchLogo);
            }

            await _unitOfWork.Repository<Branch>().CreateAsync(branch);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BranchResponse>(branch);
        }

        public async Task<bool> DeleteBranchAsync(Guid branchID)
        {
            var branch = await _unitOfWork.Repository<Branch>().GetByAsync(b => b.BranchID == branchID);

            if (branch == null)
                throw new ArgumentNullException(nameof(branch), "Branch not found");

            await _unitOfWork.Repository<Branch>().DeleteAsync(branch);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<List<BranchResponse>> GetAllAsync(Expression<Func<Branch, bool>>? expression = null)
        {
            var branches = await _unitOfWork.Repository<Branch>().GetAllAsync(expression, includeProperties: "Company");
            return _mapper.Map<List<BranchResponse>>(branches);
        }

        public async Task<BranchResponse> GetByAsync(Expression<Func<Branch, bool>> expression, bool isTracked = true)
        {
            var branch = await _unitOfWork.Repository<Branch>().GetByAsync(expression, isTracked, includeProperties: "Company");

            if (branch == null)
                throw new ArgumentNullException(nameof(branch), "Branch not found");

            return _mapper.Map<BranchResponse>(branch);
        }

        public async Task<BranchResponse> UpdateAsync(BranchUpdateRequest? branchUpdateRequest)
        {
            if (branchUpdateRequest == null)
                throw new ArgumentNullException(nameof(branchUpdateRequest));

            ValidationHelper.ValidateModel(branchUpdateRequest);

            var branch = await _unitOfWork.Repository<Branch>().GetByAsync(b => b.BranchID == branchUpdateRequest.BranchID , includeProperties: "Company");

            if (branch == null)
                throw new ArgumentNullException(nameof(branch), "Branch not found");

            _mapper.Map(branchUpdateRequest, branch);

            if (branchUpdateRequest.BranchLogo != null)
            {
                branch.BranchLogoURL = await _fileServices.CreateFile(branchUpdateRequest.BranchLogo);
            }

            await _unitOfWork.Repository<Branch>().UpdateAsync(branch);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BranchResponse>(branch);
        }
    }
}
