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
    public class SalaryScaleController : ControllerBase
    {
        private readonly IS_SalaryScale _s_SalaryScale;

        public SalaryScaleController(IS_SalaryScale s_SalaryScale)
        {
            _s_SalaryScale = s_SalaryScale;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_SalaryScale request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_SalaryScale.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_SalaryScale request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_SalaryScale.Update(request);
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_SalaryScale.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_SalaryScale.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_SalaryScale_FullParam request)
        {
            var res = await _s_SalaryScale.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_SalaryScale_FullParam request)
        {
            var res = await _s_SalaryScale.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
