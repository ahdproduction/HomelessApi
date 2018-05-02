using System;
using System.Linq;
using System.Threading.Tasks;
using HomelessApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomelessApi.Controllers
{
	[Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
	public class AccountController : Controller
	{
		readonly UserManager<User> userManager;

		public AccountController(UserManager<User> userManager)
		{
			this.userManager = userManager;
		}
      
		// POST: api/Account/Register
		[HttpPost]
		[AllowAnonymous]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody]User model)
		{
			try
			{
				if (!ModelState.IsValid || model == null)
				{
					if (model == null)
						return BadRequest("model is null");
					else
						return BadRequest(ModelState);
				}

				var user = new User { UserName = model.UserName, Email = model.Email };

				var result = await userManager.CreateAsync(user, model.Password);
				if (!result.Succeeded)
				{
					if (result.Errors.First().Code == "DuplicateUserName")
						return BadRequest(result.Errors.First().Code);
					else
						return GetErrorResult(result);
				}

				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest($"Quelque chose s'est mal passé : {ex.Message}");
			}
		}

		IActionResult GetErrorResult(IdentityResult result)
		{
			if (result == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			if (!result.Succeeded)
			{
				if (result.Errors != null)
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}

				if (ModelState.IsValid)
				{
					// Aucune erreur ModelState à envoyer, alors retourner simplement un BadRequest vide.
					return BadRequest();
				}

				return BadRequest(ModelState);
			}

			return null;
		}
	}
}
