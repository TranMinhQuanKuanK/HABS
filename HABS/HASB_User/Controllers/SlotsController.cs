using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    [Authorize(Roles = "User")]
    public class SlotsController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public SlotsController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SlotSearchModel searchModel, [FromQuery] PagingRequestModel paging)
        {
            if (searchModel is null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }

            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            //mock data
        }

    }
}
