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
    public class PayrollReconciliationController : ControllerBase
    {
        private readonly IS_PayrollReconciliation _s_PayrollReconciliation;

        public PayrollReconciliationController(IS_PayrollReconciliation s_PayrollReconciliation)
        {
            _s_PayrollReconciliation = s_PayrollReconciliation;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReconciliation(string yearMonth, int? tramId)
        {
            var res = await _s_PayrollReconciliation.GenerateReconciliation(yearMonth, tramId, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Balance(int id)
        {
            var res = await _s_PayrollReconciliation.Balance(id, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Lock(int id)
        {
            var res = await _s_PayrollReconciliation.Lock(id, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Unlock(int id)
        {
            var res = await _s_PayrollReconciliation.Unlock(id, User.GetAccountId());
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_PayrollReconciliation.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_PayrollReconciliation_FullParam request)
        {
            var res = await _s_PayrollReconciliation.GetListByPaging(request);
            return Ok(res);
        }
    }
}
