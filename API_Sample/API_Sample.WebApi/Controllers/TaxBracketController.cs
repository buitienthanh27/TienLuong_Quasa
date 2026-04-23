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
    public class TaxBracketController : ControllerBase
    {
        private readonly IS_TaxBracket _s_TaxBracket;

        public TaxBracketController(IS_TaxBracket s_TaxBracket)
        {
            _s_TaxBracket = s_TaxBracket;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_TaxBracket request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_TaxBracket.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_TaxBracket request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_TaxBracket.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var updatedBy = User.GetAccountId();
            var res = await _s_TaxBracket.UpdateStatus(id, status, updatedBy);
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_TaxBracket.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_TaxBracket.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_TaxBracket_FullParam request)
        {
            var res = await _s_TaxBracket.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_TaxBracket_FullParam request)
        {
            var res = await _s_TaxBracket.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
