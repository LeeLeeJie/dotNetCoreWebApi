using Core.Model.ConfigModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helper
{
    /// <summary>
    /// 用于颁发令牌和验证令牌
    /// </summary>
   public class JwtHelper
    {
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static string IssueJWT(TokenModel tokenModel)
        {
            var dateTime = DateTime.UtcNow;
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,tokenModel.Uid.ToString()),//用户Id
                new Claim("Role", tokenModel.Role),//身份
                new Claim("Project", tokenModel.Project),//项目名称
                new Claim(JwtRegisteredClaimNames.Iat,dateTime.ToString(),ClaimValueTypes.Integer64)
            };
            //秘钥
            var jwtConfig = new JwtAuthConfigModel();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.JWTSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //过期时间
            int exp = 0;
            switch (tokenModel.TokenType)
            {
                case "Web":
                    exp = jwtConfig.WebExp;
                    break;
                case "App":
                    exp = jwtConfig.AppExp;
                    break;
                case "MiniProgram":
                    exp = jwtConfig.MiniProgramExp;
                    break;
                case "Other":
                    exp = jwtConfig.OtherExp;
                    break;
            }
            DateTime expires = DateTime.Now;
            switch (tokenModel.EffectiveTimeType)
            {
                case "year":
                    expires = expires.AddYears(exp); 
                    break;
                case "month":
                    expires = expires.AddMonths(exp);
                    break;
                case "day":
                    expires = expires.AddDays(exp);
                    break;
                case "hours":
                    expires = expires.AddHours(exp);
                    break;
                case "min":
                    expires = expires.AddMinutes(exp);
                    break;
                case "sec":
                    expires = expires.AddSeconds(exp);
                    break;
            }
            var jwt = new JwtSecurityToken(
                issuer: "CoreApi",
                claims: claims, //声明集合
                expires: expires,
                signingCredentials: creds);

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }
        /// <summary>
        /// 解析验证
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenModel SerializeJWT(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object role = new object(); ;
            object project = new object();
            try
            {
                jwtToken.Payload.TryGetValue("Role", out role);
                jwtToken.Payload.TryGetValue("Project", out project);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new TokenModel
            {
                Uid = jwtToken.Id,
                Role = role.ToString(),
                Project = project.ToString()
            };
            return tm;
        }
        /// <summary>
        /// 验证规则
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// <param name="validatePayLoad"></param>
        /// <returns></returns>
        public static bool ValidateRuleBase(string encodeJwt,out TokenModel tm, Func<Dictionary<string, object>, bool> validatePayLoad = null)
        {
            tm = null;
            var success = true;
            var jwtArr = encodeJwt.Split('.');
            var header = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwtArr[0]));
            var payLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwtArr[1]));
            var hs256 = new HMACSHA256(Encoding.ASCII.GetBytes(new JwtAuthConfigModel().JWTSecretKey));
            //首先验证签名是否正确（必须的）
            success = string.Equals(jwtArr[2], Base64UrlEncoder.Encode(hs256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(jwtArr[0], ".", jwtArr[1])))));
            if (!success)
            {
                return success;//签名不正确直接返回
            }
            //其次验证是否在有效期内（也应该必须）
            var now = ToUnixEpochDate(DateTime.UtcNow);
            //判断时间是否在nbf之前
            if (payLoad.ContainsKey("nbf")&& now < long.Parse(payLoad["nbf"].ToString()))
            {
                return false;
            }
            if (payLoad.ContainsKey("exp")&&now > long.Parse(payLoad["exp"].ToString()))
            {
                return false;
            }
            //再其次 进行自定义的验证
            success = success && validatePayLoad(payLoad);

            //最后解析出Payload
            tm=SerializeJWT(encodeJwt);
            return success;
        }
        public static long ToUnixEpochDate(DateTime date) =>
           (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 身份
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Project { get; set; }
        /// <summary>
        /// 令牌类型
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// 有效时间类型
        /// </summary>
        public string EffectiveTimeType { get; set; }
    }


}
