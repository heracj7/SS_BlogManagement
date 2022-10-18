using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SS_BlogManagement.Application.Interfaces;
using SS_BlogManagement.InfraStructure.ResponseHandler;
using SS_BlogManagement.InfraStructure.Utils;
using SS_BlogManagement.Shared.Entities;
using SS_BlogManagement.Shared.Entities.InputModels;
using SS_BlogManagement.Shared.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.Controllers
{
    [Route("api/post")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IUnitWork _unitWork;
        private readonly CryptoKey _cryptoKey;
        private readonly Appsettings _appSettings;
        private readonly TokenLifeTime _tokenLifeTime;
        private readonly IResponseHandler _responseHandler;

        public PostController(IUnitWork unitWork, IOptions<Appsettings> appSettings, IOptions<TokenLifeTime> tokenLifeTime, IOptions<CryptoKey> cryptoKey, IResponseHandler responseHandler)
        {
            _unitWork = unitWork;
            _appSettings = appSettings.Value;
            _tokenLifeTime = tokenLifeTime.Value;
            _cryptoKey = cryptoKey.Value;
            _responseHandler = responseHandler;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetAllPost([FromQuery] GetSearchPost post)
        {
            try
            {
                if (checkAPIKey() == "401")
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }
                var results = _unitWork.post.GetAllPost(post);
                if (results.v_rescode == "201")
                {
                    var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R201);
                   return Ok(new { data = results.v_data });
                }
                var res = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R000);
                return StatusCode(500, new { code = res.code, message = res.message, description = results.v_resmessage });
            }
            catch (Exception ex)
            {
                var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R001);
                return BadRequest(new { code = response.code, message = response.message, description = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostByID([FromRoute] string id)
        {
            try
            {
                if (checkAPIKey() == "401")
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }
                var results = _unitWork.post.GetPostByID(id);
                if (results.v_rescode == "201")
                {
                    var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R201);
                    return Ok(new { data = results.v_data });
                }
                var res = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R000);
                return StatusCode(500, new { code = res.code, message = res.message, description = results.v_resmessage });
            }
            catch (Exception ex)
            {
                var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R001);
                return BadRequest(new { code = response.code, message = response.message, description = ex.Message });
            }
        }

        [HttpPost()]
        public async Task<IActionResult> CreatePost([FromBody] CreatePost info)
        {
            try
            {
                if (checkAPIKey() == "401")
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }
                string user_id = getUserIDFromToken();
                var results = _unitWork.post.CreatePost(info, user_id);

                if (results.v_rescode == "201")
                {
                    var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R201);
                    return Ok(new { data = results.v_data });
                }
                var res = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R000);
                return StatusCode(500, new { code = res.code, message = res.message, description = results.v_resmessage });
            }
            catch (Exception ex)
            {
                var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R001);
                return BadRequest(new { code = response.code, message = response.message, description = ex.Message });
            }
        }

        [HttpDelete()]
        public async Task<IActionResult> DeletePost([FromQuery] DeletePost info)
        {
            try
            {
                if (checkAPIKey() == "401")
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }
                var results = _unitWork.post.DeletePost(info);
                if (results.v_rescode == "201")
                {
                    var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R201);
                    return Ok(new { data = results.v_data });
                }
                var res = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R000);
                return StatusCode(500, new { code = res.code, message = res.message, description = results.v_resmessage });
            }
            catch (Exception ex)
            {
                var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R001);
                return BadRequest(new { code = response.code, message = response.message, description = ex.Message });
            }
        }

        [HttpPut()]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePost info)
        {
            try
            {
                if (checkAPIKey() == "401")
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }
                var results = _unitWork.post.UpdatePost(info);
                if (results.v_rescode == "201")
                {
                    var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R201);
                    return Ok(new { data = results.v_data });
                }
                var res = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R000);
                return StatusCode(500, new { code = res.code, message = res.message, description = results.v_resmessage });
            }
            catch (Exception ex)
            {
                var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R001);
                return BadRequest(new { code = response.code, message = response.message, description = ex.Message });
            }
        }

        private string getUserIDFromToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = HttpContext.Request.Headers["Authorization"].First().Replace("Bearer ", string.Empty);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var claims = jwtToken.Claims.First();
            var userId = jwtToken.Claims.First(x => x.Type == "User_ID").Value;
            var password = jwtToken.Claims.First(x => x.Type == "Password").Value;

            return userId;
        }

        private string checkAPIKey()
        {
            string _apiKey = HttpContext.Request.Headers["apiKey"].First();
            if (Controller_TextEncryption.Decrypt(_apiKey, _cryptoKey.Key) != _appSettings.Key)
            {
                return "401";
            }
            return "201";
        }
    }
}
