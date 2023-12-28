using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Student.Achieve.IRepository;
using Student.Achieve.Model;
using Student.Achieve.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Student.Achieve.Controllers
{
    /// <summary>
    /// 接口管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class ModuleController : ControllerBase
    {
        readonly IModuleRepository _moduleRepository;
        readonly IMParentRepository _mparentRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleRepository"></param>
        /// <param name="mParentRepository"></param>
        public ModuleController(IModuleRepository moduleRepository, IMParentRepository mParentRepository)
        {
            _moduleRepository = moduleRepository;
            _mparentRepository = mParentRepository;
        }

        /// <summary>
        /// 获取全部接口api
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        // GET: api/User
        [HttpGet]
        public async Task<MessageModel<PageModel<Module>>> Get(int page = 1, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
                page = 1;
            }
            int intPageSize = 50;

            Expression<Func<Module, bool>> whereExpression = a => a.IsDeleted != true && (a.Name != null && a.Name.Contains(key));

            var data = await _moduleRepository.QueryPage(whereExpression, page, intPageSize, " Id desc ");

            return new MessageModel<PageModel<Module>>()
            {
                msg = "获取成功",
                success = data.dataCount >= 0,
                response = data
            };
        }
        /// <summary>
        /// 获取表格树api
        /// </summary>
        /// <returns></returns>
        // GET: api/GetAllParents
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<ParentDto>>> GetAllParents()
        {
            var parents = await _mparentRepository.Query(d => d.IsDeleted == false); 

            var result = parents.Select(p => new ParentDto
            {
                Id = p.Id,
                IName = p.IName,
                LinkUrl = p.LinkUrl,
                Description = p.Description,
                createTime = p.CreateTime.ObjToString("yyyy-MM-dd HH:mm:ss"),
                createUser = p.CreateBy,
                Enabled = p.Enabled,
                hasChildren = true,
                children = GetChildrenForParent(p.Id).Result
            }).ToList();

            return new MessageModel<List<ParentDto>>()
            {
                msg = "获取成功",
                success = result != null && result.Any(),
                response = result
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<List<Module>> GetChildrenForParent(int parentId)
        {
            Expression<Func<Module, bool>> whereExpression = m => m.IsDeleted != true && m.ParentId == parentId;
            var modules = await _moduleRepository.Query(whereExpression);

            return modules;
        }
        // GET: api/User/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }

        /// <summary>
        /// 添加一条接口信息
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        // POST: api/User
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] Module module)
        {
            var data = new MessageModel<string>();

            var id = (await _moduleRepository.Add(module));
            data.success = id > 0;
            if (data.success)
            {
                data.response = id.ObjToString();
                data.msg = "添加成功";
            }

            return data;
        }

        /// <summary>
        /// 更新接口信息
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        // PUT: api/User/5
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] Module module)
        {
            var data = new MessageModel<string>();
            if (module != null && module.Id > 0)
            {
                data.success = await _moduleRepository.Update(module);
                if (data.success)
                {
                    data.msg = "更新成功";
                    data.response = module?.Id.ObjToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除一条接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        public async Task<MessageModel<string>> Delete(int id)
        {
            var data = new MessageModel<string>();
            if (id > 0)
            {
                var userDetail = await _moduleRepository.QueryById(id);
                userDetail.IsDeleted = true;
                data.success = await _moduleRepository.Update(userDetail);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = userDetail?.Id.ObjToString();
                }
            }

            return data;
        }
    }
    public class ParentDto
    {
        public int Id { get; set; }
        public string IName { get; set; }
        public bool hasChildren { get; set; }
        public string LinkUrl { get; set; }
        public string Description { get; set; }
        public string createTime { get; set; }
        public string createUser { get; set; }
        public int Enabled { get; set; }
        public List<Module> children { get; set; }
    };
}

