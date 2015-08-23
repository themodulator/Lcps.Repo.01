using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Lcps.DivisionDirectory.Members;



namespace Lcps.Infrastructure
{
    public class LcpsUserStore : IUserStore<DirectoryMember>
    {

        public Task CreateAsync(DirectoryMember user)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(DirectoryMember user)
        {
            throw new System.NotImplementedException();
        }

        public Task<DirectoryMember> FindByIdAsync(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<DirectoryMember> FindByNameAsync(string userName)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(DirectoryMember user)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }

    public class LcpsAccountManager : UserManager<DirectoryMember>
    {
        public LcpsAccountManager()
            :base(new UserStore<DirectoryMember>(new LcpsRepositoryContext()))
        {
            UserValidator = new UserValidator<DirectoryMember>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
        }



        public static LcpsAccountManager Create(IdentityFactoryOptions<LcpsAccountManager> options, IOwinContext context)
        {
            var manager = new LcpsAccountManager();

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<DirectoryMember>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<DirectoryMember>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}
