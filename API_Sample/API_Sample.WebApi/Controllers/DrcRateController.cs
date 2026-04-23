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
    public class DrcRateController : ControllerBase
    {
        private readonly IS_DrcRate _s_DrcRate;

        public DrcRateController(IS_DrcRate s_DrcRate)
        {
            _s_DrcRate = s_DrcRate;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_DrcRate request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_DrcRate.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_DrcRate request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_DrcRate.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_DrcRate.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_DrcRate.UpdateStatus(id, -1, User.GetAccountId());
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_DrcRate.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetByTramAndMonth(int? tramId, string yearMonth)
        {
            var res = await _s_DrcRate.GetByTramAndMonth(tramId, yearMonth);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_DrcRate_FullParam request)
        {
            var res = await _s_DrcRate.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_DrcRate_FullParam request)
        {
            var res = await _s_DrcRate.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
