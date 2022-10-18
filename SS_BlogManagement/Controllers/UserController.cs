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
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly IUnitWork _unitWork;
        private readonly CryptoKey _cryptoKey;
        private readonly Appsettings _appSettings;
        private readonly TokenLifeTime _tokenLifeTime;
        private readonly IResponseHandler _responseHandler;
        public UserController(IUnitWork unitWork, IOptions<Appsettings> appSettings, IOptions<TokenLifeTime> tokenLifeTime, IOptions<CryptoKey> cryptoKey, IResponseHandler responseHandler)
        {
            _unitWork = unitWork;
            _appSettings = appSettings.Value;
            _tokenLifeTime = tokenLifeTime.Value;
            _cryptoKey = cryptoKey.Value;
            _responseHandler = responseHandler;
        }


        [AllowAnonymous]
        [HttpPost("createuser")]
        public async Task<IActionResult> CreateUser([FromBody] RegisteredUser info)
        {
            try
            {
                string _apiKey = HttpContext.Request.Headers["apiKey"].First();
                if (Controller_TextEncryption.Decrypt(_apiKey, _cryptoKey.Key) != _appSettings.Key)
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }
               
                info.Password = Controller_TextEncryption.Encrypt(info.Password, _cryptoKey.Key);
                var results = _unitWork.user.CreateUser(info);
                if (results.v_rescode == "201")  //Success
                {
                    dynamic data = results.v_data;
                    GenerateToken genetate_token = new GenerateToken();
                    genetate_token.UserID = data.GetType().GetProperty("UserID").GetValue(data, null); 
                    genetate_token.Password = data.GetType().GetProperty("PasswordHash").GetValue(data, null);
                    var token = generateJwtToken(genetate_token);

                    var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R201);
                    return Ok(new { code = response.code, message = response.message, data = token });
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
        [HttpPost("loginuser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUser info)
        {
            try
            {
                string _apiKey = HttpContext.Request.Headers["apiKey"].First();
                if (Controller_TextEncryption.Decrypt(_apiKey, _cryptoKey.Key) != _appSettings.Key)
                {
                    var errorHandler = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R401);
                    return Unauthorized(errorHandler);
                }

                string enc_key = _cryptoKey.Key;
                info.Password = Controller_TextEncryption.Encrypt(info.Password, _cryptoKey.Key);
                var results = _unitWork.user.LoginUser(info);
                if (results.v_rescode == "201")  //Success
                {
                    GenerateToken genetate_token = new GenerateToken();
                    genetate_token.UserID = results.v_data.UserID;
                    genetate_token.Password = results.v_data.PasswordHash;
                    var token = generateJwtToken(genetate_token);

                    var response = _responseHandler.GetResponse<ResponseData>(ResponseEnum.R201);
                    return Ok(new { code = response.code, message = response.message, data = token });
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

        private string generateJwtToken(GenerateToken info)
        {
            
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
           
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                
                Subject = new ClaimsIdentity(new []
                {
                     new Claim("User_ID", info.UserID),
                     new Claim("Password", info.Password),
                }),

                Expires = DateTime.Now.AddDays(_tokenLifeTime.Time),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

       
    }
}
