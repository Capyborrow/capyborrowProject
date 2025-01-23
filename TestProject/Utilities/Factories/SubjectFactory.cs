using capyborrowProject.Models;

namespace TestProject.Utilities.Factories
{
    internal class SubjectFactory
    {
        protected SubjectFactory() { }
        public static Subject CreateEmptySubject()
        {
            return new Subject() 
            {
                Title = "",
                Teacher = TeacherFactory.CreateEmptyTeacher(),
            };
        }
    }
}