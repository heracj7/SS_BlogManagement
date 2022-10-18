using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class PostCategoryController : ControllerBase
    {
        private readonly IUnitWork _unitWork;
        private readonly CryptoKey _cryptoKey;
        private readonly Appsettings _appSettings;
        private readonly TokenLifeTime _tokenLifeTime;
        private readonly IResponseHandler _responseHandler;
        

        public PostCategoryController(IUnitWork unitWork, IOptions<Appsettings> appSettings, IOptions<TokenLifeTime> tokenLifeTime, IOptions<CryptoKey> cryptoKey, IResponseHandler responseHandler)
        {
            _unitWork = unitWork;
            _appSettings = appSettings.Value;
            _tokenLifeTime = tokenLifeTime.Value;
            _cryptoKey = cryptoKey.Value;
            _responseHandler = responseHandler;
         
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllPostCategory()
        {
            try
            {
                string _apiKey = HttpContext.Request.Headers["apiKey"].First();
                if (Controller_TextEncryption.Decrypt(_apiKey, _cryptoKey.Key) != _appSettings.Key)
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }

                var results = _unitWork.postCategory.GetAllPostCategory();
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
        public async Task<IActionResult> CreatePostCategory([FromBody] CreatePostCategory info)
        {
            try
            {
                string _apiKey = HttpContext.Request.Headers["apiKey"].First();
                if (Controller_TextEncryption.Decrypt(_apiKey, _cryptoKey.Key) != _appSettings.Key)
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }

                var results = _unitWork.postCategory.CreatePostCategory(info);
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
        public async Task<IActionResult> DeletePostCategory([FromQuery] DeletePostCategory info)
        {
            try
            {
                string _apiKey = HttpContext.Request.Headers["apiKey"].First();
                if (Controller_TextEncryption.Decrypt(_apiKey, _cryptoKey.Key) != _appSettings.Key)
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }

                var results = _unitWork.postCategory.DeletePostCategory(info);
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
        public async Task<IActionResult> UpdatePostCategory([FromBody] UpdatePostCategory info)
        {
            try
            {
                string _apiKey = HttpContext.Request.Headers["apiKey"].First();
                if (Controller_TextEncryption.Decrypt(_apiKey, _cryptoKey.Key) != _appSettings.Key)
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return BadRequest(errorHandler);
                }

                var results = _unitWork.postCategory.UpdatePostCategory(info);
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

       
    }
}
