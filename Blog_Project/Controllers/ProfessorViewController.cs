using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Blog_Project.Models;
using System.Linq;
using BCrypt.Net;
using System.Net.Mail;
using System.Net;
using Blog_Project.Services;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Microsoft.Ajax.Utilities;

namespace Blog_Project.Controllers
{
    public class ProfessorViewController : Controller
    {
        AuthenticationService authenticationService = new AuthenticationService();

        UNIVERSITY_MANAGEMENT_SYSTEMEntities dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();
        public ActionResult Dashboard()
        {
            return View();
        }
        [Authorize(Roles = "Professor")]
        public ActionResult Index()
        {
            var userName = User.Identity.Name;
            var userId = dbContext.Users_Table.Where(y => y.CreatedBy == userName).Select(y => y.UserId).FirstOrDefault();

            var dataList = $@"select COURSES.Title as Title,
            COURSES.CREATED_AT as CreatedAt,
            COURSES.Exam_Date as ExamDate,
            COURSES.ID AS Id,
            COURSES.PROFESSOR_ID as ProfessorId,
			COURSES.isEvaluated as Evaluated,
			COURSES.Evaluated_Time as AnswersTime,
			COURSES.Review as Review,
			COURSES.Retake as Retake,
            (select     COUNT(*) AS StudentCount
             from EVALUATION where INVALIDATED = 1
             and COURSES_ID = COURSES.ID) as StudentCount
            from COURSES
            where invalidated = 1 and COURSES.PROFESSOR_ID = {userId}
            ";
            var courses = dbContext.Database.SqlQuery<CoursesModel>(dataList).ToList();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View(courses);
        }

        public bool AddCourseName(string courseName, DateTime? examDate)
        {
            var userName = User.Identity.Name;
            var userId = dbContext.Users_Table.Where(y => y.CreatedBy == userName).Select(y => y.UserId).FirstOrDefault();
            var data = dbContext.COURSES.Where(x => x.Title == courseName && x.PROFESSOR_ID == userId && x.INVALIDATED == true).FirstOrDefault();
            if (data == null)
            {
                var addData = new COURS
                {
                    Title = courseName,
                    CREATED_AT = DateTime.Now,
                    MODIFIED_AT = DateTime.Now,
                    CREATED_BY = userId,
                    MODIFIED_BY = userId,
                    INVALIDATED = true,
                    PROFESSOR_ID = userId,
                    Exam_Date = examDate,
                };
                dbContext.COURSES.Add(addData);
                dbContext.SaveChanges();
            }
            return true;
        }




        public bool EditCourseName(string courseName, int id, DateTime? examDate, int? evaluated, string evaluatedTime, int? retakeSelect, int? reviewSelect)
        {
            var userName = User.Identity.Name;
            var userId = dbContext.Users_Table.Where(y => y.CreatedBy == userName).Select(y => y.UserId).FirstOrDefault();
            var course = dbContext.COURSES.FirstOrDefault(x => x.ID == id && x.PROFESSOR_ID == userId);

            if (course != null)
            {
                if (!string.IsNullOrEmpty(courseName))
                {
                    course.Title = courseName;
                    course.MODIFIED_AT = DateTime.Now;
                    course.MODIFIED_BY = userId;

                    if (evaluated.HasValue) 
                        course.isEvaluated = evaluated.Value;

                    if (!string.IsNullOrEmpty(evaluatedTime)) 
                        course.Evaluated_Time = evaluatedTime;

                    if (retakeSelect.HasValue) 
                        course.Retake = retakeSelect.Value;

                    if (reviewSelect.HasValue) 
                        course.Review = reviewSelect.Value;

                    if (examDate.HasValue)
                        course.Exam_Date = examDate.Value;

                    dbContext.SaveChanges();

                }


            }
            return true;
        }
        public bool DeleteCourseName(int? id)
        {
            var userName = User.Identity.Name;
            var userId = dbContext.Users_Table.Where(y => y.CreatedBy == userName).Select(y => y.UserId).FirstOrDefault();
            var course = dbContext.COURSES.FirstOrDefault(x => x.ID == id && x.PROFESSOR_ID == userId);

            if (course != null)
            {
                course.INVALIDATED = false;

                dbContext.SaveChanges();
                return true;
            }
            return true;

        }
        public ActionResult AddStudents(int course, string courseName)

