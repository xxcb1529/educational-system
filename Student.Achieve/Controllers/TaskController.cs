using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Student.Achieve.AuthHelper;
using Student.Achieve.Common.Helper;
using Student.Achieve.Common.HttpContextUser;
using Student.Achieve.IRepository;
using Student.Achieve.Model;
using Student.Achieve.Model.Models;
using Student.Achieve.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Student.Achieve.Controllers.TaskController;

namespace Student.Achieve.Controllers
{
    /// <summary>
    /// 任务管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _iTaskRepository;
        private readonly ITeamRepository _iTeamRepository;
        private readonly ITeamTaskRepository _iTeamTaskRepository;
        private readonly ITeacherRepository _iTeacherRepository;
        private readonly ISysAdminRepository _iSysAdminRepository;

        public TaskController(ISysAdminRepository iSysAdminRepository, ITeacherRepository iTeacherRepository, ITaskRepository iTaskRepository, ITeamRepository iTeamRepository, ITeamTaskRepository iTeamTaskRepository)
        {
            _iTaskRepository = iTaskRepository;
            _iTeamRepository = iTeamRepository;
            _iTeamTaskRepository = iTeamTaskRepository;
            _iTeacherRepository = iTeacherRepository;
            _iSysAdminRepository = iSysAdminRepository;
        }
        /// <summary>
        /// 管理员获取全部（目前teamids无效）
        /// </summary>
        /// <param name="page"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        // GET: api/Task
        [HttpGet]
        public async Task<MessageModel<PageModel<Model.Models.Task>>> Get(string teamids,int page = 1, string key = "")
        {
            try
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
                if (teamids != null && teamids.Length > 0)
                {
                    var ids = teamids.Split(',').Select(id => Convert.ToInt32(id)).ToArray();
                    object[] idObjects = teamids.Cast<object>().ToArray();
                    var tasks = await _iTaskRepository.QueryByIDs(idObjects);
                    var data = tasks.Where(t => t.IsDeleted == false && (t.name != null && t.name.Contains(key))).ToList();
                    var pageModel = new PageModel<Model.Models.Task>
                    {
                        data = data, 
                        page = page,
                        dataCount = data.Count(),
                        pageCount = data.Count(),
                        PageSize = intPageSize
                    };
                    return new MessageModel<PageModel<Model.Models.Task>>()
                    {
                        msg = "获取成功",
                        success = data.Count() > 0,
                        response = pageModel
                    };
                }
                else
                {
                    var data = await _iTaskRepository.QueryPage(a => (a.IsDeleted == false && (a.name != null && a.name.Contains(key))), page, intPageSize, " Id asc ");
                    return new MessageModel<PageModel<Model.Models.Task>>()
                    {
                        msg = "获取成功",
                        success = data.dataCount >= 0,
                        response = data
                    };
                }
            }
            catch (Exception ex)
            {
                return new MessageModel<PageModel<Model.Models.Task>>()
                {
                    msg = $"获取失败: {ex.Message}",
                    success = false,
                    response = null
                };
            }
            
        }

        /// <summary>
        /// 根据参与团队ids获取个人任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        // GET: api/Task
        [HttpGet]
        public async Task<MessageModel<List<Model.Models.Task>>> GetMyTasksByTeamIds(string ids)
        {
            List<Model.Models.Task> myTasks = new ();
            try
            {
                //使用辅助结构HashSet存储已经添加过的任务的id，
                //如果找到已经添加的数据id则不重复添加，复杂度为O（1），但是会消耗内存

                //if (string.IsNullOrEmpty(ids))
                //{
                //    return new MessageModel<List<Model.Models.Task>>()
                //    {
                //        msg = "获取失败，无任务",
                //        success = false,
                //        response = null
                //    };
                //}

                //var teamids = ids.Split(',').Select(id => Convert.ToInt32(id)).ToArray();
                //var myTasks = new List<Model.Models.Task>();
                //var addedTaskIds = new HashSet<int>(); // 用于记录已添加的任务 ID

                //for (int i = 0; i < teamids.Length; i++)
                //{
                //    var tasks = await _iTeamTaskRepository.Query(t => t.team_id == teamids[i] && t.IsDeleted == false);
                //    var taskArr = tasks.ToArray();
                //    var taskids = taskArr.Select(t => t.task_id).ToArray();
                //    var teamTasks = await _iTaskRepository.GetByIdsAsync(taskids);

                //    foreach (var task in teamTasks)
                //    {
                //        // 检查是否已经添加过这个任务，如果没有则添加到 myTasks 中，并将任务 ID 记录在 addedTaskIds 中
                //        if (!addedTaskIds.Contains(task.Id))
                //        {
                //            myTasks.Add(task);
                //            addedTaskIds.Add(task.Id);
                //        }
                //    }
                //}

                //return new MessageModel<List<Model.Models.Task>>()
                //{
                //    msg = "获取成功",
                //    success = myTasks.Count > 0, // 这里应该是大于 0，因为如果任务数量为 0 也表示成功获取
                //    response = myTasks
                //};

                //从最终结果进行筛选，代码量少，LinQ性能较差，需要进行遍历和分组，
                //无法原地修改，需要创建新表，因此会占用额外的内存，并且会导致原始列表的修改和迭代器失效。
                if (string.IsNullOrEmpty(ids))
                {
                    return new MessageModel<List<Model.Models.Task>>()
                    {
                        msg = "获取失败，无任务",
                        success = false,
                        response = null
                    };
                }
                var teamids = ids.Split(',').Select(id => Convert.ToInt32(id)).ToArray();
                object[] idObjects = teamids.Cast<object>().ToArray();
                //var tasks = await _iTaskRepository.QueryByIDs(idObjects);
                //var tasks = await _iTaskRepository.GetByIdsAsync(idArray);
                for (int i = 0; i < teamids.Length; i++)
                {
                    var tasks = await _iTeamTaskRepository.Query(t => t.team_id == teamids[i] && t.IsDeleted == false);
                    var taskArr = tasks.ToArray();
                    var taskids = taskArr.Select(t => t.task_id).ToArray();
                    var mytask = await _iTaskRepository.GetByIdsAsync(taskids);
                    foreach( var task in mytask)
                    {
                        myTasks.Add(task);
                    }
                }
                // 使用 LINQ 移除重复项
                myTasks = myTasks.GroupBy(t => t.Id).Select(g => g.First()).ToList();

                return new MessageModel<List<Model.Models.Task>>()
                {
                    msg = "获取成功",
                    success = myTasks.Count >= 0,
                    response = myTasks
                };
            }
            catch (Exception ex)
            {
                // Log the exception details here
                return new MessageModel<List<Model.Models.Task>>()
                {
                    msg = $"获取失败: {ex.Message}",
                    success = false,
                    response = null
                };
            }
        }

        /// <summary>
        /// 根据团队ids获取全部团队的任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        // GET: api/Task
        [HttpGet]
        public async Task<MessageModel<List<Model.Models.Task>>> GetTasksByTeamIds(string ids)
        {
            List<Model.Models.Task> myTasks = new List<Model.Models.Task>();

            try
            {
                if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
                {
                    var tokenValue = token.ToString().Split(' ').Last();
                    var tokenModel = JwtToken.SerializeJwt(tokenValue);
                    if(tokenModel != null && tokenModel.Role == "Admin_Role")
                    {
                        var allTeams = await _iTeamRepository.Query(t => t.IsDeleted == false);
                        var allTeamIds = allTeams.Select(t=>t.Id).ToArray();
                        for (int i = 0; i < allTeamIds.Length; i++)
                        {
                            var tasks = await _iTeamTaskRepository.Query(t => t.team_id == allTeamIds[i] && t.IsDeleted == false);
                            var taskArr = tasks.ToArray();
                            var taskids = taskArr.Select(t => t.task_id).ToArray();
                            var myTask = new Model.Models.Task
                            {
                                team_name = allTeams[i].name,
                                name = null,
                                description = null,
                                priority = null,
                                assign_team_ids = null,
                                status = 6,
                                start_time = new DateTime(),
                                end_time = new DateTime(),
                                remark = null,
                                create_by = null,
                                CreateTime = new DateTime(),
                                update_time = new DateTime(),
                                update_by = null,
                                children = await _iTaskRepository.GetByIdsAsync(taskids)
                            };
                            myTasks.Add(myTask);
                        }
                        return new MessageModel<List<Model.Models.Task>>()
                        {
                            msg = $"获取成功",
                            success = false,
                            response = myTasks
                        };
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(ids))
                        {
                            return new MessageModel<List<Model.Models.Task>>()
                            {
                                msg = "获取失败，无参与团队",
                                success = false,
                                response = null
                            };
                        }
                        var teamids = ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
                        object[] idObjects = teamids.Cast<object>().ToArray();
                        var teams = await _iTeamRepository.QueryByIDs(idObjects);
                        for (int i = 0; i < teamids.Length; i++)
                        {
                            var tasks = await _iTeamTaskRepository.Query(t => t.team_id == teamids[i] && t.IsDeleted == false);
                            var taskArr = tasks.ToArray();
                            var taskids = taskArr.Select(t => t.task_id).ToArray();
                            var myTask = new Model.Models.Task
                            {
                                team_name = teams[i].name,
                                name = null,
                                description = null,
                                priority = null,
                                assign_team_ids = null,
                                status = 6,
                                start_time = new DateTime(),
                                end_time = new DateTime(),
                                remark = null,
                                create_by = null,
                                CreateTime = new DateTime(),
                                update_time = new DateTime(),
                                update_by = null,
                                children = await _iTaskRepository.GetByIdsAsync(taskids)
                            };
                            myTasks.Add(myTask);
                        }
                        return new MessageModel<List<Model.Models.Task>>()
                        {
                            msg = "获取成功",
                            success = myTasks.Count > 0,
                            response = myTasks
                        };
                    }
                }
                return new MessageModel<List<Model.Models.Task>>()
                {
                    msg = "获取失败，无token",
                    success =false,
                    response = null
                };
            }
            catch (Exception ex)
            {
                return new MessageModel<List<Model.Models.Task>>()
                {
                    msg = $"获取失败: {ex.Message}",
                    success = false,
                    response = null
                };
            }
        }

        /// <summary>
        /// 获取参与团队
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        // GET: api/Task
        [HttpGet]
        public async Task<MessageModel<List<Jteam>>> GetJoinTeams(string ids,int tid)
        {
            try
            {
                if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
                {
                    var tokenValue = token.ToString().Split(' ').Last();
                    var tokenModel = JwtToken.SerializeJwt(tokenValue);
                    if (tokenModel != null && tokenModel.Role == "Admin_Role")
                    {
                        var allTeams = (await _iTeamRepository.Query(t=>t.IsDeleted == false)).Select(t => new Jteam { value = t.Id, label = t.name }).ToList();
                        return new MessageModel<List<Jteam>>()
                        {
                            msg = "获取成功",
                            success = allTeams.Count > 0,
                            response = allTeams
                        };
                    }
                    else
                    {
                        if (ids == null || ids.Length == 0)
                        {
                            return new MessageModel<List<Jteam>>
                            {
                                msg = "获取失败",
                                success = false,
                                response = null
                            };
                        }

                        var teamids = ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
                        object[] idObjects = teamids.Cast<object>().ToArray();
                        var teams = await _iTeamRepository.QueryByIDs(idObjects);
                        var JTeam = teams.Where(t => t.member_ids.Split(',').Select(i => Convert.ToInt32(i)).Contains(tid) && t.head_id == tid).Select(t => new Jteam { value = t.Id, label = t.name }).ToList();
                        return new MessageModel<List<Jteam>>()
                        {
                            msg = "获取成功",
                            success = JTeam.Count > 0,
                            response = JTeam
                        };
                    }
                    
                }
                return new MessageModel<List<Jteam>>()
                {
                    msg = "获取失败，无token",
                    success = false,
                    response = null
                };
            }
            catch (Exception ex) 
            {
                return new MessageModel<List<Jteam>>
                {
                    msg = $"获取失败: {ex.Message}",
                    success = false,
                    response = null
                };
            }
        }

        public class Jteam
        {
            public int value;
            public string label;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// /// <param name="task"></param>
        /// <returns></returns>
        // GET: api/Task
        [HttpPost]
        public async Task<MessageModel<string>> Post([FromBody] Model.Models.Task task)
        {
            var data = new MessageModel<string>();
            string cb;
            try
            {
                if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
                {
                    //获取token的用户信息，判断是管理员还是教师
                    var tokenValue = token.ToString().Split(' ').Last();
                    var tokenModel = JwtToken.SerializeJwt(tokenValue);
                    if(tokenModel != null && tokenModel.Role == "Admin_Role")
                    {
                        cb = (await _iSysAdminRepository.QueryById(tokenModel.Uid)).uRealName;
                    }
                    else
                    {
                        cb = (await _iTeacherRepository.QueryById(tokenModel.Uid)).Name;
                    }
                    task.create_by = cb;
                    task.CreateTime = DateTime.Now;


                    // 获取任务所属的团队ids并进行转换
                    var team_ids = task.assign_team_ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();

                    // 先添加任务获取添加成功后的任务id
                    var tid = await _iTaskRepository.Add(task);

                    //进行关联，将任务加给每一个团队和每一个人
                    foreach (var team_id in team_ids)
                    {
                        //先添加进关联表
                        var tt = new TeamTask()
                        {
                            task_id = tid,
                            team_id = team_id,
                            CreateTime = DateTime.Now,
                        };
                        await _iTeamTaskRepository.Add(tt);

                        //给每一个团队添加任务
                        var team = await _iTeamRepository.QueryById(team_id);
                        var team_taskIds = string.IsNullOrEmpty(team.task_ids) ? "" : team.task_ids;
                        if (!team_taskIds.Split(',').Contains(tid.ToString()))
                        {
                            team_taskIds = string.IsNullOrEmpty(team_taskIds) ? tid.ToString() : $"{team_taskIds},{tid}";
                            team.task_ids = team_taskIds;
                            await _iTeamRepository.Update(team);
                        }

                        // 给团队下的每一个人添加任务
                        var team_members = (await _iTeamRepository.QueryById(team_id)).member_ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
                        foreach (var member_id in team_members)
                        {
                            var member = await _iTeacherRepository.QueryById(member_id);
                            var member_taskIds = string.IsNullOrEmpty(member.task_ids) ? "" : member.task_ids;
                            if (!member_taskIds.Split(',').Contains(tid.ToString()))
                            {
                                member_taskIds = string.IsNullOrEmpty(member_taskIds) ? tid.ToString() : $"{member_taskIds},{tid}";
                                member.task_ids = member_taskIds;
                                await _iTeacherRepository.Update(member);
                            }
                        }
                    }

                    data.success = tid > 0;
                    if (data.success)
                    {
                        data.response = tid.ObjToString();
                        data.msg = "添加成功";
                    }

                    return data;
                }
                data.msg += "无token";
                data.response = null;
                data.success = false;
                return data;
            }
            catch (Exception ex)
            {
                data.msg += ex.Message;
                data.response = null;
                data.success = false;
                return data;
            }
        }


    }
}
