using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImagesConverter.JsonModels
{
    public class JsonResponse
    {
        public bool Success { get; set; }

        public object Data { get; set; }

        public string Message { get; set; }

        public static JsonResponse Ok(object data)
        {
            return new JsonResponse()
            {
                Success = true,
                Data = data,
                Message = null
            };
        }

        public static JsonResponse Ok()
        {
            return Ok(1);
        }

        public static JsonResponse Fail(string message)
        {
            return new JsonResponse()
            {
                Success = false,
                Data = null,
                Message = message
            };
        }
    }
}