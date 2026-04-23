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
    public class WorkTypeController : ControllerBase
    {
        private readonly IS_WorkType _s_WorkType;

        public WorkTypeController(IS_WorkType s_WorkType)
        {
            _s_WorkType = s_WorkType;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_WorkType request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_WorkType.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_WorkType request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_WorkType.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_WorkType.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_WorkType.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_WorkType.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetByCode(string code)
        {
            var res = await _s_WorkType.GetByCode(code);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_WorkType_FullParam request)
        {
            var res = await _s_WorkType.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_WorkType_FullParam request)
        {
            var res = await _s_WorkType.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
