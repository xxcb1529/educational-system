using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Student.Achieve.AuthHelper;
using Student.Achieve.Common.Helper;
using Student.Achieve.Model;
using Student.Achieve.Model.Models;
using Student.Achieve.Common.HttpContextUser;
using Student.Achieve.IRepository;
using Student.Achieve.Common.LogHelper;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using SqlSugar;
namespace Student.Achieve.Controllers
{
    /// <summary>
    /// 首页数据
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class HomeDataController : ControllerBase
    {
        readonly ISysAdminRepository _SysAdminRepository;
        readonly IUserRoleRepository _userRoleRepository;
        readonly IRoleRepository _roleRepository;
        private readonly IUser _user;
        private readonly IModuleRepository moduleRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly IRoleModulePermissionRepository roleModulePermissionRepository;
        private readonly ITeacherRepository _iTeacherRepository;
        private readonly IUser _iUser;
        private readonly ICCTRepository _cCTRepository;
        private readonly ICourseRepository courseRepository;
        private readonly IClassRepository _classRepository;
        readonly IHomeDataRepository _homeDataRepository;
        public HomeDataController(IHomeDataRepository homeDataRepository, ISysAdminRepository SysAdminRepository, IUserRoleRepository userRoleRepository, IRoleRepository roleRepository, IUser user, IModuleRepository moduleRepository, IPermissionRepository permissionRepository, IRoleModulePermissionRepository roleModulePermissionRepository, IUser iUser, ITeacherRepository iTeacherRepository, IClassRepository iClassRepository)
        {
            _SysAdminRepository = SysAdminRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _user = user;
            this.moduleRepository = moduleRepository;
            this.permissionRepository = permissionRepository;
            this.roleModulePermissionRepository = roleModulePermissionRepository;
            this._iTeacherRepository = iTeacherRepository;
            this._iUser = iUser;
            _classRepository = iClassRepository;
            _homeDataRepository = homeDataRepository;
        }
        [HttpGet]
        public IActionResult GetSystemInfo()
        {
            // 获取操作系统信息
            string osName = RuntimeInformation.OSDescription;
            string osArchitecture = RuntimeInformation.OSArchitecture.ToString();
            string ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();


            // 框架信息
            string backendFramework = "Dotnet6";
            string frontEndFramework = "VUE2";
            string database = "MySQL";
            string UIFramework = "element-ui";

            // 封装系统信息为一个对象
            var systemInfo = new
            {
                OSName = osName,
                OSArchitecture = osArchitecture,
                IPAddress = ipAddress,
                BackendFramework = backendFramework,
                FrontEndFramework = frontEndFramework,
                Database = database,
                UIFramework,
            };

            // 返回系统信息
            return Ok(systemInfo);
        }
        [HttpGet]
        public IActionResult GetHomeData()
        {
            var allClasses = _homeDataRepository.GetAllClasses();
            var classesAddedWithinLastYear = _homeDataRepository.GetClassesAddedWithinLastYear();

            var totalClassCount = allClasses.Count();
            var classCountAddedWithinLastYear = classesAddedWithinLastYear.Count();

            return Ok(new
            {
                TotalClassCount = totalClassCount,
                ClassCountAddedWithinLastYear = classCountAddedWithinLastYear
            });

        }
    }
}
