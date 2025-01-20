using capyborrowProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Utilities.Factories
{
    internal class LessonFactory
    {
        protected LessonFactory() { }
        public static Lesson CreateEmptyLesson()
        {
            return new Lesson()
            {
                Location = "",
                Type = Lesson.LessonType.Lecture,
                Date = DateTime.Now,
                Importance = Lesson.LessonImportance.Test,
                Attendances = [],
                Subject = SubjectFactory.CreateEmptySubject(),
                Group = GroupFactory.CreateEmptyGroup(),
            };
        }
    }
}
