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
    public class RubberUnitPriceController : ControllerBase
    {
        private readonly IS_RubberUnitPrice _s_RubberUnitPrice;

        public RubberUnitPriceController(IS_RubberUnitPrice s_RubberUnitPrice)
        {
            _s_RubberUnitPrice = s_RubberUnitPrice;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_RubberUnitPrice request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_RubberUnitPrice.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_RubberUnitPrice request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_RubberUnitPrice.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_RubberUnitPrice.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_RubberUnitPrice.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_RubberUnitPrice.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentPrice(int tramId, string grade, DateTime? effectiveDate = null)
        {
            var res = await _s_RubberUnitPrice.GetCurrentPrice(tramId, grade, effectiveDate);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_RubberUnitPrice_FullParam request)
        {
            var res = await _s_RubberUnitPrice.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_RubberUnitPrice_FullParam request)
        {
            var res = await _s_RubberUnitPrice.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
