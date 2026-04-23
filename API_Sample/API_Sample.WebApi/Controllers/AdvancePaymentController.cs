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
    public class AdvancePaymentController : ControllerBase
    {
        private readonly IS_AdvancePayment _s_AdvancePayment;

        public AdvancePaymentController(IS_AdvancePayment s_AdvancePayment)
        {
            _s_AdvancePayment = s_AdvancePayment;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_AdvancePayment request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_AdvancePayment.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_AdvancePayment request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_AdvancePayment.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> MarkAsDeducted(int id, int payrollId)
        {
            var res = await _s_AdvancePayment.MarkAsDeducted(id, payrollId, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_AdvancePayment.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_AdvancePayment.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_AdvancePayment.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingByEmployee(int employeeId)
        {
            var res = await _s_AdvancePayment.GetPendingByEmployee(employeeId);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_AdvancePayment_FullParam request)
        {
            var res = await _s_AdvancePayment.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_AdvancePayment_FullParam request)
        {
            var res = await _s_AdvancePayment.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
