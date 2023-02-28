﻿using Dapper;
using Microsoft.Extensions.Logging;
using SWTI.Interfaces.IProviders;
using SWTI.Interfaces.IRepositories;
using SWTI.Utils;
using SWTL.Models.ModelDapper;
using SWTL.Models.Requests.Partner;
using System.Data;

namespace SWTI.Partner.Domain.Repositories
{
    public class PartnerRepository : IPartnerRepository
    {
        private readonly ILogger<PartnerRepository> _logger;
        private readonly IProductIntroduceDBContext _dBContext;

        public PartnerRepository(ILogger<PartnerRepository> logger
            , IProductIntroduceDBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        public async Task<(int, BaseResponse?)> CreatePartner(CreatePartnerDapper request, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"INSERT INTO {nameof(SWTL.Models.Entities.Partners)}
                                           (Code,
                                           Name,
                                           UrlLogo,
                                           Description, 
                                           CreatedUser,
                                           Status,
                                           CreatedDate)
                                   VALUES (@Code,
                                           @Name,
                                           @UrlLogo,
                                           @Description, 
                                           @CreatedUser,
                                           0,
                                           GETDATE());";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.ExecuteScalarAsync<int>(sqlCreate, request, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository CreatePartner {ex} {request.Dump()}");
                return (-1, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public Task<(IEnumerable<SWTL.Models.Entities.Partners>, BaseResponse)> GetPartnePaging(GetPartnerPagingRequest req, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<(SWTL.Models.Entities.Partners?, BaseResponse?)> GetPartnerByCode(string code, CancellationToken cancellationToken)
        {
            try
            {
                var sqlSelect = @$"SELECT * 
                                   FROM {nameof(SWTL.Models.Entities.Partners)}
                                   WHERE Code = @Code;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryFirstOrDefaultAsync<SWTL.Models.Entities.Partners>(sqlSelect, new { Code = code }, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository GetPartnerByCode {ex} code {code}");
                return (null, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(SWTL.Models.Entities.Partners?, BaseResponse?)> GetPartnerByID(int id, CancellationToken cancellationToken)
        {
            try
            {
                var sqlSelect = @$"SELECT * 
                                   FROM {nameof(SWTL.Models.Entities.Partners)}
                                   WHERE ID = @ID;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.QueryFirstOrDefaultAsync<SWTL.Models.Entities.Partners>(sqlSelect, new { ID = id }, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository GetPartnerByCode {ex} ID ={id}");
                return (null, BaseResponseExt.Error(500, ex.Message));
            }
        }

        public async Task<(int, BaseResponse)> UpdatePartner(UpdatePartnerDapper request, CancellationToken cancellationToken)
        {
            try
            {
                var sqlCreate = @$"UPDATE {nameof(SWTL.Models.Entities.Partners)} " +
                                @" SET 
                                       Name = @Name,
                                       UrlLogo = @UrlLogo,
                                       Description = @Description, 
                                       UpdatedUser = @UpdatedUser,
                                       Status = @Status,
                                       UpdatedDate = GETDATE()
                                    WHERE ID = @PartnerId;";

                using var connection = _dBContext.CreateConnection();
                await connection.OpenAsync(cancellationToken);
                var result = await connection.ExecuteScalarAsync<int>(sqlCreate, request, commandType: CommandType.Text);
                return (result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PartnerRepository UpdatePartner {ex} {request.Dump()}");
                return (-1, BaseResponseExt.Error(500, ex.Message));
            }
        }
    }
}