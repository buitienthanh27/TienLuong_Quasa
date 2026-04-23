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
    public class ProductionController : ControllerBase
    {
        private readonly IS_Production _s_Production;

        public ProductionController(IS_Production s_Production)
        {
            _s_Production = s_Production;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Production request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Production.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Production request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_Production.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_Production.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_Production.UpdateStatus(id, -1, User.GetAccountId());
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_Production.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Production_FullParam request)
        {
            var res = await _s_Production.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Production_FullParam request)
        {
            var res = await _s_Production.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
