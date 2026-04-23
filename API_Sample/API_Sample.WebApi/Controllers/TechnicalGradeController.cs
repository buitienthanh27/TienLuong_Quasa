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
    public class TechnicalGradeController : ControllerBase
    {
        private readonly IS_TechnicalGrade _s_TechnicalGrade;

        public TechnicalGradeController(IS_TechnicalGrade s_TechnicalGrade)
        {
            _s_TechnicalGrade = s_TechnicalGrade;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_TechnicalGrade request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_TechnicalGrade.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_TechnicalGrade request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_TechnicalGrade.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_TechnicalGrade.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_TechnicalGrade.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_TechnicalGrade.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetByGrade(string grade)
        {
            var res = await _s_TechnicalGrade.GetByGrade(grade);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_TechnicalGrade_FullParam request)
        {
            var res = await _s_TechnicalGrade.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_TechnicalGrade_FullParam request)
        {
            var res = await _s_TechnicalGrade.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
