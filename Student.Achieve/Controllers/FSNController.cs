using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student.Achieve.Common.HttpContextUser;
using Student.Achieve.IRepository;
using Student.Achieve.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Achieve.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class FSNController : ControllerBase
    {

        private readonly IExScoreRepository _iExScoreRepository;
        private readonly IStudentsRepository _iStudentsRepository;
        private readonly IExamRepository _iExamRepository;
        private readonly ICourseRepository _iCourseRepository;
        private readonly IClassRepository _iClazzRepository;
        private readonly IGradeRepository _iGradeRepository;
        private readonly ICCTRepository _iCCTRepository;
        private readonly ITeacherRepository _iTeacherRepository;
        private readonly IUser _iUser;
        private int GID = 0;

        public FSNController(IExScoreRepository iExScoreRepository, IStudentsRepository iStudentsRepository, IExamRepository iExamRepository, ICourseRepository iCourseRepository, IClassRepository iClazzRepository, IGradeRepository iGradeRepository, IUser iUser, ICCTRepository iCCTRepository, ITeacherRepository iTeacherRepository)
        {
            this._iExScoreRepository = iExScoreRepository;
            this._iStudentsRepository = iStudentsRepository;
            this._iExamRepository = iExamRepository;
            this._iCourseRepository = iCourseRepository;
            this._iClazzRepository = iClazzRepository;
            this._iGradeRepository = iGradeRepository;
            this._iCCTRepository = iCCTRepository;
            this._iTeacherRepository = iTeacherRepository;
            GID = (iUser.GetClaimValueByType("GID").FirstOrDefault()).ObjToInt();
        }


        [HttpGet]
        public async Task<MessageModel<PageModel<FSN>>> Get(int GradeId, string AcademicYearSchoolTerm, string ExamName, int fsn, int ClazzId, int page = 1)
        {
            int intPageSize = 100;
            if (!(GradeId > 0 && !string.IsNullOrEmpty(AcademicYearSchoolTerm) && !string.IsNullOrEmpty(ExamName)))
            {
                return new MessageModel<PageModel<FSN>>();
            }

            var exScoreList = await _iExScoreRepository.Query(d => d.IsDeleted == false);

            var gradeList = await _iGradeRepository.Query(d => d.IsDeleted == false && d.Id == GradeId);
            var clazzList = await _iClazzRepository.Query(d => d.IsDeleted == false && d.GradeId == GradeId);
            var courseList = await _iCourseRepository.Query(d => d.IsDeleted == false);
            var examList = await _iExamRepository.Query(d => d.IsDeleted == false && d.gradeid == GradeId);
            var studentsList = await _iStudentsRepository.Query(d => d.IsDeleted == false && d.gradeid == GradeId);
            var cctList = await _iCCTRepository.Query(d => d.IsDeleted == false && d.gradeid == GradeId);
            var teachersList = await _iTeacherRepository.Query(d => d.IsDeleted == false && d.gradeId == GradeId);

            foreach (var item in examList)
            {
                item.grade = gradeList.Where(d => d.Id == item.gradeid).FirstOrDefault();
                item.course = courseList.Where(d => d.Id == item.courseid).FirstOrDefault();
            }

            foreach (var exscore in exScoreList)
            {
                exscore.exam = examList.Where(d => d.Id == exscore.examid).FirstOrDefault();
                var teacherid = cctList.Where(d => d.classid == exscore.classid && d.gradeid == exscore.exam.gradeid && d.courseid == exscore.exam.courseid).FirstOrDefault()?.teacherid;
                exscore.Teacher = teachersList.Where(d => d.Id == teacherid.ObjToInt()).FirstOrDefault()?.Name;


                exscore.clazz = clazzList.Where(d => d.Id == exscore.classid).FirstOrDefault();
                exscore.student = studentsList.Where(d => d.Id == exscore.studentid).FirstOrDefault();
            }


            exScoreList = exScoreList.Where(d => (d.exam != null && d.exam.grade != null && d.exam.grade.Id == GID || (GID == -9999 && true))).ToList();


            if (GradeId > 0)
            {
                exScoreList = exScoreList.Where(d => d.exam != null && d.exam.grade != null && d.exam.gradeid == GradeId).ToList();
            }
            if (!string.IsNullOrEmpty(AcademicYearSchoolTerm))
            {
                exScoreList = exScoreList.Where(d => d.exam != null && d.exam.grade != null && AcademicYearSchoolTerm == (d.exam.AcademicYear + d.exam.SchoolTerm)).ToList();
            }
            if (!string.IsNullOrEmpty(ExamName))
            {
                exScoreList = exScoreList.Where(d => d.exam != null && d.exam.grade != null && d.exam.ExamName == ExamName).ToList();
            }



            List<FSN> FSNs = new List<FSN>();

            foreach (var item in studentsList)
            {
                var clazzModel = clazzList.Where(d => d.Id == item.classid).FirstOrDefault();
                var exscoreStudentList = exScoreList.Where(d => d.studentid == item.Id).ToList();

                FSN fSN = new FSN()
                {
                    StudentNo = item.StudentNo,
                    StudentName = item.Name,
                    Clazz = clazzModel.ClassNo,
                    ClazzId = clazzModel.Id,
                    ProDirection = item.ProDirection,
                    CGPA = item.CGPA,
                    Chinese = exscoreStudentList.Where(d => d.exam.course.Name == "语文").FirstOrDefault().score,
                    Meth = exscoreStudentList.Where(d => d.exam.course.Name == "数学").FirstOrDefault().score,
                    English = exscoreStudentList.Where(d => d.exam.course.Name == "英语").FirstOrDefault().score,
                    Physics = exscoreStudentList.Where(d => d.exam.course.Name == "物理").FirstOrDefault().score,
                    Chemistry = exscoreStudentList.Where(d => d.exam.course.Name == "化学").FirstOrDefault().score,
                    Politics = exscoreStudentList.Where(d => d.exam.course.Name == "政治").FirstOrDefault().score,
                    History = exscoreStudentList.Where(d => d.exam.course.Name == "历史").FirstOrDefault().score,
                    Biology = exscoreStudentList.Where(d => d.exam.course.Name == "生物").FirstOrDefault().score,
                    Geography = exscoreStudentList.Where(d => d.exam.course.Name == "地理").FirstOrDefault().score,
                };

                fSN.T = fSN.Chinese + fSN.Chinese + fSN.Chinese;

                fSN.N = fSN.Chinese + fSN.Meth + fSN.English + fSN.Physics + fSN.Chemistry + fSN.Biology + fSN.Politics + fSN.History + fSN.Geography;

                FSNs.Add(fSN);
            }




            {
                FSNs = FSNs.OrderByDescending(d => d.Chinese).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].ChineseSort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.Meth).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].MethSort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.English).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].EnglishSort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.Physics).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].PhysicsSort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.Chemistry).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].ChemistrySort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.Politics).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].PoliticsSort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.History).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].HistorySort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.Biology).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].BiologySort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.Geography).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].GeographySort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.T).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].TSort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.F).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].FSort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.S).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].SSort = i + 1;

                FSNs = FSNs.OrderByDescending(d => d.N).ToList();
                for (int i = 0; i < FSNs.Count; i++) FSNs[i].NSort = i + 1;

            }


            if (ClazzId > 0)
            {
                FSNs = FSNs.Where(d => d.ClazzId == ClazzId).ToList();
            }



            var totalCount = FSNs.Count;
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();


            var exScores = FSNs.Skip((page - 1) * intPageSize).Take(intPageSize).ToList();



            PageModel<FSN> data = new PageModel<FSN>()
            {
                data = exScores,
                dataCount = totalCount,
                page = page,
                pageCount = pageCount,
                PageSize = intPageSize
            };


            return new MessageModel<PageModel<FSN>>()
            {
                msg = "获取成功",
                success = data.dataCount >= 0,
                response = data
            };

        }

    }

    public class FSN
    {
        public string GradeId { get; set; }
        public string AcademicYearSchoolTerm { get; set; }

        public string ExamName { get; set; }
        public int courseid { get; set; } //考试科目ID



        public string StudentNo { get; set; }
        public string Clazz { get; set; }
        public int ClazzId { get; set; }
        public string StudentName { get; set; }
        public string ProDirection { get; set; }
        public double CGPA { get; set; }

        public int Chinese { get; set; }
        public int ChineseSort { get; set; }
        public int Meth { get; set; }
        public int MethSort { get; set; }
        public int English { get; set; }
        public int EnglishSort { get; set; }
        public int Physics { get; set; }
        public int PhysicsSort { get; set; }
        public int Chemistry { get; set; }
        public int ChemistrySort { get; set; }
        public int Politics { get; set; }
        public int PoliticsSort { get; set; }
        public int History { get; set; }
        public int HistorySort { get; set; }
        public int Biology { get; set; }
        public int BiologySort { get; set; }
        public int Geography { get; set; }
        public int GeographySort { get; set; }
        public int T { get; set; }
        public int TSort { get; set; }
        public int F { get; set; }
        public int FSort { get; set; }
        public int S { get; set; }
        public int SSort { get; set; }
        public int N { get; set; }
        public int NSort { get; set; }


    }
}