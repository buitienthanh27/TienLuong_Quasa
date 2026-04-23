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
    public class HolidayController : ControllerBase
    {
        private readonly IS_Holiday _s_Holiday;

        public HolidayController(IS_Holiday s_Holiday)
        {
            _s_Holiday = s_Holiday;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Holiday request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Holiday.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Holiday request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_Holiday.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_Holiday.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_Holiday.Delete(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_Holiday.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetHolidaysByYear(int year)
        {
            var res = await _s_Holiday.GetHolidaysByYear(year);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> CheckHoliday(DateTime date)
        {
            var res = await _s_Holiday.CheckHoliday(date);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Holiday_FullParam request)
        {
            var res = await _s_Holiday.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Holiday_FullParam request)
        {
            var res = await _s_Holiday.GetListByFullParam(request);
            return Ok(res);
        }
    }
}
