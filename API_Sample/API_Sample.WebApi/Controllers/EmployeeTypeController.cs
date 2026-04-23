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
    public class EmployeeTypeController : ControllerBase
    {
        private readonly IS_EmployeeType _s_EmployeeType;

        public EmployeeTypeController(IS_EmployeeType s_EmployeeType)
        {
            _s_EmployeeType = s_EmployeeType;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_EmployeeType request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_EmployeeType.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_EmployeeType request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_EmployeeType.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_EmployeeType.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_EmployeeType.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_EmployeeType.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetByCode(string code)
        {
            var res = await _s_EmployeeType.GetByCode(code);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_EmployeeType_FullParam request)
        {
            var res = await _s_EmployeeType.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_EmployeeType_FullParam request)
        {
            var res = await _s_EmployeeType.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
