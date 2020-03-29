using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public RedisController(IRedisCacheClient redisCacheClient, IMapper mapper)
        {
            _redis = redisCacheClient;
            _mapper = mapper;
        }
        // GET: api/Redis
        [HttpGet]
        [Route("location")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Localtion>>> GetLocation(string keySearch)
        {
            try
            {
                var setString = String.Format("*{0}*",keySearch).ToString();
                var listKeys = _redis.Db0.SearchKeys(pattern: setString);
                var getValue = _redis.Db0.GetAll<string>(listKeys);
                List<string> array = new List<string>();
                List<Localtion> location = new List<Localtion>();
                if (getValue.Any())
                {
                    array.AddRange(getValue.Values);
                    array.ForEach(x => location.Add(JsonConvert.DeserializeObject<Localtion>(x)));
                }
                //var convert2Json = JsonConvert.DeserializeObject<List<Localtion>>(array.ForEach());
                //var listData = _mapper.Map<Localtion>(convert2Json);
                return Ok(location);
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
                //            var games = new List<Tuple<string, Nullable<double>>>()
                //{
                //    new Tuple<string, Nullable<double>>("Fallout 3:    $", 13.95),
                //    new Tuple<string, Nullable<double>>("GTA V:    $", 45.95),
                //    new Tuple<string, Nullable<double>>("Rocket League:    $", 19.95)
                //};

                //            games.Add(new Tuple<string, double?>("Skyrim", 15.10));
                //var list = new List<Tuple<string, Localtion>>();

                foreach (var item in localtion)
                {
                    _ = _redis.Db0.Add(item.Code.ToUpper(), JsonConvert.SerializeObject(item));
                }
                foreach (var item in localtion)
                {
                    _ = _redis.Db0.Add(item.Label.ToUpper(), JsonConvert.SerializeObject(item));
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
