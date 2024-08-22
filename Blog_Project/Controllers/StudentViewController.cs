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
using System.Web.UI.WebControls;

namespace Blog_Project.Controllers
{
    public class StudentViewController : Controller
    {
        AuthenticationService authenticationService = new AuthenticationService();

        UNIVERSITY_MANAGEMENT_SYSTEMEntities dbContext = new UNIVERSITY_MANAGEMENT_SYSTEMEntities();
        [Authorize(Roles = "Student")]
        public ActionResult StudentIndex()
        {
            var userName = User.Identity.Name;
            var userId = dbContext.Users_Table
                                   .Where(y => y.CreatedBy == userName)
                                   .Select(y => y.UserId)
                                   .FirstOrDefault();

            var dataQuery = $@"
        SELECT 
        COURSES.Title AS CourseTitle,
        COURSES.Exam_Date AS ExamDate,
        EVALUATION.GRADE AS Grade,
        EVALUATION.ABSENT AS Absent,
        Users_Table.CreatedBy AS ProfessorName,
        COURSES.isEvaluated AS isEvaluated,
        CASE 
            WHEN COURSES.isEvaluated = 1 THEN COURSES.Title
            ELSE NULL
        END AS isEvaluatedTitle
        FROM COURSES
        LEFT JOIN EVALUATION ON COURSES.ID = EVALUATION.COURSES_ID
        LEFT JOIN Users_Table ON COURSES.PROFESSOR_ID = Users_Table.UserId
        WHERE EVALUATION.STUDENT_ID = {userId}";

            var dataList = dbContext.Database.SqlQuery<StuentsDataViewModel>(dataQuery).ToList();

            var isEvaluatedQuery = $@"
        SELECT 
        STRING_AGG(COURSES.Title, ', ') AS isEvaluatedTitle
        FROM COURSES
        LEFT JOIN EVALUATION ON COURSES.ID = EVALUATION.COURSES_ID
        WHERE EVALUATION.STUDENT_ID = {userId}
        AND COURSES.isEvaluated = 1";

            var isEvaluatedData = dbContext.Database.SqlQuery<StuentsDataViewModel>(isEvaluatedQuery).FirstOrDefault();

            var evaluatedTime = $@"SELECT 
            STRING_AGG(CONCAT(COURSES.title, ' needs ', COURSES.Evaluated_Time, ' to be evaluated'), '; ') AS Evaluation_Time
            FROM EVALUATION 
            JOIN COURSES 
            ON EVALUATION.COURSES_ID = COURSES.ID 
            WHERE 
            STUDENT_ID = {userId} 
            AND COALESCE(COURSES.title, '') <> '' 
            AND COALESCE(COURSES.Evaluated_Time, '') <> ''";

            var evaluatedTimeData = dbContext.Database.SqlQuery<StuentsDataViewModel>(evaluatedTime).FirstOrDefault();

            var evaluationReview = $@"SELECT 
            STRING_AGG(
                CONCAT(
                    CASE WHEN COURSES.Review = 1 THEN 'Yes you can' ELSE 'No you can not' END,
                    ' review course ', 
                    COURSES.Title
                ), 
                '; '
            ) AS evaluationReview
            FROM EVALUATION 
            JOIN COURSES 
            ON EVALUATION.COURSES_ID = COURSES.ID 
            WHERE 
            STUDENT_ID = {userId} 
            AND COALESCE(COURSES.Title, '') <> '' 
            AND COALESCE(COURSES.Evaluated_Time, '') <> ''";

            var evaluatedReviewData = dbContext.Database.SqlQuery<StuentsDataViewModel>(evaluationReview).FirstOrDefault();

            var evaluationRetake = $@"SELECT 
    STRING_AGG(
        CONCAT(
            CASE WHEN COURSES.Retake = 1 THEN 'Yes you can' ELSE 'No you can not' END,
            ' retake course ', 
            COURSES.Title
        ), 
        '; '
    ) AS evaluationRetake
    FROM EVALUATION 
    JOIN COURSES 
    ON EVALUATION.COURSES_ID = COURSES.ID 
    WHERE 
    STUDENT_ID = {userId} 
    AND COALESCE(COURSES.Title, '') <> '' 
    AND COALESCE(COURSES.Evaluated_Time, '') <> ''";

            var evaluatedRetakeData = dbContext.Database.SqlQuery<StuentsDataViewModel>(evaluationRetake).FirstOrDefault();

            var dataListVal = new StudentVM()
            {
                Students = dataList,
                isEvaluatedTitle = isEvaluatedData?.isEvaluatedTitle ,
                Evaluation_Time = evaluatedTimeData?.Evaluation_Time,
                evaluationReview = evaluatedReviewData?.evaluationReview,
                evaluationRetake = evaluatedRetakeData?.evaluationRetake
            };
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return View(dataListVal);
        }

    }

}