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
    public class TechnicalEvaluationController : ControllerBase
    {
        private readonly IS_TechnicalEvaluation _s_TechnicalEvaluation;

        public TechnicalEvaluationController(IS_TechnicalEvaluation s_TechnicalEvaluation)
        {
            _s_TechnicalEvaluation = s_TechnicalEvaluation;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_TechnicalEvaluation request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_TechnicalEvaluation.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_TechnicalEvaluation request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_TechnicalEvaluation.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Review(int id, string reviewedGrade, decimal? reviewedScore, string? note)
        {
            var res = await _s_TechnicalEvaluation.Review(id, reviewedGrade, reviewedScore, User.GetAccountId(), note);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_TechnicalEvaluation.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_TechnicalEvaluation.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_TechnicalEvaluation.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetByEmployeeMonth(int employeeId, string yearMonth)
        {
            var res = await _s_TechnicalEvaluation.GetByEmployeeMonth(employeeId, yearMonth);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_TechnicalEvaluation_FullParam request)
        {
            var res = await _s_TechnicalEvaluation.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_TechnicalEvaluation_FullParam request)
        {
            var res = await _s_TechnicalEvaluation.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
