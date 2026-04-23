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
    public class TramController : ControllerBase
    {
        private readonly IS_Tram _s_Tram;

        public TramController(IS_Tram s_Tram)
        {
            _s_Tram = s_Tram;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Tram request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Tram.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Tram request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_Tram.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_Tram.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatusList(string sequenceIds, short status)
        {
            var res = await _s_Tram.UpdateStatusList(sequenceIds, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_Tram.UpdateStatus(id, -1, User.GetAccountId());
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_Tram.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Tram_FullParam request)
        {
            var res = await _s_Tram.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Tram_FullParam request)
        {
            var res = await _s_Tram.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
