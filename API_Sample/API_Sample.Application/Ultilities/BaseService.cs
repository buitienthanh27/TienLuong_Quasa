using API_Sample.Data.EF;
using API_Sample.Models.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace API_Sample.Application.Ultilities
{
    public abstract class BaseService<TService>
    {
        protected readonly MainDbContext _context;
        protected readonly ILogger<TService> _logger;

        protected BaseService(MainDbContext context, ILogger<TService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Trả về lỗi logic thông thường (Result = 0)
        /// </summary>
        protected ErrorResponseBase Error(HttpStatusCode statusCode, string message)
        {
            // Kiểm tra nếu có transaction đang mở thì tự động Rollback
            if (_context.Database.CurrentTransaction != null)
                _context.Database.CurrentTransaction.Rollback();

            return new ErrorResponseBase { IsException = false, StatusCode = (int)statusCode, Message = message };
        }

        /// <summary>
        /// Trả về lỗi hệ thống/Exception (Result = -1), tự động ghi Log và Rollback Transaction nếu có
        /// </summary>
        protected ErrorResponseBase CatchException(Exception ex, string methodName, object parameters = null)
        {
            // Kiểm tra nếu có transaction đang mở thì tự động Rollback
            if (_context.Database.CurrentTransaction != null)
                _context.Database.CurrentTransaction.Rollback();

            // Serialize parameters để log để debug
            var paramStr = parameters != null ? JsonConvert.SerializeObject(parameters) : "None";

            // Tự động log lỗi theo Tên class, Tên hàm và Parameters
            _logger.LogError(ex, $"{typeof(TService).Name}.{methodName} Exception_Logger. Parameters: {paramStr}");

            return new ErrorResponseBase { IsException = true, StatusCode = (int)HttpStatusCode.InternalServerError, Message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}" };
        }
    }
}

