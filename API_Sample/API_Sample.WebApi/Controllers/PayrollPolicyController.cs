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
    public class PayrollPolicyController : ControllerBase
    {
        private readonly IS_PayrollPolicy _s_PayrollPolicy;

        public PayrollPolicyController(IS_PayrollPolicy s_PayrollPolicy)
        {
            _s_PayrollPolicy = s_PayrollPolicy;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_PayrollPolicy request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_PayrollPolicy.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_PayrollPolicy request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_PayrollPolicy.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var updatedBy = User.GetAccountId();
            var res = await _s_PayrollPolicy.UpdateStatus(id, status, updatedBy);
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_PayrollPolicy.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_PayrollPolicy.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_PayrollPolicy_FullParam request)
        {
            var res = await _s_PayrollPolicy.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_PayrollPolicy_FullParam request)
        {
            var res = await _s_PayrollPolicy.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
