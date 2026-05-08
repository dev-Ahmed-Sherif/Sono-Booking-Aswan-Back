using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SonoBooking.Common.DTO.Integration.File;

namespace SonoBooking.Integration.FileRepository
{
    public interface IFileRepository
    {
        /// <summary>
        /// Get Tokens Using File Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<TokenDto>> GetTokens(List<Guid> ids);
    }
}

