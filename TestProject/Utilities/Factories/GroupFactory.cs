using capyborrowProject.Models;

namespace TestProject.Utilities.Factories
{
    internal class GroupFactory
    {
        protected GroupFactory() { }
        public static Group CreateEmptyGroup()
        {
            return new Group 
            {
                Name = "",
                Students = [],
            };
        }
    }
}
