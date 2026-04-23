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
    public class PositionController : ControllerBase
    {
        private readonly IS_Position _s_Position;

        public PositionController(IS_Position s_Position)
        {
            _s_Position = s_Position;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Position request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Position.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Position request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_Position.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var updatedBy = User.GetAccountId();
            var res = await _s_Position.UpdateStatus(id, status, updatedBy);
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_Position.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_Position.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Position_FullParam request)
        {
            var res = await _s_Position.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Position_FullParam request)
        {
            var res = await _s_Position.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
