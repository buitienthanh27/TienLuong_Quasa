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
    public class SystemParameterController : ControllerBase
    {
        private readonly IS_SystemParameter _s_SystemParameter;

        public SystemParameterController(IS_SystemParameter s_SystemParameter)
        {
            _s_SystemParameter = s_SystemParameter;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_SystemParameter request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_SystemParameter.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_SystemParameter request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_SystemParameter.Update(request);
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_SystemParameter.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_SystemParameter.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_SystemParameter_FullParam request)
        {
            var res = await _s_SystemParameter.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_SystemParameter_FullParam request)
        {
            var res = await _s_SystemParameter.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
