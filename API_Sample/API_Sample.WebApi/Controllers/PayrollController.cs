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
    public class PayrollController : ControllerBase
    {
        private readonly IS_Payroll _s_Payroll;

        public PayrollController(IS_Payroll s_Payroll)
        {
            _s_Payroll = s_Payroll;
        }

        [HttpPost]
        public async Task<IActionResult> Calculate([FromBody] MReq_PayrollCalculate request)
        {
            request.CalculatedBy = User.GetAccountId();
            var res = await _s_Payroll.Calculate(request);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Recalculate(int payrollId)
        {
            var res = await _s_Payroll.Recalculate(payrollId, User.GetAccountId());
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailById(int payrollId)
        {
            var res = await _s_Payroll.GetDetailById(payrollId);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Payroll_FullParam request)
        {
            var res = await _s_Payroll.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByYearMonth(string yearMonth)
        {
            var res = await _s_Payroll.GetListByYearMonth(yearMonth);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePayrollStatus(int id, string payrollStatus)
        {
            var res = await _s_Payroll.UpdatePayrollStatus(id, payrollStatus, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePayrollStatusBulk([FromBody] List<int> ids, [FromQuery] string payrollStatus)
        {
            var res = await _s_Payroll.UpdatePayrollStatusBulk(ids, payrollStatus, User.GetAccountId());
            return Ok(res);
        }
    }
}
