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
    public class CostCenterController : ControllerBase
    {
        private readonly IS_CostCenter _s_CostCenter;

        public CostCenterController(IS_CostCenter s_CostCenter)
        {
            _s_CostCenter = s_CostCenter;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_CostCenter request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_CostCenter.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_CostCenter request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_CostCenter.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_CostCenter.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_CostCenter.UpdateStatus(id, -1, User.GetAccountId());
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_CostCenter.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_CostCenter_FullParam request)
        {
            var res = await _s_CostCenter.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_CostCenter_FullParam request)
        {
            var res = await _s_CostCenter.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
