using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Utilities.AuthUtils
{
    internal static class RoleManagerFactory
    {
        public static RoleManager<IdentityRole> Create()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var roleValidators = new List<IRoleValidator<IdentityRole>>
        {
            new RoleValidator<IdentityRole>()
        };

            var keyNormalizer = new UpperInvariantLookupNormalizer();
            var errors = new IdentityErrorDescriber();
            var logger = new Logger<RoleManager<IdentityRole>>(new LoggerFactory());

            return new RoleManager<IdentityRole>(
                roleStore.Object,
                roleValidators,
                keyNormalizer,
                errors,
                logger);
        }
    }
}
