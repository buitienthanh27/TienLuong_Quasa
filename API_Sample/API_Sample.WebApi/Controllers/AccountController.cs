using API_Sample.Application.Services;
using API_Sample.Models.Request;
using API_Sample.WebApi.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Sample.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IS_Account _s_Account;

        public AccountController(IS_Account s_Account)
        {
            _s_Account = s_Account;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] MReq_Account_Login request)
        {
            var res = await _s_Account.Login(request);
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(MReq_Token_Refresh request)
        {
            var res = await _s_Account.RefreshToken(request);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Revoke()
        {
            var res = await _s_Account.RevokeToken(User.GetUserName());
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Account request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Account.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_Account.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatusList(string sequenceIds, short status)
        {
            var res = await _s_Account.UpdateStatusList(sequenceIds, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_Account.UpdateStatus(id, -1, User.GetAccountId());
            return Ok(res);
        }

        //[HttpDelete]
        //public async Task<IActionResult> DeleteHard(int id)
        //{
        //    var res = await _s_Account.Delete(id);
        //    return Ok(res);
        //}

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_Account.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Account_FullParam request)
        {
            var res = await _s_Account.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Account_FullParam request)
        {
            var res = await _s_Account.GetListByFullParam(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListBySpFullParam([FromQuery] MReq_Account_FullParam request)
        {
            var res = await _s_Account.GetListBySpFullParam(request);
            return Ok(res);
        }

    }
}
