using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Versioning;
using SkiaSharp;
using webapi.Database;
using webapi.Framework;
using webapi.Helpers;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {

        private readonly UserManager<Models.User> _userManager;
        //private readonly UserContext _userContext;

        public UsersController(IConfiguration config, AppDbContext context, UserManager<Models.User> userManager, /*UserContext userContext,*/ IMapper mapper, ILogger<UsersController> logger)
            :base(config, context, mapper, logger)
        {
            _userManager = userManager;
            //_userContext = userContext;
        }

        // GET: api/Users
        [HttpGet, Authorize(Policy = "Admin")]
        public async Task<IResult> Get([FromQuery] TableQuery? query = null)
        {
            query ??= new TableQuery()
            {
                SortBy = "UserName"
            };
            query.SearchBy = "UserName";

            return await GetEntries(_context.Users, query);
        }


        // GET: api/Users/5
        [HttpGet("{id}"), Authorize(Policy = "Admin")]
        public async Task<IResult> GetUser(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.PersonalDetail)
                .Where(u => u.Id == id)
                .FirstAsync();

            if (user == null)
                return TypedResults.NotFound();

            // Obter o carimbo de segurança
            var securityStamp = await _userManager.GetSecurityStampAsync(user);

            var model = _mapper.Map<ViewModels.UserData>(user);

            model.SecurityStamp = securityStamp;

            return TypedResults.Ok(model);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Edit"), Authorize(Policy = "Admin")]
        public async Task<IResult> PutUser(ViewModels.UserData userForm)
        {
            if (!ModelState.IsValid)
                return TypedResults.ValidationProblem(GetModelErros());

            try
            {
                var user = await _userManager.FindByIdAsync(userForm.Id.ToString());

                if(user == null)
                    return TypedResults.NotFound();

                // Não permitir editar UserName dos utilizadores Default
                var reservedUser = user.UserName == Models.User.USER_ADMIN || user.UserName == Models.User.USER_GUEST;
                var username = user.UserName;
                var email = user.Email;

                // Load personal data (if exists)
                /*await _context.Entry(user)
                    .Reference(u => u.PersonalDetail)
                    .LoadAsync();*/

                _mapper.Map(userForm, user, typeof(ViewModels.UserData), typeof(Models.User));

                if(reservedUser)
                {
                    // Não permitir editar UserName dos utilizadores Default
                    user.UserName = username;
                    user.Email = email;
                    user.State = Framework.BaseEnums.RowState.Valid;
                }

                if(user.PersonalDetail != null)
                {
                    if(user.PersonalDetail.Id == Guid.Empty)
                    {
                        user.PersonalDetail.GenerateId();
                        _context.Entry(user.PersonalDetail).State = EntityState.Added;
                    }
                    else
                        _context.Entry(user.PersonalDetail).State = EntityState.Modified;
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Atualizar o carimbo de segurança
                    await _userManager.UpdateSecurityStampAsync(user);

                    // Update password
                    if (!string.IsNullOrEmpty(userForm.Password))
                    {
                        await _userManager.RemovePasswordAsync(user);
                        var passwordResult = await _userManager.AddPasswordAsync(user, userForm.Password);

                        if (!passwordResult.Succeeded)
                            return ReturnErrors(passwordResult.Errors);

                        // Atualizar o carimbo de segurança
                        await _userManager.UpdateSecurityStampAsync(user);
                    }

                    return TypedResults.Ok();
                }

                // Lidar com falhas na atualização do utilizador
                return ReturnErrors(result.Errors);
            }
            catch (DbUpdateConcurrencyException)
            {
                return TypedResults.Problem("Erro ao editar o utilizador");
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("New"), Authorize(Policy = "Admin")]
        public async Task<IResult> PostUser(ViewModels.UserData userForm)
        {
            if (!ModelState.IsValid)
                return TypedResults.ValidationProblem(GetModelErros());

            Models.User? user = _mapper.Map<Models.User>(userForm);

            try
            {
                user.GenerateId();
                user.State = Framework.BaseEnums.RowState.Valid;
                user.PersonalDetail?.GenerateId();
                if (user.PersonalDetail != null)
                    user.PersonalDetail.State = Framework.BaseEnums.RowState.Valid;

                var result = await (string.IsNullOrEmpty(userForm.Password) ? _userManager.CreateAsync(user) : _userManager.CreateAsync(user, userForm.Password));

                if (result.Succeeded)
                {
                    // Atualizar o carimbo de segurança
                    await _userManager.UpdateSecurityStampAsync(user);

                    return TypedResults.Ok(new { id = user.Id });
                }

                // Lidar com falhas na criação do utilizador
                return ReturnErrors(result.Errors);
            }
            catch (DbUpdateException)
            {
                if (EntryExists(_context.Users, user.Id))
                {
                    return TypedResults.Conflict();
                }
                else
                {
                    return TypedResults.Problem("Erro ao criar o utilizador");
                }
            }
        }

        // DELETE
        [HttpPost("Delete"), Authorize(Policy = "Admin")]
        public async Task<IResult> DeleteUser(Guid id)
        {
            return await DeleteEntry(_context.Users, id, (user) =>
            {
                // Não permitir remover os utilizadores Default
                var valido = user.UserName != Models.User.USER_ADMIN && user.UserName != Models.User.USER_GUEST;

                if (!valido)
                    ModelState.AddModelError("Proibido!", "Você não pode eliminar este utilizador.");
                
                return valido;
            },
            async (entry) =>
            {
                // Remover os dados associados
                await _context.Entry(entry)
                    .Reference(b => b.PersonalDetail)
                    .LoadAsync();

                if (entry.PersonalDetail != null)
                    _context.PersonalDetails.Remove(entry.PersonalDetail);
            });
        }
    }
}
