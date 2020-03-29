using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Resdis.Controllers
{
    [Route("api/Redis")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IRedisCacheClient _redis;
        public RedisController(IRedisCacheClient redisCacheClient)
        {
            _redis = redisCacheClient;
        }
        // GET: api/Redis
        [HttpGet]
        [Route("location")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Localtion>> GetLocation(string keySearch)
        {
            try
            {
                //var getKeys = _database.HashGet(keySearch);
                var setString = String.Format("*{0}*",keySearch).ToString();
                //var res = _database.StringGet(keySearch);
                //var keysSearch = _server.Keys(pattern: setString).ToArray();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost]
        [Route("location")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SetLocaltion([FromBody] List<Localtion> localtion)
        {
            try
            {
                foreach (var item in localtion)
                {
                    _ = _redis.Db0.SetAdd(item.Label, JsonConvert.SerializeObject(item));
                }
                return Ok();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, ex.Message);
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        //public static HashEntry[] ToHashEntries(Localtion obj)
        //{
        //    PropertyInfo[] properties = obj.GetType().GetProperties();
        //    return properties
        //        .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
        //        .Select
        //        (
        //              property =>
        //              {
        //                  object propertyValue = property.GetValue(obj);
        //                  string hashValue;

        //          // This will detect if given property value is 
        //          // enumerable, which is a good reason to serialize it
        //          // as JSON!
        //          if (propertyValue is IEnumerable<object>)
        //                  {
        //              // So you use JSON.NET to serialize the property
        //              // value as JSON
        //              hashValue = JsonConvert.SerializeObject(propertyValue);
        //                  }
        //                  else
        //                  {
        //                      hashValue = propertyValue.ToString();
        //                  }

        //                  return new HashEntry(property.Name, hashValue);
        //              }
        //        )
        //        .ToArray();
        //}

    }
}
