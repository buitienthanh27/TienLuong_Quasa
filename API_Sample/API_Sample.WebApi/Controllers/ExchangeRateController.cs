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
    public class ExchangeRateController : ControllerBase
    {
        private readonly IS_ExchangeRate _s_ExchangeRate;

        public ExchangeRateController(IS_ExchangeRate s_ExchangeRate)
        {
            _s_ExchangeRate = s_ExchangeRate;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_ExchangeRate request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_ExchangeRate.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_ExchangeRate request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_ExchangeRate.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Approve(int id)
        {
            var res = await _s_ExchangeRate.Approve(id, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_ExchangeRate.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_ExchangeRate.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_ExchangeRate.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetRateByMonth(string yearMonth, string fromCurrency, string toCurrency)
        {
            var res = await _s_ExchangeRate.GetRateByMonth(yearMonth, fromCurrency, toCurrency);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_ExchangeRate_FullParam request)
        {
            var res = await _s_ExchangeRate.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_ExchangeRate_FullParam request)
        {
            var res = await _s_ExchangeRate.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
