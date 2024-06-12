using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.Models.VM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 


namespace SiliconWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment env;
        private List<int> BestSellers { get; set; }
        public CourseController(ApplicationDbContext context,UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            this.context = context;
            this.userManager = userManager;
            this.env = env;
            BestSellers = [];
        }
        [HttpGet("get-user/{email}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(string email)
        {
            UserDto userDtos = new();
            var user = await userManager.FindByEmailAsync(email);

            if (user is not null)
            {

                userDtos = new()
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName}  {user.FirstName}",
                    Address1 = user.Address1,
                    Address2 = user.Address2,
                    Bio = user.Bio,
                    City = user.City,
                    PhoneNumber = user.Phone,
                    PersonImg = user.PersonImg
                };

            }
            return Ok(userDtos);
        }


        [HttpGet("get-All/{UserId}")]
        public async Task<ActionResult<List<Course>>> GetCoursesAsync(string UserId)
        {
            int CountBest = 0;
            int UsersCount = context.ApplicationUsers.Count();

            List<Course> Courses = [];
            Courses = await context.Courses
                        .Include(i=>i.Category)
                        .Include(i=>i.ApplicationUser)
                        .Include(i=>i.CourseLearns)
                        .Include(i=>i.CourseDetails)
                        .Include(i=>i.TeacherCourses)
                        .ToListAsync();


            foreach(var  course in Courses)
            {
                CountBest = 0;
                BestSellers = [];

                BestSellers.Add(Courses.Max(i => i.iDowinload));

                if (context.Courses.Where(i => i.iDowinload > 0).Count() > 6)
                    CountBest = 6;
                else
                    CountBest = context.Courses.Where(i => i.iDowinload > 0).Count();

                while (BestSellers.Count < CountBest)
                {
                    BestSellers.Add((from c in Courses
                                     where !BestSellers.Any(i => i == c.iDowinload)
                                     select c).Max(i => i.iDowinload));
                }
                if (course != null)
                {

                    course.NumberOfLikesResult = (course.NumberOfLikes * 100 / UsersCount);
                    course.IsBestSeller = BestSellers.Any(i => i == course.iDowinload);
                    course.Teachers = GetTeachers(course.CourseId);
                    course.IsSaved=context.UserSavedItems.Where(i=>i.CourseId== course.CourseId && i.ApplicationUserId==UserId).Select(i=>i.Is_Saved).FirstOrDefault();
                    course.IsLiked=context.UserSavedItems.Where(i=>i.CourseId== course.CourseId && i.ApplicationUserId==UserId).Select(i => i.Is_Liked).FirstOrDefault();
                }
            }
 
            return Ok(Courses);
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


            BestSellers = [];

            BestSellers.Add(Courses.Max(i => i.iDowinload));

            if (context.Courses.Where(i => i.iDowinload > 0).Count() > 6)
                CountBest = 6;
            else
                CountBest = context.Courses.Where(i => i.iDowinload > 0).Count();

            while (BestSellers.Count < CountBest)
            {
                BestSellers.Add((from c in Courses
                                 where !BestSellers.Any(i => i == c.iDowinload)
                                 select c).Max(i => i.iDowinload));
            }
            if (Course != null)
            {

                Course.NumberOfLikesResult = (Course.NumberOfLikes * 100 / UsersCount);
                Course.IsBestSeller = BestSellers.Any(i => i == Course.iDowinload);
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

 

        [HttpGet("get-UserId/{Email}")]
        public async Task<ActionResult<ApplicationUser>> GetUserIdAsync(string Email)
        {
          ApplicationUser? user=await context.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == Email);
            if(user == null) return BadRequest();
            return Ok(user);
        }

 
        [HttpPost("SaveCourse")]
        public async Task<ActionResult> SaveCourseAsync(SaveCourseDto saveCourseDto )
        {
            Response res = new();
            int CourseId = saveCourseDto.CourseId; int IsSaved= saveCourseDto.IsSaved;
            string Message;

            string? userid = saveCourseDto.UserId;
            if (userid != null)
            {
                if (IsSaved == 1)
                {
                    //Save and Un Save Course
                    UserSavedItem? userSavedItem = context.UserSavedItems.Where(i => i.CourseId == CourseId && i.ApplicationUserId == userid).FirstOrDefault();
                    if (userSavedItem != null)
                    {
                        if (userSavedItem.Is_Saved)
                        {
                            userSavedItem.Is_Saved = false;
                            Message = "The Course  Was UnSaved";
                        }
                        else
                        {
                            userSavedItem.Is_Saved = true;
                            Message = "The Course  Was Saved";
                        }
                        context.UserSavedItems.Update(userSavedItem);
                        await context.SaveChangesAsync();

                        res.IsSuccess = userSavedItem.Is_Saved;
                        res.Message = Message;
                        return Ok(res);
                    }
                    else
                    {
                        Message = "The Course  Was Saved";
                        userSavedItem = new() { ApplicationUserId = userid, CourseId = CourseId, Is_Saved = true, Is_Liked = false };
                        context.UserSavedItems.Add(userSavedItem);
                        await context.SaveChangesAsync();

                        res.IsSuccess = userSavedItem.Is_Saved;
                        res.Message = Message;
                        return Ok(res);
                    }
                }
                else
                {
                    // Like And dislike Course
                    UserSavedItem? userSavedItem = context.UserSavedItems.Where(i => i.CourseId == CourseId && i.ApplicationUserId == userid).FirstOrDefault();
                    if (userSavedItem != null)
                    {
                        if (userSavedItem.Is_Liked)
                        {
                            Course? course1 = await context.Courses.Where(i => i.CourseId == CourseId).FirstOrDefaultAsync();
                            if (course1 != null)
                            {
                                course1.NumberOfLikes = course1.NumberOfLikes - 1;
                                if (course1.NumberOfLikes < 0)
                                    course1.NumberOfLikes = 0;
                                context.Courses.Update(course1);
                                await context.SaveChangesAsync();
                            }
                            userSavedItem.Is_Liked = false;
                            Message = "You  disLike This Course";
                        }
                        else
                        {
                            Course? course1 = await context.Courses.Where(i => i.CourseId == CourseId).FirstOrDefaultAsync();
                            if (course1 != null)
                            {
                                if (course1.NumberOfLikes < 0)
                                    course1.NumberOfLikes = 0;
                                course1.NumberOfLikes = course1.NumberOfLikes + 1;
                                context.Courses.Update(course1);
                                await context.SaveChangesAsync();
                            }
                            userSavedItem.Is_Liked = true;
                            Message = "You  Like This Course";
                        }
                        context.UserSavedItems.Update(userSavedItem);
                        await context.SaveChangesAsync();


                        res.IsSuccess = userSavedItem.Is_Liked;
                        res.Message = Message;
                        return Ok(res);
                        
                    }
                    else
                    {
                        Message = "You  Like This Course";
                        userSavedItem = new() { ApplicationUserId = userid, CourseId = CourseId, Is_Saved = false, Is_Liked = true };
                        context.UserSavedItems.Add(userSavedItem);
                        await context.SaveChangesAsync();

                        Course? course1 = await context.Courses.Where(i => i.CourseId == CourseId).FirstOrDefaultAsync();
                        if (course1 != null)
                        {
                            if (course1.NumberOfLikes < 0)
                                course1.NumberOfLikes = 0;
                            course1.NumberOfLikes = course1.NumberOfLikes + 1;
                            context.Courses.Update(course1);
                            await context.SaveChangesAsync();
                        }

                        res.IsSuccess = userSavedItem.Is_Liked;
                        res.Message = Message;
                        return Ok(res);
                      
                    }

                }



            }

            res.IsSuccess = false;
            res.Message =" Un Error";
            return Ok(res);
 


        }
    }
}
