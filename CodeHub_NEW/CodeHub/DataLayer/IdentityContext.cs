using BusinessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class IdentityContext
    {
        UserManager<User> _userManager;
        CodeHubDbContext _context;

        public IdentityContext(CodeHubDbContext context, UserManager<User> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        #region CRUD

        public async Task CreateUserAsync(User user, string password, Role role)
        {
            try
            {
                IdentityResult result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new ArgumentException(result.Errors.First().Description);
                }

                if (role == Role.ADMINISTRATOR)
                {
                    await _userManager.AddToRoleAsync(user, Role.ADMINISTRATOR.ToString());
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, Role.USER.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<User> LogInUserAsync(string email, string password)
        {
            try
            {
                User user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return null;
                }

                IdentityResult result =
                    await _userManager.PasswordValidators[1].ValidateAsync(_userManager, user, password);

                if (result.Succeeded)
                {
                    return await _context.Users.FindAsync(user.Id);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> ReadUserAsync(string key)
        {
            try
            {
                return await _userManager.FindByIdAsync(key);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> ReadAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* public async Task UpdateUserAsync(string id, string username, string name, int age)
         {
             try
             {
                 if (!string.IsNullOrEmpty(username))
                 {
                     User user = await context.Users.FindAsync(id);
                     context.Entry(user).CurrentValues.SetValues(user);
                     await userManager.UpdateAsync(user);
                 }
             }
             catch (Exception ex)
             {
                 throw ex;
             }
         }
 */
        public async Task DeleteUserByNameAsync(string name)
        {
            try
            {
                User user = await FindUserByNEmailAsync(name);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found for deletion!");
                }

                await _userManager.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> FindUserByNEmailAsync(string name)
        {
            try
            {
                // Identity return Null if there is no user!
                return await _userManager.FindByEmailAsync(name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
