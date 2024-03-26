using System.Threading.Tasks;
using Student.Achieve.IRepository;
using Student.Achieve.Model;
using Student.Achieve.Model.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Student.Achieve.Common.Helper;
using Student.Achieve.Common.HttpContextUser;
using Microsoft.AspNetCore.Authorization;
using System;
using static Student.Achieve.Controllers.TaskController;

namespace Student.Achieve.Controllers
{
    /// <summary>
    /// 教师管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherRepository _iTeacherRepository;
        private readonly ICCTRepository _iCCTRepository;
        private readonly IGradeRepository _iGradeRepository;
        private readonly IClassRepository _iClazzRepository;
        private readonly ICourseRepository _iCourseRepository;
        private readonly IUser _iUser;
        private readonly IUserRoleRepository _iUserRoleRepository;
        private int GID = 0;

        public TeacherController(IUserRoleRepository iUserRoleRepository, ITeacherRepository iTeacherRepository, ICCTRepository iCCTRepository, IGradeRepository iGradeRepository, IClassRepository iClazzRepository, ICourseRepository iCourseRepository, IUser iUser)
        {
            this._iTeacherRepository = iTeacherRepository;
            this._iCCTRepository = iCCTRepository;
            this._iGradeRepository = iGradeRepository;
            this._iClazzRepository = iClazzRepository;
            this._iCourseRepository = iCourseRepository;
            _iUserRoleRepository = iUserRoleRepository;
            GID = (iUser.GetClaimValueByType("GID").FirstOrDefault()).ObjToInt();
        }

