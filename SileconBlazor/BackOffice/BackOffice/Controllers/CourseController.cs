using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Security.Principal;
using System.Xml.Linq;

namespace BackOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration config;

        private List<int> bestSellers { get; set; }
        public CourseController(ApplicationDbContext context,UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env, IConfiguration config)
        {
            this.context = context;
            this.userManager = userManager;
            this.env = env;
            this.config = config;
            bestSellers = new();
        }
        [HttpDelete("DeleteCourse/{CourseId}")]
        public async Task<ActionResult> DeleteCourseAsync(int CourseId)
        {
            Course? course =await context.Courses.FindAsync(CourseId);
              

             
            if (course == null) { return BadRequest(); }


            var cs = context.CourseLearns.Where(l => l.CourseId == CourseId).ToList();
            if (cs.Count != 0)
            {
                context.CourseLearns.RemoveRange(cs);
            }

            var cl= context.CourseLearns.Where(l => l.CourseId==CourseId).ToList();
            if (cl.Count != 0)
            {
                    context.CourseLearns.RemoveRange(cl);
            }
        


            var cd = context.CourseDetails.Where(l => l.CourseId == CourseId).ToList();
            if (cd.Count != 0)
            {
                context.CourseDetails.RemoveRange(cd);
            }

            var ct = context.TeacherCourses.Where(l => l.CourseId == CourseId).ToList();
            if (ct.Count != 0)
            {
                context.TeacherCourses.RemoveRange(ct);
            }
            await context.SaveChangesAsync();

            context.Courses.Remove(course);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("get-All/{UserEmail}")]
        public async Task<ActionResult<List<Course>>> GetCoursesAsync(string UserEmail)
        {
            List<Course> courses = [];
            courses=await context.Courses
                        .Include(i=>i.Category)
                        .Include(i=>i.ApplicationUser)
                        .Include(i=>i.CourseLearns)
                        .Include(i=>i.CourseDetails)
                        .Include(i=>i.TeacherCourses)
                        .ToListAsync();

           // if (User.IsInRole("SuberAdmin"))
           //     return Ok(courses);

           //var user=await userManager.FindByEmailAsync(UserEmail);
           // if (user == null)
           //     return Ok(new List<Course>());

           // courses = courses.Where(i => i.ApplicationUserId == user.Id).ToList();
            return Ok(courses);
        }


        [HttpGet("get-Course/{CourseId}")]
        public async Task<ActionResult<CourseDto>> GetCourseAsync(int CourseId)
        {
            Course course = new();

            course = await context.Courses
              
                        .Include(i => i.Category)
                        .Include(i => i.ApplicationUser)
                        .Include(i => i.CourseLearns)
                        .Include(i => i.CourseDetails)
                        .Include(i => i.TeacherCourses)
                        .Where(i => i.CourseId == CourseId)
                        .FirstOrDefaultAsync()??new();

            CourseDto courseDto = new()
            {
                ApplicationUserId = course.ApplicationUserId,
                Aritc = course.Aritc,
                CategoryId = course.CategoryId,
                Certifacate = course.Certifacate,
                CoursDec = course.CoursDec,
                CourseDetails = course.CourseDetails,
                CourseLearns = course.CourseLearns,
                TeacherCourses = course.TeacherCourses,
                CourseName = course.CourseName,
                DowinloadResource = course.DowinloadResource,
                NumberOfHours = course.NumberOfHours,
                Time = course.Time,
                Rate = course.Rate,
                Price = course.Price,
                PriceAfterDiscount = course.PriceAfterDiscount,
                NumberOfLikes = course.NumberOfLikes,
                iDowinload = course.iDowinload,
                CourseImg = course.ImagePath,
                ImageFile = course.ImageFile,
                NumberOfLikesResult = course.NumberOfLikesResult,
                CourseId = course.CourseId,
                

            };

           
            return Ok(courseDto);
        }

        [HttpGet("get-CourseDetails/{CourseId}")]
        public async Task<ActionResult<Course>> GetCourseDetailsAsync(int CourseId)
        {


            var course = await context.Courses.FindAsync(CourseId);
            List<Course> Courses = await context.Courses.ToListAsync();
            if (course == null)
            {
                return NotFound();
            }

            int CountBest = 0;
            int UsersCount = context.ApplicationUsers.Count();

            Course? Course = await context.Courses.Where(i => i.CourseId == CourseId)
                .Include(c => c.Category)
                .Include(c => c.CourseLearns)
                .Include(c => c.CourseDetails)
                .FirstOrDefaultAsync();


            bestSellers = [];

            bestSellers.Add(Courses.Max(i => i.iDowinload));

            if (context.Courses.Where(i => i.iDowinload > 0).Count() > 6)
                CountBest = 6;
            else
                CountBest = context.Courses.Where(i => i.iDowinload > 0).Count();

            while (bestSellers.Count < CountBest)
            {
                bestSellers.Add((from c in Courses
                                 where !bestSellers.Any(i => i == c.iDowinload)
                                 select c).Max(i => i.iDowinload));
            }
            if (Course != null)
            {

                Course.NumberOfLikesResult = (Course.NumberOfLikes * 100 / UsersCount);
                Course.IsBestSeller = bestSellers.Any(i => i == Course.iDowinload);
                Course.Teachers = GetTeachers(Course.CourseId);

            }


            return Ok( Courses.Where(i => i.CourseId == CourseId).First());


        }
        private List<Teacher2>? GetTeachers(int id)
        {
            var x = context.TeacherCourses.Where(i => i.CourseId == id).ToList();
            List<Teacher2>? result = [];
            List<Teacher2> teacher2 = new();
            foreach (var course in x)
            {
                teacher2 = (from t in context.Teachers
                            where t.Id == course.TeacherId
                            select new Teacher2
                            {
                                Id = t.Id,
                                TeacherName = t.TeacherName,
                                ImagePath = t.ImagePath
                            }).ToList();

                result.AddRange(teacher2);
            }
            return result;
        }
        [HttpGet("get-Categories")]
        public async Task<ActionResult<List<Category>>> GetCategoriesAsync()
        {
            return await context.Catogoris.ToListAsync();
        }
        [HttpGet("get-Teachers")]
        public async Task<ActionResult<List<Teacher>>> GetTeachersAsync()
        {
            //if(id == 0)
            return await context.Teachers.ToListAsync();
            //return await  (from t in context.Teachers
            // select new Teacher
            // {
            //     Id= t.Id,
            //     TeacherName=t.TeacherName,
            //     IsSelected= GetIsSelected(context,t.Id,id)
            // }).ToListAsync();
        }

        //private static bool GetIsSelected(ApplicationDbContext context, int teacherId, int courseId)
        //{
        //  return (from t in context.Teachers
        //          join tc in context.TeacherCourses on t.Id equals tc.TeacherId
        //          where t.Id == teacherId && tc.CourseId == courseId
        //          select t
        //     ).ToList().Count > 0;
        //}

        [HttpGet("get-UserId/{Email}")]
        public async Task<ActionResult<ApplicationUser>> GetUserIdAsync(string Email)
        {
          ApplicationUser? user=await context.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == Email);
            if(user == null) return BadRequest();
            return Ok(user);
        }

        [HttpPost("Add-Course")]
        public async  Task<ActionResult<Course>> AddCourseAsync(CourseDto course)
        {

            if(ModelState.IsValid)
            {
                Course course1 = new()
                {
                    ApplicationUserId = course.ApplicationUserId,
                    Aritc=course.Aritc,
                    CategoryId=course.CategoryId,
                    Certifacate=course.Certifacate, 
                    CoursDec=course.CoursDec,
                    CourseDetails=course.CourseDetails,
                    CourseLearns=course.CourseLearns,
                    TeacherCourses=course.TeacherCourses,
                    CourseName=course.CourseName,
                    DowinloadResource=course.DowinloadResource,
                    NumberOfHours=course.NumberOfHours,
                    Time= course.Time,
                    Rate = course.Rate,
                    Price= course.Price,
                    PriceAfterDiscount=course.PriceAfterDiscount,
                    NumberOfLikes=course.NumberOfLikes,
                    iDowinload=course.iDowinload,
                    
                    

                };
              
                    if (course.ImageFile.base64data is not null)
                {
                    string baseUrl = config["BaseAddress"] ?? "";
                    if (!Directory.Exists(env.WebRootPath + "/courses/images"))
                        {
                            Directory.CreateDirectory(env.WebRootPath + "/courses/images/");
                        }
                        string path = "\\courses\\images\\" + course.ImageFile.fileName;
                        var buf = Convert.FromBase64String(course.ImageFile.base64data);

                        if (System.IO.File.Exists(env.WebRootPath + path))
                            System.IO.File.Delete(env.WebRootPath + path);

                        await System.IO.File.WriteAllBytesAsync(env.WebRootPath + path, buf);
                        course1.ImagePath = baseUrl+path;


                    }
            
                context.Courses.Add(course1);
               var res= await context.SaveChangesAsync()>0;
                if (res)
                {
                    return Ok();

                }
                return BadRequest();

            }
            return BadRequest();
        }



        
        [HttpPut("Update-Course")]
        public async  Task<ActionResult<Course>> UpdateCourseAsync(CourseDto course)
        {

            if(ModelState.IsValid)
            {
                Course? course1 =await  context.Courses
                    .Include(i=>i.CourseLearns)
                    .Include(i=>i.CourseDetails)
                    .Include(i=>i.TeacherCourses)
                    .Where(i=>i.CourseId== course.CourseId)
                    .FirstOrDefaultAsync();


           



                if (course1 == null) return BadRequest();

                context.CourseLearns.RemoveRange(course1.CourseLearns);
                context.TeacherCourses.RemoveRange(course1.TeacherCourses);
                context.CourseDetails.RemoveRange(course1.CourseDetails);

                    course1.ApplicationUserId = course.ApplicationUserId;
                    course1.Aritc=course.Aritc;
                    course1.CategoryId=course.CategoryId;
                    course1.Certifacate=course.Certifacate; 
                    course1.CoursDec=course.CoursDec;
                    course1.CourseDetails=course.CourseDetails;
                    course1.CourseLearns=course.CourseLearns;
                    course1.TeacherCourses=course.TeacherCourses;
                    course1.CourseName=course.CourseName;
                    course1.DowinloadResource=course.DowinloadResource;
                    course1.NumberOfHours=course.NumberOfHours;
                    course1.Time= course.Time;
                    course1.Rate = course.Rate;
                    course1.Price= course.Price;
                    course1.PriceAfterDiscount=course.PriceAfterDiscount;
                    course1.NumberOfLikes=course.NumberOfLikes;
                    course1.iDowinload=course.iDowinload;
                    
                    
 
              
                    if (course.ImageFile!=null && !string.IsNullOrEmpty(course.ImageFile.base64data ))
                    {
                    string baseUrl = config["BaseAddress"]??"";

                    if (!Directory.Exists(env.WebRootPath + "/courses/images"))
                        {
                            Directory.CreateDirectory(env.WebRootPath + "/courses/images/");
                        }
                        string path = "\\courses\\images\\" + course.ImageFile.fileName;
                        var buf = Convert.FromBase64String(course.ImageFile.base64data);

                        if (System.IO.File.Exists(env.WebRootPath + path))
                            System.IO.File.Delete(env.WebRootPath + path);

                        await System.IO.File.WriteAllBytesAsync(env.WebRootPath + path, buf);
                        course1.ImagePath = baseUrl+path;


                    }
            
                context.Courses.Update(course1);

                context.Entry(course1).State = EntityState.Modified;

                foreach (var cl in course1.CourseLearns)
                {
                    if (cl.Id != 0)
                    {
                        context.Entry(cl).State = EntityState.Modified;
                       
                    }
                    else
                    {
                        context.Entry(cl).State = EntityState.Added;
                    }

                   
                }
                foreach (var tc in course1.TeacherCourses)
                {
                    if (tc.Id != 0)
                    {
                        context.Entry(tc).State = EntityState.Modified;
                    
                    }
                    else
                    {
                        context.Entry(tc).State = EntityState.Added;
                    }
                   
                }
                foreach (var cd in course1.CourseDetails)
                {
                    if (cd.Id != 0)
                    {
                        context.Entry(cd).State = EntityState.Modified;
                        
                    }
                    else
                    {
                        context.Entry(cd).State = EntityState.Added;
                    }
                   
                }
                

                var res= await context.SaveChangesAsync()>0;
                if (res)
                {
                    return Ok();

                }
                return BadRequest();

            }
            return BadRequest();
        }

        
    }
}