        {
            var userName = User.Identity.Name;
            var userId = dbContext.Users_Table.Where(y => y.CreatedBy == userName).Select(y => y.UserId).FirstOrDefault();
            var data = dbContext.COURSES.Where(x => x.Title == courseName && x.PROFESSOR_ID == userId && x.INVALIDATED == true).FirstOrDefault();



            var studentsInCourses = $@" select EVALUATION.STUDENT_ID as StudentId, 
            EVALUATION.Students_Name as StudentName, 
            EVALUATION.ABSENT AS Absent,
            EVALUATION.COURSES_ID as COURSES_ID,
			EVALUATION.PROFESSOR_ID as PROFESSOR_ID,        
            COURSES.TITLE AS COURSE_TITLE,
            GRADE as Grade from EVALUATION 
            LEFT JOIN COURSES on EVALUATION.COURSES_ID = COURSES.ID
            where COURSES_ID = {course} and EVALUATION.INVALIDATED = 1 and EVALUATION.PROFESSOR_ID = {userId}";
            var students = dbContext.Database.SqlQuery<StudentViewModel>(studentsInCourses).ToList();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View(students);
        }
        public bool AddNewStudents(int courseId, string studentName)
        {
            var studentId = dbContext.Users_Table.Where(y => y.CreatedBy == studentName).Select(y => y.UserId).FirstOrDefault();
            var userName = User.Identity.Name;
            var professorId = dbContext.Users_Table.Where(y => y.CreatedBy == userName).Select(y => y.UserId).FirstOrDefault();
            if (studentId != null)
            {
                var studentInEvaluetion = dbContext.EVALUATIONs.Where(x => x.STUDENT_ID == studentId && x.COURSES_ID == courseId && x.INVALIDATED == true).FirstOrDefault();
                if (studentInEvaluetion == null)
                {
                    var addData = new EVALUATION
                    {
                        PROFESSOR_ID = professorId,
                        STUDENT_ID = studentId,
                        Students_Name = studentName,
                        COURSES_ID = courseId,
                        CREATED_AT = DateTime.Now,
                        MODIFIED_AT = DateTime.Now,
                        CREATED_BY = professorId,
                        MODIFIED_BY = professorId,
                        INVALIDATED = true,

                    };

                    dbContext.EVALUATIONs.Add(addData);
                    dbContext.SaveChanges();

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EditStudents(int? courseId, decimal? studentGrade, int? studentAbsences, int? studentId)
        {

            var userName = User.Identity.Name;
            var professorId = dbContext.Users_Table.Where(y => y.CreatedBy == userName).Select(y => y.UserId).FirstOrDefault();
            var evaluation = dbContext.EVALUATIONs.FirstOrDefault(x => x.COURSES_ID == courseId && x.STUDENT_ID == studentId && x.INVALIDATED == true);

            if (evaluation != null)
            {
                if (studentAbsences != null) evaluation.ABSENT = studentAbsences;
                if (studentGrade != null) evaluation.GRADE = studentGrade;
                dbContext.SaveChanges();
                return true;
            }
            return true;

        }

        public bool DeleteStudents(int courseId, int studentId)
        {
            var userName = User.Identity.Name;
            var professorId = dbContext.Users_Table.Where(y => y.CreatedBy == userName).Select(y => y.UserId).FirstOrDefault();
            var evaluation = dbContext.EVALUATIONs.FirstOrDefault(x => x.COURSES_ID == courseId && x.STUDENT_ID == studentId && x.INVALIDATED == true);

            if (evaluation != null)
            {
                evaluation.INVALIDATED = false;

                dbContext.SaveChanges();
                return true;
            }
            return true;
        }


    }

}