        /// <summary>
        /// 获取全部教师
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        // GET: api/Teacher
        [HttpGet]
        public async Task<MessageModel<PageModel<Teacher>>> Get(int page = 1, string key = "")
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                key = "";
            }
            else
            {
                page = 1;
            }
            int intPageSize = 10;


            var data = await _iTeacherRepository.QueryPage(a => (a.IsDeleted == false && (a.Name != null && a.Name.Contains(key)))&& (a.gradeId == GID || (GID == -9999 && true)), page, intPageSize, " Id asc ");

            var cctList = await _iCCTRepository.Query(d => d.IsDeleted == false);
            var gradeList = await _iGradeRepository.Query(d => d.IsDeleted == false);
            var classList = await _iClazzRepository.Query(d => d.IsDeleted == false);
            var coureseList = await _iCourseRepository.Query(d => d.IsDeleted == false);
            foreach (var item in cctList)
            {
                item.grade = gradeList.Where(d => d.Id == item.gradeid).FirstOrDefault();
                item.clazz = classList.Where(d => d.Id == item.classid).FirstOrDefault();
                item.course = coureseList.Where(d => d.Id == item.courseid).FirstOrDefault();
            }
            var ur = await _iUserRoleRepository.Query(d => d.IsDeleted == false);
            foreach (var item in data.data)
            {
                item.cct = cctList.Where(d => d.teacherid == item.Id).ToList();
                var roleId = (from rt in ur
                              where rt.TId == item.Id // 假设 item.Id 是教师的 ID
                              select rt.RoleId).FirstOrDefault();
                item.role_id = roleId;
                if (item.courseIds == null)
                {
                    item.courseIds = string.Empty;
                }
                else
                {
                    item.courseIds.Split(",").ToList();
                }
                if (item.Class_ids == null)
                {
                    item.Class_ids = string.Empty;
                }
                else
                {
                    item.Class_ids.Split(",").ToList();
                }
            }

            return new MessageModel<PageModel<Teacher>>()
            {
                msg = "获取成功",
                success = data.dataCount >= 0,
                response = data
            };

        }

        // GET: api/Teacher/5
        [HttpGet("{id}")]
        public async Task<MessageModel<Teacher>> Get(string id)
        {
            var data = await _iTeacherRepository.QueryById(id);

            return new MessageModel<Teacher>()
            {
                msg = "获取成功",
                success = data != null,
                response = data
            };
        }


        /// <summary>
        /// 添加一个教师
        /// </summary>
        /// <param name="Teacher"></param>
        /// <returns></returns>
        // POST: api/Teacher
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] Teacher Teacher)
        {
            try
            {
                var data = new MessageModel<string>();
                Teacher.Password = MD5Helper.MD5Encrypt32(Teacher.TeacherNo);
                var id = await _iTeacherRepository.Add(Teacher);

                data.success = id > 0;
                if (data.success)
                {
                    List<CCT> cCTs = (from item in Teacher.Class_ids.Split(',').Select(int.Parse).ToArray()
                                      select new CCT
                                      {
                                          IsDeleted = false,
                                          classid = item,
                                          courseid = 0,
                                          teacherid = id,
                                          gradeid = Teacher.gradeId,
                                      }).ToList();
                    var role = new UserRole() { 
                        TId = id,
                        RoleId = Teacher.role_id,
                        UserId = 0,
                    };
                    var newDataSave = await _iCCTRepository.Add(cCTs);
                    var newDataSave2 = await _iUserRoleRepository.Add(role);
                    data.response = id.ObjToString();
                    data.msg = "添加成功";
                }

                return data;
            }
            catch (Exception ex)
            {
                return new MessageModel<string>
                {
                    msg = $"添加失败: {ex.Message}"
                };
            }
            
        }

        /// <summary>
        /// 更新教师
        /// </summary>
        /// <param name="Teacher"></param>
        /// <returns></returns>
        // PUT: api/Teacher/5
        [HttpPut]
        public async Task<MessageModel<string>> Put([FromBody] Teacher Teacher)
        {
            var data = new MessageModel<string>();
            if (Teacher != null && Teacher.Id > 0)
            {

                data.success = await _iTeacherRepository.Update(Teacher);
                if (data.success)
                {
                    var cctCureent = await _iCCTRepository.Query(d => d.teacherid == Teacher.Id);
                    var deleteSave = await _iCCTRepository.DeleteByIds(cctCureent.Select(d => d.Id.ToString()).ToArray());
                    var URId = (await _iUserRoleRepository.Query(ur => ur.TId == Teacher.Id)).First().Id;
                    if (Teacher.Class_ids.ToArray().Length>0)
                    {
                        List<CCT> cCTs = (from item in Teacher.Class_ids.Split(',').Select(int.Parse).ToArray()
                                          select new CCT
                                          {
                                              IsDeleted = false,
                                              classid = item,
                                              courseid = 0,
                                              teacherid = Teacher.Id,
                                              gradeid = Teacher.gradeId,
                                          }).ToList();
                        var role = new UserRole()
                        {
                            Id = URId,
                            TId = Teacher.Id,
                            RoleId = Teacher.role_id,
                            UserId = 0,
                        };

                        var newDataSave = await _iCCTRepository.Add(cCTs);
                        var newDataSave2 = await _iUserRoleRepository.Update(role);
                    }

                    data.msg = "更新成功";
                    data.response = Teacher?.Id.ObjToString();
                }
            }

            return data;
        }

        /// <summary>
        /// 删除教师
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
                var model = await _iTeacherRepository.QueryById(id);
                model.IsDeleted = true;
                data.success = await _iTeacherRepository.Update(model);
                if (data.success)
                {
                    data.msg = "删除成功";
                    data.response = model?.Id.ObjToString();
                }
            }

            return data;
        }



        // GET: api/Teacher/GetTeacherTree
        [HttpGet]
        [AllowAnonymous]
        public async Task<MessageModel<List<TreeModel>>> GetTeacherTree()
        {
            var gradeList = await _iTeacherRepository.Query(d => d.IsDeleted == false);
            var data = gradeList.Select(d => new TreeModel { value = d.Id, label = d.Name }).ToList();

            return new MessageModel<List<TreeModel>>()
            {
                msg = "获取成功",
                success = data != null,
                response = data
            };
        }
    }
}
