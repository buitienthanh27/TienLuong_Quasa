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
    public class ProductController : ControllerBase
    {
        private readonly IS_Product _s_Product;

        public ProductController(IS_Product s_Product)
        {
            _s_Product = s_Product;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Product request)
        {
            request.CreatedBy = User.GetAccountId();
            var res = await _s_Product.Create(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Product request)
        {
            request.UpdatedBy = User.GetAccountId();
            var res = await _s_Product.Update(request);
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status)
        {
            var res = await _s_Product.UpdateStatus(id, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatusList(string sequenceIds, short status)
        {
            var res = await _s_Product.UpdateStatusList(sequenceIds, status, User.GetAccountId());
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _s_Product.UpdateStatus(id, -1, User.GetAccountId());
            return Ok(res);
        }

        //[HttpDelete]
        //public async Task<IActionResult> DeleteHard(int id)
        //{
        //    var res = await _s_Product.Delete(id);
        //    return Ok(res);
        //}

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _s_Product.GetById(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Product_FullParam request)
        {
            var res = await _s_Product.GetListByPaging(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Product_FullParam request)
        {
            var res = await _s_Product.GetListByFullParam(request);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetListBySpFullParam([FromQuery] MReq_Product_FullParam request)
        {
            var res = await _s_Product.GetListBySpFullParam(request);
            return Ok(res);
        }

    }
}
