using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student.Achieve;
using Student.Achieve.Common.HttpContextUser;
using Student.Achieve.IRepository;
using Student.Achieve.Model;
using Student.Achieve.Model.Models;
using Student.Achieve.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Student.Achieve.Controllers
{
    /// <summary>
    /// 父级接口管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class MParentController : ControllerBase
    {
        private readonly IMParentRepository _iMParentRepository;
        public MParentController(IMParentRepository iMParentRepository)
        {
            this._iMParentRepository = iMParentRepository;
        }
        public class PageModel<T>
        {
            public List<T> Data { get; set; }
            // 其他分页相关属性，如 TotalCount, PageSize, PageNumber 等
        }

        /// <summary>
        /// 获取全部父级接口
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        // GET: api/Grade
        [HttpGet]
        public async Task<MessageModel<PageModel<MParent>>> Get(string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }

            var data = await _iMParentRepository.Query(a => (a.IsDeleted == false && (a.IName != null && a.IName.Contains(key))), " IName desc ");

            var pageModel = new PageModel<MParent>
            {
                Data = data
            };

            return new MessageModel<PageModel<MParent>>()
            {
                msg = "获取成功",
                success = data.ToArray().Length >= 0,
                response = pageModel
            };

        }
    }
}

