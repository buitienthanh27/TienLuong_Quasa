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
    public class EmployeeController : ControllerBase
    {
        private readonly IS_Employee _s_Employee;

        public EmployeeController(IS_Employee s_Employee)
        {
            _s_Employee = s_Employee;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Employee request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Employee.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Employee request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_Employee.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_Employee.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatusList(string sequenceIds, short status)
        {
            var res = await _s_Employee.UpdateStatusList(sequenceIds, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_Employee.UpdateStatus(id, -1, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTaxableStatus(int employeeId, bool isTaxable)
        {
            var updatedBy = User.GetAccountId();
            var res = await _s_Employee.UpdateTaxableStatus(employeeId, isTaxable, updatedBy);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> BulkUpdateTaxableStatus(string employeeIds, bool isTaxable)
        {
            var updatedBy = User.GetAccountId();
            var res = await _s_Employee.BulkUpdateTaxableStatus(employeeIds, isTaxable, updatedBy);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_Employee.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Employee_FullParam request)
        {
            var res = await _s_Employee.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Employee_FullParam request)
        {
            var res = await _s_Employee.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
