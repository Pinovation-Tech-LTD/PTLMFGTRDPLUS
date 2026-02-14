using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY.Exceptions
{
    public class AppException : Exception
    {
        public string ErrorCode { get; }
        public HttpStatusCode StatusCode { get; }

        public AppException(string message, string errorCode, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION")
            : base(message, errorCode, HttpStatusCode.BadRequest) { }
    }
    public class NotFoundException : AppException
    {
        public NotFoundException(string resourceName, string identifier, string errorCode = "RESOURCE_NOT_FOUND")
            : base($"{resourceName} with ID {identifier} not found", errorCode, HttpStatusCode.NotFound) { }
    }
    public class DataAccessException : AppException
    {
        public string SqlCallType { get; }
        public string SqlProcedure { get; }
        public object Parameters { get; }

        public DataAccessException(string message, string sqlCallType, string sqlProcedure, 
            object parameters, Exception inner)
            : base(message, "DATA_ACCESS_ERROR", HttpStatusCode.InternalServerError)
        {
            SqlCallType = sqlCallType;
            SqlProcedure = sqlProcedure;
            Parameters = parameters;
        }
    }   
}
