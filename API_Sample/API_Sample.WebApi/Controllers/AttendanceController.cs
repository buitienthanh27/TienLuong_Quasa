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
    public class AttendanceController : ControllerBase
    {
        private readonly IS_Attendance _s_Attendance;

        public AttendanceController(IS_Attendance s_Attendance)
        {
            _s_Attendance = s_Attendance;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Attendance request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Attendance.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Attendance request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_Attendance.Update(request);
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_Attendance.UpdateStatus(id, -1, User.GetAccountId());
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_Attendance.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Attendance_FullParam request)
        {
            var res = await _s_Attendance.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Attendance_FullParam request)
        {
            var res = await _s_Attendance.GetListByFullParam(request);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> BulkImport(MReq_Attendance_BulkImport request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Attendance.BulkImport(request);
            return Ok(res);
        }
    }
}
